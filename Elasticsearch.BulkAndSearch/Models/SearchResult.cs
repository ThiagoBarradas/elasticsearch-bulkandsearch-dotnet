using System.Collections.Generic;

namespace Elasticsearch.BulkAndSearch.Models
{
    public class SearchResult<T> where T : class
    {
        public long Total { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
