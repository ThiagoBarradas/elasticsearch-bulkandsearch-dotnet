﻿using Elasticsearch.BulkAndSearch.Helpers;
using Elasticsearch.BulkAndSearch.Models;
using Elasticsearch.Net;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Elasticsearch.BulkAndSearch.Factories
{
    public static class ElasticClientFactory
    {
        private static JsonSerializerSettings _snakeCaseJsonSerializerSettings;

        public static JsonSerializerSettings JsonSerializerSettings = JsonHelper.SnakeCaseJsonSerializerSettings;

        public static NamingStrategy NamingStrategy = new SnakeCaseNamingStrategy();

        public static IElasticClient GetInstance(ConnectionMode mode, ElasticsearchOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var uri = (mode == ConnectionMode.Write) ? options.WriteUrl : options.ReadUrl;
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
                    () => JsonSerializerSettings,
                    resolver => resolver.NamingStrategy = NamingStrategy))
                .RequestTimeout(TimeSpan.FromSeconds(options.TimeoutInSeconds))
                .MaximumRetries(options.MaximumRetries)
                .GlobalHeaders(headersCollection)
                .ThrowExceptions();

            if (!string.IsNullOrWhiteSpace(options.DefaultIndexName))
            {
                connectionSettings.DefaultIndex(options.DefaultIndexName);
            }

            if (!string.IsNullOrWhiteSpace(options.DefaultTypeName))
            {
                connectionSettings.DefaultTypeName(options.DefaultTypeName);
            }

            return new ElasticClient(connectionSettings);
        }
    }
}
