using Elasticsearch.BulkAndSearch.Helpers;
using Elasticsearch.BulkAndSearch.Models;
using Nest;
using System.Linq;

namespace Elasticsearch.BulkAndSearch
{
    public class ElasticsearchQuery<T> : BaseElasticsearch<T>, IElasticsearchQuery<T> where T : class
    {
        public ElasticsearchQuery(
            ElasticsearchOptions options)
            : base(ConnectionMode.Read, options, null)
        { }

        public T Get(object id)
        {
            var options = new SearchOptions { Size = 1, Page = 1 };
            var query = new TermQuery
            {
                Field = "_id",
                Value = id
            };

            return this.Search(query, options).Items.FirstOrDefault();
        }

        public SearchResult<T> Search(QueryContainer query, SearchOptions searchOptions)
        {
            SearchDescriptor<T> descriptor = new SearchDescriptor<T>();

            descriptor.Index($"{this.Options.DefaultIndexName}*")
                      .Type(this.Options.DefaultTypeName)
                      .AddQuery(query)
                      .AddPaging(searchOptions)
                      .AddSorting(searchOptions);

            var elasticResponse = this.ElasticClient.Search<T>(descriptor);
            
            return new SearchResult<T>
            {
                Total = elasticResponse.Total,
                Items = elasticResponse.Documents
            };
        }

        public ScrollResult<T> Scroll(QueryContainer query, ScrollOptions scrollOptions)
        {
            ISearchResponse<T> elasticResponse = null;

            if (!string.IsNullOrWhiteSpace(scrollOptions.ScrollId))
            {
                ScrollRequest scrollRequest = new ScrollRequest(scrollOptions.ScrollId, scrollOptions.Scroll);
                elasticResponse = this.ElasticClient.Scroll<T>(scrollRequest);
            }
            else
            {
                SearchDescriptor<T> descriptor = new SearchDescriptor<T>();

                descriptor.Index($"{this.Options.DefaultIndexName}*")
                          .Type(this.Options.DefaultTypeName)
                          .AddQuery(query)
                          .AddScroll(scrollOptions)
                          .AddSorting(scrollOptions);

                elasticResponse = this.ElasticClient.Search<T>(descriptor);
                scrollOptions.ScrollId = elasticResponse.ScrollId;
            }

            return new ScrollResult<T>
            {
                Total = elasticResponse.Total,
                Items = elasticResponse.Documents,
                ScrollId = elasticResponse.ScrollId
            };
        }
    }
}
