using Elasticsearch.Net;
using Nest;
using System.Collections.Generic;

namespace Elasticsearch.BulkAndSearch
{
    public interface IElasticsearchCommand<TEntity> where TEntity : class
    {
        bool Upsert(TEntity document, Refresh refresh = Refresh.False);

        BulkResponse Bulk(IEnumerable<TEntity> documents);
    }
}
