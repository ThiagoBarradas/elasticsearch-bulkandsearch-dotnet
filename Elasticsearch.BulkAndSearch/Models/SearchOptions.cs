namespace Elasticsearch.BulkAndSearch.Models
{
    public class SearchOptions : SearchBaseOptions
    {
        public SearchOptions()
        {
            this.Page = 1;
        }

        public int Page { get; set; }
    }
}
