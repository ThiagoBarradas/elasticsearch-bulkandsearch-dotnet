namespace Elasticsearch.BulkAndSearch.Models
{
    public class SearchBaseOptions
    {
        public SearchBaseOptions()
        {
            this.Size = 10;
        }

        public int Size { get; set; }

        public string SortField { get; set; }

        public SortMode SortMode { get; set; }
    }
}
