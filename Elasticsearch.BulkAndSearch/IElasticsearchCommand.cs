using Elasticsearch.Net;
using Nest;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elasticsearch.BulkAndSearch
{
    public interface IElasticsearchCommand<in TEntity> where TEntity : class
    {
        bool Upsert(TEntity document, string type = null, Refresh refresh = Refresh.False);

        Task<bool> UpsertAsync(TEntity document, string type = null, Refresh refresh = Refresh.False, CancellationToken cancellationToken = default);

        IBulkResponse Bulk(IEnumerable<TEntity> documents, string type = null);

        Task<IBulkResponse> BulkAsync(IEnumerable<TEntity> documents, string type = null, CancellationToken cancellationToken = default);
    }
}
