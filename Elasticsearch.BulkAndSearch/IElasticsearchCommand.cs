using Elasticsearch.Net;
using System.Collections.Generic;

namespace Elasticsearch.BulkAndSearch
{
    public interface IElasticsearchCommand<TEntity> where TEntity : class
    {
        bool Upsert(TEntity document, string type = null, Refresh refresh = Refresh.False);

        bool Bulk(IEnumerable<TEntity> documents, string type = null);
    }
}
