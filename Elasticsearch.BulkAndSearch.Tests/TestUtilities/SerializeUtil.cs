using Elasticsearch.Net;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Text;

namespace Elasticsearch.BulkAndSearch.Tests.TestUtilities
{
    public static class SerializeUtil
    {
        private static JsonConverter[] JsonConverter = new JsonConverter[] { new StringEnumConverter() };

        private static IElasticClient ElasticClient = new ElasticClient(new ConnectionSettings(
                new SingleNodeConnectionPool(new Uri("http://localhost:9200")),
                (elasticsearchSerializer, connSettings) => new JsonNetSerializer(
                    elasticsearchSerializer,
                    connSettings,
                    contractJsonConverters: JsonConverter)));

        public static string Serialize<T>(T obj)
        {
            var stream = new MemoryStream();
            ElasticClient.RequestResponseSerializer.Serialize(obj, stream, SerializationFormatting.None);
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
