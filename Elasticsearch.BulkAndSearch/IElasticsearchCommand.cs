using Nest;
using System.Collections.Generic;

namespace Elasticsearch.BulkAndSearch
{
    public interface IElasticsearchCommand<TEntity> where TEntity : class
    {
        bool Upsert(TEntity document, string type = null);

        IBulkResponse Bulk(IEnumerable<TEntity> documents, string type = null);
    }
}
