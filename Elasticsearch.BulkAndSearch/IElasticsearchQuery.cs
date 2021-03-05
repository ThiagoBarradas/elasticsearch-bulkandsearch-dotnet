using Elasticsearch.BulkAndSearch.Models;
using Nest;

namespace Elasticsearch.BulkAndSearch
{
    public interface IElasticsearchQuery<T> where T : class
    {
        T Get(object id, string index = null, string type = null);

        SearchResult<T> Search(QueryContainer query, SearchOptions searchOptions, string index = null, string type = null, FieldsFilter fieldsFilter = null);

        ScrollResult<T> Scroll(QueryContainer query, ScrollOptions scrollOptions, string index = null, string type = null, FieldsFilter fieldsFilter = null);
    }
}