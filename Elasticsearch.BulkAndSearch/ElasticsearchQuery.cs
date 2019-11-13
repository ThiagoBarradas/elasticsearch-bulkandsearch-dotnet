using Elasticsearch.BulkAndSearch.Helpers;
using Elasticsearch.BulkAndSearch.Models;
using Nest;
using System;

namespace Elasticsearch.BulkAndSearch
{
    public class ElasticsearchQuery<TEntity> : BaseElasticsearch<TEntity>, IElasticsearchQuery<TEntity> where TEntity : class
    {
        public ElasticsearchQuery(
            ElasticsearchOptions options)
            : base(ConnectionMode.Read, options, null)
        { }

        public TEntity Get(object id, string index = null, string type = null)
        {
            try
            {
                DocumentPath<TEntity> path = new DocumentPath<TEntity>(id.ToString())
                    .Type(type ?? this.Options.DefaultTypeName)
                    .Index(index ?? this.Options.DefaultIndexName);

                return this.ElasticClient.Get(path)?.Source;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public SearchResult<TEntity> Search(QueryContainer query, SearchOptions searchOptions, string index = null, string type = null)
        {
            SearchDescriptor<TEntity> descriptor = new SearchDescriptor<TEntity>();

            descriptor.Index(index ?? $"{this.Options.DefaultIndexName}*")
                      .Type(type ?? this.Options.DefaultTypeName)
                      .AddQuery(query)
                      .AddPaging(searchOptions)
                      .AddSorting(searchOptions)
                      .IgnoreUnavailable(true)
                      .AllowNoIndices(true)
                      .AllowPartialSearchResults(true);

            var elasticResponse = this.ElasticClient.Search<TEntity>(descriptor);
            
            return new SearchResult<TEntity>
            {
                Total = elasticResponse.Total,
                Items = elasticResponse.Documents
            };
        }

        public ScrollResult<TEntity> Scroll(QueryContainer query, ScrollOptions scrollOptions, string index = null, string type = null)
        {
            ISearchResponse<TEntity> elasticResponse = null;

            if (!string.IsNullOrWhiteSpace(scrollOptions.ScrollId))
            {
                ScrollRequest scrollRequest = new ScrollRequest(scrollOptions.ScrollId, scrollOptions.Scroll);
                elasticResponse = this.ElasticClient.Scroll<TEntity>(scrollRequest);
            }
            else
            {
                SearchDescriptor<TEntity> descriptor = new SearchDescriptor<TEntity>();

                descriptor.Index(index ?? $"{this.Options.DefaultIndexName}*")
                          .Type(this.Options.DefaultTypeName)
                          .AddQuery(query)
                          .AddScroll(scrollOptions)
                          .AddSorting(scrollOptions)
                          .IgnoreUnavailable(true)
                          .AllowNoIndices(true)
                          .AllowPartialSearchResults(true);

                elasticResponse = this.ElasticClient.Search<TEntity>(descriptor);
                scrollOptions.ScrollId = elasticResponse.ScrollId;
            }

            return new ScrollResult<TEntity>
            {
                Total = elasticResponse.Total,
                Items = elasticResponse.Documents,
                ScrollId = elasticResponse.ScrollId
            };
        }
    }
}
