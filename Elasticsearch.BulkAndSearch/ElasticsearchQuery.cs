using Elasticsearch.BulkAndSearch.Helpers;
using Elasticsearch.BulkAndSearch.Models;
using Nest;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elasticsearch.BulkAndSearch
{
    public class ElasticsearchQuery<TEntity> : BaseElasticsearch<TEntity>, IElasticsearchQuery<TEntity> where TEntity : class
    {
        public ElasticsearchQuery(ElasticsearchOptions options)
            : base(ConnectionMode.Read, options, null)
        {
        }

        public TEntity Get(object id, string index = null, string type = null)
        {
            try
            {
                var path = this.BuildDocumentPath(id, index, type);
                return this.ElasticClient.Get(path)?.Source;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("404"))
                {
                    return null;
                }

                throw new OperationCanceledException("Invalid result on get", e);
            }
        }

        public async Task<TEntity> GetAsync(object id, string index = null, string type = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var path = this.BuildDocumentPath(id, index, type);
                var response = await this.ElasticClient.GetAsync(path, cancellationToken: cancellationToken);
                return response?.Source;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("404"))
                {
                    return null;
                }

                throw new OperationCanceledException("Invalid result on get", e);
            }
        }

        public ScrollResult<TEntity> Scroll(QueryContainer query, ScrollOptions scrollOptions, string index = null, string type = null)
        {
            var elasticResponse = string.IsNullOrWhiteSpace(scrollOptions.ScrollId) ?
                this.ScrollFromStart(query, scrollOptions, index) :
                this.ScrollFromId(scrollOptions);

            return new ScrollResult<TEntity>
            {
                Total = elasticResponse.Total,
                Items = elasticResponse.Documents,
                ScrollId = elasticResponse.ScrollId
            };
        }

        public async Task<ScrollResult<TEntity>> ScrollAsync(QueryContainer query, ScrollOptions scrollOptions, string index = null, string type = null, CancellationToken cancellationToken = default)
        {
            var elasticResponse = string.IsNullOrWhiteSpace(scrollOptions.ScrollId) ?
                await this.ScrollFromStartAsync(query, scrollOptions, index, cancellationToken) :
                await this.ScrollFromIdAsync(scrollOptions, cancellationToken);

            return new ScrollResult<TEntity>
            {
                Total = elasticResponse.Total,
                Items = elasticResponse.Documents,
                ScrollId = elasticResponse.ScrollId
            };
        }

        public SearchResult<TEntity> Search(QueryContainer query, SearchOptions searchOptions, string index = null, string type = null)
        {
            var descriptor = this.BuildSearchDescriptor(query, searchOptions, index, type);
            try
            {
                var elasticResponse = this.ElasticClient.Search<TEntity>(descriptor);
                return new SearchResult<TEntity>
                {
                    Total = elasticResponse.Total,
                    Items = elasticResponse.Documents
                };
            }
            catch (Exception e)
            {
                throw new OperationCanceledException("Invalid result on search", e);
            }
        }

        public async Task<SearchResult<TEntity>> SearchAsync(QueryContainer query, SearchOptions searchOptions, string index = null, string type = null, CancellationToken cancellationToken = default)
        {
            var descriptor = this.BuildSearchDescriptor(query, searchOptions, index, type);
            try
            {
                var elasticResponse = await this.ElasticClient.SearchAsync<TEntity>(descriptor, cancellationToken);
                return new SearchResult<TEntity>
                {
                    Total = elasticResponse.Total,
                    Items = elasticResponse.Documents
                };
            }
            catch (Exception e)
            {
                throw new OperationCanceledException("Invalid result on search", e);
            }
        }

        private DocumentPath<TEntity> BuildDocumentPath(object id, string index, string type)
        {
            return new DocumentPath<TEntity>(id.ToString())
                .Type(type ?? this.Options.DefaultTypeName)
                .Index(index ?? this.Options.DefaultIndexName);
        }

        private SearchDescriptor<TEntity> BuildScrollSearchDescriptor(QueryContainer query, ScrollOptions scrollOptions, string index)
        {
            var descriptor = new SearchDescriptor<TEntity>();
            descriptor.Index(index ?? $"{this.Options.DefaultIndexName}*")
                .Type(this.Options.DefaultTypeName)
                .AddQuery(query)
                .AddScroll(scrollOptions)
                .AddSorting(scrollOptions)
                .IgnoreUnavailable(true)
                .AllowNoIndices(true)
                .AllowPartialSearchResults(true);

            return descriptor;
        }

        private SearchDescriptor<TEntity> BuildSearchDescriptor(QueryContainer query, SearchOptions searchOptions, string index, string type)
        {
            var descriptor = new SearchDescriptor<TEntity>();
            descriptor.Index(index ?? $"{this.Options.DefaultIndexName}*")
                .Type(type ?? this.Options.DefaultTypeName)
                .AddQuery(query)
                .AddPaging(searchOptions)
                .AddSorting(searchOptions)
                .IgnoreUnavailable(true)
                .AllowNoIndices(true)
                .AllowPartialSearchResults(true);
            return descriptor;
        }

        private ISearchResponse<TEntity> ScrollFromId(ScrollOptions scrollOptions)
        {
            var scrollRequest = new ScrollRequest(scrollOptions.ScrollId, scrollOptions.Scroll);
            try
            {
                return this.ElasticClient.Scroll<TEntity>(scrollRequest);
            }
            catch (Exception e)
            {
                throw new OperationCanceledException("Invalid result on scroll by id", e);
            }
        }

        private async Task<ISearchResponse<TEntity>> ScrollFromIdAsync(ScrollOptions scrollOptions, CancellationToken cancellationToken)
        {
            var scrollRequest = new ScrollRequest(scrollOptions.ScrollId, scrollOptions.Scroll);
            try
            {
                return await this.ElasticClient.ScrollAsync<TEntity>(scrollRequest, cancellationToken);
            }
            catch (Exception e)
            {
                throw new OperationCanceledException("Invalid result on scroll by id", e);
            }
        }

        private ISearchResponse<TEntity> ScrollFromStart(QueryContainer query, ScrollOptions scrollOptions, string index)
        {
            var descriptor = this.BuildScrollSearchDescriptor(query, scrollOptions, index);
            try
            {
                var elasticResponse = this.ElasticClient.Search<TEntity>(descriptor);
                scrollOptions.ScrollId = elasticResponse.ScrollId;
                return elasticResponse;
            }
            catch (Exception e)
            {
                throw new OperationCanceledException("Invalid result on new scroll", e);
            }
        }

        private async Task<ISearchResponse<TEntity>> ScrollFromStartAsync(QueryContainer query, ScrollOptions scrollOptions, string index, CancellationToken cancellationToken)
        {
            var descriptor = this.BuildScrollSearchDescriptor(query, scrollOptions, index);
            try
            {
                var elasticResponse = await this.ElasticClient.SearchAsync<TEntity>(descriptor, cancellationToken);
                scrollOptions.ScrollId = elasticResponse.ScrollId;
                return elasticResponse;
            }
            catch (Exception e)
            {
                throw new OperationCanceledException("Invalid result on new scroll", e);
            }
        }
    }
}