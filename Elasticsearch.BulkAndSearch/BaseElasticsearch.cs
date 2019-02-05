using Elasticsearch.BulkAndSearch.Factories;
using Elasticsearch.BulkAndSearch.Models;
using Nest;
using System;

namespace Elasticsearch.BulkAndSearch
{
    public abstract class BaseElasticsearch<T> where T : class
    {
        public IElasticClient ElasticClient { get; set; }

        public ElasticsearchOptions Options { get; protected set; }

        public Func<string, T, string> GenerateIndexName { get; protected set; }

        public BaseElasticsearch(
            ConnectionMode mode,
            ElasticsearchOptions options, 
            Func<string, T, string> generateIndexName)
        {
            this.ElasticClient = ElasticClientFactory.GetInstance(mode, options);
            this.GenerateIndexName = generateIndexName;
            this.Options = options;
        }

        public string GetIndexName(T document)
        {
            return this.GenerateIndexName?.Invoke(this.Options.DefaultIndexName, document) ?? this.Options.DefaultIndexName;
        }
    }
}
