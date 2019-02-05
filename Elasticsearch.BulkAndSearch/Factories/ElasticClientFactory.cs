using Elasticsearch.BulkAndSearch.Models;
using Elasticsearch.Net;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Specialized;

namespace Elasticsearch.BulkAndSearch.Factories
{
    public static class ElasticClientFactory
    {
        public static IElasticClient GetInstance(ConnectionMode mode, ElasticsearchOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var uri = (mode == ConnectionMode.Write) ? options.WriteUrl : options.ReadUrl;

            var jsonConverter = new JsonConverter[] { new StringEnumConverter() };
            var connectionPool = new SingleNodeConnectionPool(new Uri(uri));

            var headersCollection = new NameValueCollection
            {
                { nameof(options.Environment), options.Environment }
            };

            var connectionSettings = new ConnectionSettings(
                connectionPool,
                (elasticsearchSerializer, connSettings) => new JsonNetSerializer(
                    elasticsearchSerializer,
                    connSettings,
                    contractJsonConverters: jsonConverter))
                .DefaultIndex(options.DefaultIndexName)
                .DefaultTypeName(options.DefaultTypeName)
                .RequestTimeout(TimeSpan.FromSeconds(options.TimeoutInSeconds))
                .MaximumRetries(options.MaximumRetries)
                .GlobalHeaders(headersCollection)
                .ThrowExceptions();
            
            return new ElasticClient(connectionSettings);
        }
    }
}
