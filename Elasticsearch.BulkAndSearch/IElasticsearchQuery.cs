using Elasticsearch.BulkAndSearch.Models;
using Nest;

namespace Elasticsearch.BulkAndSearch
{
    public interface IElasticsearchQuery<T> where T : class
    {
        T Get(object id);

        SearchResult<T> Search(QueryContainer query, SearchOptions searchOptions);

        ScrollResult<T> Scroll(QueryContainer query, ScrollOptions scrollOptions);
    }
}
