using Elasticsearch.Net;
using Nest;
using System.Collections.Generic;

namespace Elasticsearch.BulkAndSearch
{
    public interface IElasticsearchCommand<TEntity> where TEntity : class
    {
        bool Upsert(TEntity document, string type = null, Refresh refresh = Refresh.False);

        IBulkResponse Bulk(IEnumerable<TEntity> documents, string type = null);
    }
}
