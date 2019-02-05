using System.Collections.Generic;

namespace Elasticsearch.BulkAndSearch
{
    public interface IElasticsearchCommand<T> where T : class
    {
        bool Upsert(T document);

        bool Bulk(IEnumerable<T> documents);
    }
}
