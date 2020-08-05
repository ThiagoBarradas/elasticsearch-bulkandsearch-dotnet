using Elasticsearch.BulkAndSearch.Models;
using Moq;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elasticsearch.BulkAndSearch.Tests.TestUtilities
{
    public static class ElasticClientMock
    {
        public static string LastElasticClientAction { get; set; }

        public static string LastProcessedIndex { get; set; }

        public static List<string> LastProcessedIndexes { get; set; }

        public static Person LastProcessedPerson { get; set; }

        public static List<Person> LastProcessedPersons { get; set; }

        public static string LastQueryBody { get; set; }

        public static IElasticClient GetElasticClientMock(
            ElasticsearchOptions options,
            Func<string, Person, string> generateIndexName,
            Person returnedPerson,
            string scrollId)
        {
            var clientMock = new Mock<IElasticClient>();
            ConfigureClientIndexMock(options, generateIndexName, clientMock);
            ConfigureClientIndexAsyncMock(options, generateIndexName, clientMock);

            ConfigureClientBulkMock(clientMock);
            ConfigureClientBulkAsyncMock(clientMock);

            ConfigureClientGetMock(clientMock, returnedPerson);
            ConfigureClientGetAsyncMock(clientMock, returnedPerson);

            var searchMock = BuildSearchMock(returnedPerson, scrollId);
            ConfigureClientSearchMock(clientMock, searchMock);
            ConfigureClientSearchAsyncMock(clientMock, searchMock);

            ConfigureClientScrollMock(clientMock, searchMock);
            ConfigureClientScrollAsyncMock(clientMock, searchMock);

            return clientMock.Object;
        }

        public static ElasticsearchOptions GetOptions()
        {
            return new ElasticsearchOptions
            {
                DefaultIndexName = "my-index",
                DefaultTypeName = "my-type",
                Environment = "my-env",
                MaximumRetries = 4,
                ReadUrl = "http://esread:9200",
                WriteUrl = "http://eswrite:9200",
                TimeoutInSeconds = 30
            };
        }

        private static Mock<ISearchResponse<Person>> BuildSearchMock(Person returnedPerson, string scrollId)
        {
            var persons = new List<Person>();
            if (returnedPerson != null)
            {
                persons.Add(returnedPerson);
            }

            var searchMock = new Mock<ISearchResponse<Person>>();
            searchMock.SetupGet(m => m.IsValid).Returns(true);
            searchMock.SetupGet(m => m.Total).Returns((returnedPerson == null) ? 0 : 1);
            searchMock.SetupGet(m => m.Documents).Returns(persons);
            searchMock.SetupGet(m => m.ScrollId).Returns(scrollId);
            return searchMock;
        }

        private static void ConfigureClientBulkAsyncMock(Mock<IElasticClient> clientMock)
        {
            var bulkMock = new Mock<IBulkResponse>();
            bulkMock.SetupGet(m => m.IsValid).Returns(true);

            clientMock
                .Setup(m => m.BulkAsync(It.IsAny<BulkDescriptor>(), It.IsAny<CancellationToken>()))
                .Returns((BulkDescriptor descriptor, CancellationToken cancellationToken) =>
                {
                    LastElasticClientAction = "BulkAsync";
                    LastProcessedPersons = new List<Person>();
                    LastProcessedIndexes = new List<string>();
                    var descriptorAsJson = SerializeUtil.Serialize(descriptor);
                    var lines = descriptorAsJson.Split('\n');

                    var isOperation = true;
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        if (isOperation)
                        {
                            var operation = JsonConvert.DeserializeObject<dynamic>(line);
                            LastProcessedIndexes.Add(operation.index._index.ToString());
                            isOperation = false;
                        }
                        else
                        {
                            var document = JsonConvert.DeserializeObject<Person>(line);
                            LastProcessedPersons.Add(document);
                            isOperation = true;
                        }
                    }

                    return Task.FromResult(bulkMock.Object);
                });
        }

        private static void ConfigureClientBulkMock(Mock<IElasticClient> clientMock)
        {
            var bulkMock = new Mock<IBulkResponse>();
            bulkMock.SetupGet(m => m.IsValid).Returns(true);

            clientMock
                .Setup(m => m.Bulk(It.IsAny<BulkDescriptor>()))
                .Returns((BulkDescriptor descriptor) =>
                {
                    LastElasticClientAction = "Bulk";
                    LastProcessedPersons = new List<Person>();
                    LastProcessedIndexes = new List<string>();
                    var descriptorAsJson = SerializeUtil.Serialize(descriptor);
                    var lines = descriptorAsJson.Split('\n');

                    var isOperation = true;
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        if (isOperation)
                        {
                            var operation = JsonConvert.DeserializeObject<dynamic>(line);
                            LastProcessedIndexes.Add(operation.index._index.ToString());
                            isOperation = false;
                        }
                        else
                        {
                            var document = JsonConvert.DeserializeObject<Person>(line);
                            LastProcessedPersons.Add(document);
                            isOperation = true;
                        }
                    }

                    return bulkMock.Object;
                });
        }

        private static void ConfigureClientGetAsyncMock(Mock<IElasticClient> clientMock, Person returnedPerson)
        {
            var getMock = new Mock<IGetResponse<Person>>();
            getMock.SetupGet(m => m.Source).Returns(returnedPerson);

            clientMock
                .Setup(m => m.GetAsync(It.IsAny<DocumentPath<Person>>(), null, It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    LastElasticClientAction = "GetAsync";
                    return Task.FromResult(getMock.Object);
                });
        }

        private static void ConfigureClientGetMock(Mock<IElasticClient> clientMock, Person returnedPerson)
        {
            var getMock = new Mock<IGetResponse<Person>>();
            getMock.SetupGet(m => m.Source).Returns(returnedPerson);

            clientMock
                .Setup(m => m.Get(It.IsAny<DocumentPath<Person>>(), null))
                .Returns(() =>
                {
                    LastElasticClientAction = "Get";
                    return getMock.Object;
                });
        }

        private static void ConfigureClientIndexAsyncMock(
            ElasticsearchOptions options,
            Func<string, Person, string> generateIndexName,
            Mock<IElasticClient> clientMock)
        {
            var responseMock = new Mock<IIndexResponse>();
            responseMock.SetupGet(m => m.IsValid).Returns(true);

            clientMock
                .Setup(m => m.IndexAsync(
                    It.IsAny<object>(),
                    It.IsAny<Func<IndexDescriptor<object>,
                        IIndexRequest<object>>>(),
                    It.IsAny<CancellationToken>()))
                .Returns((object document, Func<IndexDescriptor<object>, IIndexRequest<object>> selector, CancellationToken cancellationToken) =>
                {
                    LastElasticClientAction = "IndexAsync";
                    LastProcessedPerson = (Person)document;
                    LastProcessedIndex =
                        generateIndexName?.Invoke(options.DefaultIndexName, (Person)document)
                        ?? options.DefaultIndexName;
                    return Task.FromResult(responseMock.Object);
                });
        }

        private static void ConfigureClientIndexMock(
                    ElasticsearchOptions options,
            Func<string, Person, string> generateIndexName,
            Mock<IElasticClient> clientMock)
        {
            var responseMock = new Mock<IIndexResponse>();
            responseMock.SetupGet(m => m.IsValid).Returns(true);

            clientMock
                .Setup(m => m.Index(
                    It.IsAny<object>(),
                    It.IsAny<Func<IndexDescriptor<object>,
                        IIndexRequest<object>>>()))
                .Returns((object document, Func<IndexDescriptor<object>, IIndexRequest<object>> selector) =>
                {
                    LastElasticClientAction = "Index";
                    LastProcessedPerson = (Person)document;
                    LastProcessedIndex =
                        generateIndexName?.Invoke(options.DefaultIndexName, (Person)document)
                        ?? options.DefaultIndexName;
                    return responseMock.Object;
                });
        }

        private static void ConfigureClientScrollAsyncMock(Mock<IElasticClient> clientMock, IMock<ISearchResponse<Person>> searchMock)
        {
            clientMock
                .Setup(m => m.ScrollAsync<Person>(It.IsAny<IScrollRequest>(), It.IsAny<CancellationToken>()))
                .Returns((IScrollRequest request, CancellationToken cancellationToken) =>
                {
                    LastElasticClientAction = "ScrollAsync";
                    LastQueryBody = SerializeUtil.Serialize(request);
                    return Task.FromResult(searchMock.Object);
                });
        }

        private static void ConfigureClientScrollMock(Mock<IElasticClient> clientMock, IMock<ISearchResponse<Person>> searchMock)
        {
            clientMock
                .Setup(m => m.Scroll<Person>(It.IsAny<IScrollRequest>()))
                .Returns((IScrollRequest request) =>
                {
                    LastElasticClientAction = "Scroll";
                    LastQueryBody = SerializeUtil.Serialize(request);
                    return searchMock.Object;
                });
        }

        private static void ConfigureClientSearchAsyncMock(Mock<IElasticClient> clientMock, IMock<ISearchResponse<Person>> searchMock)
        {
            clientMock
                .Setup(m => m.SearchAsync<Person>(It.IsAny<ISearchRequest>(), It.IsAny<CancellationToken>()))
                .Returns((ISearchRequest request, CancellationToken cancellationToken) =>
                {
                    LastElasticClientAction = "SearchAsync";
                    LastQueryBody = SerializeUtil.Serialize(request);
                    return Task.FromResult(searchMock.Object);
                });
        }

        private static void ConfigureClientSearchMock(Mock<IElasticClient> clientMock, IMock<ISearchResponse<Person>> searchMock)
        {
            clientMock
                .Setup(m => m.Search<Person>(It.IsAny<ISearchRequest>()))
                .Returns((ISearchRequest request) =>
                {
                    LastElasticClientAction = "Search";
                    LastQueryBody = SerializeUtil.Serialize(request);
                    return searchMock.Object;
                });
        }
    }
}