using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.BulkAndSearch.Models;
using Nest;

namespace Elasticsearch.BulkAndSearch
{
    public interface IElasticsearchQuery<T> where T : class
    {
        T Get(object id, string index = null, string type = null);

        Task<T> GetAsync(object id, string index = null, string type = null, CancellationToken cancellationToken = default);

        SearchResult<T> Search(QueryContainer query, SearchOptions searchOptions, string index = null, string type = null);

        Task<SearchResult<T>> SearchAsync(QueryContainer query, SearchOptions searchOptions, string index = null, string type = null, CancellationToken cancellationToken = default);

        ScrollResult<T> Scroll(QueryContainer query, ScrollOptions scrollOptions, string index = null, string type = null);

        Task<ScrollResult<T>> ScrollAsync(QueryContainer query, ScrollOptions scrollOptions, string index = null, string type = null, CancellationToken cancellationToken = default);
    }
}