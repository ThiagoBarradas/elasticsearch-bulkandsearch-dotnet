namespace Elasticsearch.BulkAndSearch.Models
{
    public class ScrollResult<T> : SearchResult<T> where T : class
    {
        public string ScrollId { get; set; }
    }
}
