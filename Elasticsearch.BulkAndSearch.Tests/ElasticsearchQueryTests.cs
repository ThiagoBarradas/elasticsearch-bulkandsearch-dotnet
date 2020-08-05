using Elasticsearch.BulkAndSearch.Models;
using Elasticsearch.BulkAndSearch.Tests.TestUtilities;
using Nest;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Elasticsearch.BulkAndSearch.Tests
{
    public static class ElasticsearchQueryTests
    {
        [Fact]
        public static void Get_Should_Return_Zero_Documents()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, null, null);

            // act
            var result = query.Get("some-id");

            // assert
            Assert.Null(result);
            Assert.Equal("Get", ElasticClientMock.LastElasticClientAction);
        }

        [Fact]
        public static async Task GetAsync_Should_Return_Zero_Documents()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, null, null);

            // act
            var result = await query.GetAsync("some-id");

            // assert
            Assert.Null(result);
            Assert.Equal("GetAsync", ElasticClientMock.LastElasticClientAction);
        }

        [Fact]
        public static void Get_Should_Return_Document()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            var person = new Person { Id = "1", Name = "Thiago Barradas", Age = 27, CreateDate = new DateTime(2019, 01, 01) };
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, person, null);

            // act
            var result = query.Get("some-id");

            // assert
            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
            Assert.Equal("Thiago Barradas", result.Name);
            Assert.Equal("Get", ElasticClientMock.LastElasticClientAction);
        }

        [Fact]
        public static async Task GetAsync_Should_Return_Document()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            var person = new Person { Id = "2", Name = "Bernardo Esbérard", Age = 36, CreateDate = new DateTime(2020, 03, 26) };
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, person, null);

            // act
            var result = await query.GetAsync("some-id");

            // assert
            Assert.NotNull(result);
            Assert.Equal("2", result.Id);
            Assert.Equal("Bernardo Esbérard", result.Name);
            Assert.Equal("GetAsync", ElasticClientMock.LastElasticClientAction);
        }

        [Fact]
        public static void Search_Should_Return_Zero_Document()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            var searchOptions = new SearchOptions { Page = 1, Size = 1 };
            var queryToFilter = Query<Person>.Match(i => i.Field("name").Query("Ralph Barradas"));
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, null, null);

            // act
            var result = query.Search(queryToFilter, searchOptions);

            // assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
            Assert.Equal("{\"from\":0,\"query\":{\"match\":{\"name\":{\"query\":\"Ralph Barradas\"}}},\"size\":1}", ElasticClientMock.LastQueryBody);
            Assert.Equal("Search", ElasticClientMock.LastElasticClientAction);
        }

        [Fact]
        public static async Task SearchAsync_Should_Return_Zero_Document()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            var searchOptions = new SearchOptions { Page = 1, Size = 1 };
            var queryToFilter = Query<Person>.Match(i => i.Field("name").Query("Berna dos Anjos"));
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, null, null);

            // act
            var result = await query.SearchAsync(queryToFilter, searchOptions);

            // assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
            Assert.Equal("{\"from\":0,\"query\":{\"match\":{\"name\":{\"query\":\"Berna dos Anjos\"}}},\"size\":1}", ElasticClientMock.LastQueryBody);
            Assert.Equal("SearchAsync", ElasticClientMock.LastElasticClientAction);
        }

        [Fact]
        public static void Search_Should_Return_Document()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            var person = new Person { Id = "1", Name = "Thiago Barradas", Age = 27, CreateDate = new DateTime(2019, 01, 01) };
            var searchOptions = new SearchOptions { Page = 1, Size = 1 };
            var queryToFilter = Query<Person>.Match(i => i.Field("name").Query("Thiago Barradas"));
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, person, null);

            // act
            var result = query.Search(queryToFilter, searchOptions);

            // assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.Total);
            Assert.Equal("1", result.Items.First().Id);
            Assert.Equal("Thiago Barradas", result.Items.First().Name);
            Assert.Equal("Search", ElasticClientMock.LastElasticClientAction);
            Assert.Equal("{\"from\":0,\"query\":{\"match\":{\"name\":{\"query\":\"Thiago Barradas\"}}},\"size\":1}", ElasticClientMock.LastQueryBody);
        }

        [Fact]
        public static async Task SearchAsync_Should_Return_Document()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            var person = new Person { Id = "2", Name = "Bernardo Esbérard", Age = 27, CreateDate = new DateTime(2019, 01, 01) };
            var searchOptions = new SearchOptions { Page = 1, Size = 1 };
            var queryToFilter = Query<Person>.Match(i => i.Field("name").Query("Bernardo Esbérard"));
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, person, null);

            // act
            var result = await query.SearchAsync(queryToFilter, searchOptions);

            // assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.Total);
            Assert.Equal("2", result.Items.First().Id);
            Assert.Equal("Bernardo Esbérard", result.Items.First().Name);
            Assert.Equal("SearchAsync", ElasticClientMock.LastElasticClientAction);
            Assert.Equal("{\"from\":0,\"query\":{\"match\":{\"name\":{\"query\":\"Bernardo Esbérard\"}}},\"size\":1}", ElasticClientMock.LastQueryBody);
        }

        [Fact]
        public static void Scroll_Should_Return_Zero_Document()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            var scrollOptions = new ScrollOptions { Scroll = "10m", Size = 1 };
            var queryToFilter = Query<Person>.Match(i => i.Field("name").Query("Ralph Barradas"));
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, null, null);

            // act
            var result = query.Scroll(queryToFilter, scrollOptions);

            // assert
            Assert.NotNull(result);
            Assert.Null(result.ScrollId);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
            // The expected value is 'Search' because it will search from start.
            Assert.Equal("Search", ElasticClientMock.LastElasticClientAction); 
            Assert.Equal("{\"query\":{\"match\":{\"name\":{\"query\":\"Ralph Barradas\"}}},\"size\":1}", ElasticClientMock.LastQueryBody);
        }

        [Fact]
        public static async Task ScrollAsync_Should_Return_Zero_Document()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            var scrollOptions = new ScrollOptions { Scroll = "10m", Size = 1 };
            var queryToFilter = Query<Person>.Match(i => i.Field("name").Query("Berna dos Anjos"));
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, null, null);

            // act
            var result = await query.ScrollAsync(queryToFilter, scrollOptions);

            // assert
            Assert.NotNull(result);
            Assert.Null(result.ScrollId);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
            // The expected value is 'SearchAsync' because it will search from start.
            Assert.Equal("SearchAsync", ElasticClientMock.LastElasticClientAction);
            Assert.Equal("{\"query\":{\"match\":{\"name\":{\"query\":\"Berna dos Anjos\"}}},\"size\":1}", ElasticClientMock.LastQueryBody);
        }

        [Fact]
        public static void Scroll_Should_Return_Document()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            var person = new Person { Id = "1", Name = "Thiago Barradas", Age = 27, CreateDate = new DateTime(2019, 01, 01) };
            var scrollOptions = new ScrollOptions { Scroll = "10m", Size = 1 };
            var queryToFilter = Query<Person>.Match(i => i.Field("name").Query("Thiago Barradas"));
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, person, "1234567");

            // act
            var result = query.Scroll(queryToFilter, scrollOptions);

            // assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.Total);
            Assert.Equal("1234567", result.ScrollId);
            Assert.Equal("1", result.Items.First().Id);
            Assert.Equal("Thiago Barradas", result.Items.First().Name);
            // The expected value is 'Search' because it will search from start.
            Assert.Equal("Search", ElasticClientMock.LastElasticClientAction);
            Assert.Equal("{\"query\":{\"match\":{\"name\":{\"query\":\"Thiago Barradas\"}}},\"size\":1}", ElasticClientMock.LastQueryBody);
        }

        [Fact]
        public static async Task ScrollAsync_Should_Return_Document()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            var person = new Person { Id = "2", Name = "Bernardo Esbérard", Age = 36, CreateDate = new DateTime(2020, 03, 26) };
            var scrollOptions = new ScrollOptions { Scroll = "10m", Size = 1 };
            var queryToFilter = Query<Person>.Match(i => i.Field("name").Query("Bernardo Esbérard"));
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, person, "1234567");

            // act
            var result = await query.ScrollAsync(queryToFilter, scrollOptions);

            // assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.Total);
            Assert.Equal("1234567", result.ScrollId);
            Assert.Equal("2", result.Items.First().Id);
            Assert.Equal("Bernardo Esbérard", result.Items.First().Name);
            // The expected value is 'Search' because it will search from start.
            Assert.Equal("SearchAsync", ElasticClientMock.LastElasticClientAction);
            Assert.Equal("{\"query\":{\"match\":{\"name\":{\"query\":\"Bernardo Esbérard\"}}},\"size\":1}", ElasticClientMock.LastQueryBody);
        }

        [Fact]
        public static void Scroll_Should_Return_Document_With_ScrollId()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            var person = new Person { Id = "1", Name = "Thiago Barradas", Age = 27, CreateDate = new DateTime(2019, 01, 01) };
            var scrollOptions = new ScrollOptions { Scroll = "10m", ScrollId = "1234567", Size = 1 };
            var queryToFilter = Query<Person>.Match(i => i.Field("name").Query("Thiago Barradas"));
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, person, "1234567");

            // act
            var result = query.Scroll(queryToFilter, scrollOptions);

            // assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.Total);
            Assert.Equal("1234567", result.ScrollId);
            Assert.Equal("1", result.Items.First().Id);
            Assert.Equal("Thiago Barradas", result.Items.First().Name);
            Assert.Equal("Scroll", ElasticClientMock.LastElasticClientAction);
            Assert.Equal("{\"scroll\":\"10m\",\"scroll_id\":\"1234567\"}", ElasticClientMock.LastQueryBody);
        }

        [Fact]
        public static async Task ScrollAsync_Should_Return_Document_With_ScrollId()
        { 
            // arrage
            var options = ElasticClientMock.GetOptions();
            var query = new ElasticsearchQuery<Person>(options);
            var person = new Person { Id = "2", Name = "Bernardo Esbérard", Age = 36, CreateDate = new DateTime(2020, 03, 26) };
            var scrollOptions = new ScrollOptions { Scroll = "10m", ScrollId = "1234567", Size = 1 };
            var queryToFilter = Query<Person>.Match(i => i.Field("name").Query("Bernardo Esbérard"));
            query.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, person, "1234567");

            // act
            var result = await query.ScrollAsync(queryToFilter, scrollOptions);

            // assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.Total);
            Assert.Equal("1234567", result.ScrollId);
            Assert.Equal("2", result.Items.First().Id);
            Assert.Equal("Bernardo Esbérard", result.Items.First().Name);
            Assert.Equal("ScrollAsync", ElasticClientMock.LastElasticClientAction);
            Assert.Equal("{\"scroll\":\"10m\",\"scroll_id\":\"1234567\"}", ElasticClientMock.LastQueryBody);
        }
    }
}
