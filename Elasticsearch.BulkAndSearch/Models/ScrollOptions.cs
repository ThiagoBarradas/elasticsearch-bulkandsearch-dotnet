namespace Elasticsearch.BulkAndSearch.Models
{
    public class ScrollOptions : SearchBaseOptions
    {
        public string ScrollId { get; set; }

        public string Scroll { get; set; }
    }
}
