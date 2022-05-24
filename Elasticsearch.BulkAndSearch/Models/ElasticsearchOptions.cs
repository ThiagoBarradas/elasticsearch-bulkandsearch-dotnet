﻿namespace Elasticsearch.BulkAndSearch.Models
{
    public class ElasticsearchOptions
    {
        public string WriteUrl { get; set; }

        public string ReadUrl { get; set; }
        
        public string DefaultIndexName { get; set; }

        public int TimeoutInSeconds { get; set; }

        public int MaximumRetries { get; set; }

        public string Environment { get; set; }
        
        public string User { get; set; }
        
        public string Pass { get; set; }
    }
}
