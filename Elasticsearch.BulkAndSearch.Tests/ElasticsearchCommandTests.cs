using Elasticsearch.BulkAndSearch.Tests.TestUtilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Elasticsearch.BulkAndSearch.Tests
{
    public static class ElasticsearchCommandTests
    {
        [Fact]
        public static void Upsert_Should_Index_In_Default_Index()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var command = new ElasticsearchCommand<Person>(options, null);
            command.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, null, null);
            var person = new Person { Id = "1", Name = "Thiago Barradas", Age = 27, CreateDate = new DateTime(2019, 01, 01) };

            // act
            var result = command.Upsert(person);

            // assert
            Assert.True(result);
            Assert.Equal("Index", ElasticClientMock.LastElasticClientAction);
            Assert.Equal(options.DefaultIndexName, ElasticClientMock.LastProcessedIndex);
            Assert.Equal(person.Id, ElasticClientMock.LastProcessedPerson.Id);
            Assert.Equal(person.Name, ElasticClientMock.LastProcessedPerson.Name);
        }

        [Fact]
        public static async Task UpsertAsync_Should_Index_In_Default_Index()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var command = new ElasticsearchCommand<Person>(options, null);
            command.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, null, null);
            var person = new Person { Id = "2", Name = "Bernardo Esbérard", Age = 36, CreateDate = new DateTime(2020, 03, 26) };

            // act
            var result = await command.UpsertAsync(person);

            // assert
            Assert.True(result);
            Assert.Equal("IndexAsync", ElasticClientMock.LastElasticClientAction);
            Assert.Equal(options.DefaultIndexName, ElasticClientMock.LastProcessedIndex);
            Assert.Equal(person.Id, ElasticClientMock.LastProcessedPerson.Id);
            Assert.Equal(person.Name, ElasticClientMock.LastProcessedPerson.Name);
        }

        [Fact]
        public static void Upsert_Should_Index_In_Default_Index_With_Period()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var command = new ElasticsearchCommand<Person>(options, Person.GenerateIndexName);
            command.ElasticClient = ElasticClientMock.GetElasticClientMock(options, Person.GenerateIndexName, null, null);
            var person = new Person { Id = "1", Name = "Thiago Barradas", Age = 27, CreateDate = new DateTime(2019, 01, 01) };

            // act
            var result = command.Upsert(person);

            // assert
            Assert.True(result);
            Assert.Equal("Index", ElasticClientMock.LastElasticClientAction);
            Assert.Equal(options.DefaultIndexName + "-2019-01", ElasticClientMock.LastProcessedIndex);
            Assert.Equal(person.Id, ElasticClientMock.LastProcessedPerson.Id);
            Assert.Equal(person.Name, ElasticClientMock.LastProcessedPerson.Name);
        }

        [Fact]
        public static async Task UpsertAsync_Should_Index_In_Default_Index_With_Period()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var command = new ElasticsearchCommand<Person>(options, Person.GenerateIndexName);
            command.ElasticClient = ElasticClientMock.GetElasticClientMock(options, Person.GenerateIndexName, null, null);
            var person = new Person { Id = "2", Name = "Bernardo Esbérard", Age = 36, CreateDate = new DateTime(2020, 03, 26) };

            // act
            var result = await command.UpsertAsync(person);

            // assert
            Assert.True(result);
            Assert.Equal("IndexAsync", ElasticClientMock.LastElasticClientAction);
            Assert.Equal(options.DefaultIndexName + "-2020-03", ElasticClientMock.LastProcessedIndex);
            Assert.Equal(person.Id, ElasticClientMock.LastProcessedPerson.Id);
            Assert.Equal(person.Name, ElasticClientMock.LastProcessedPerson.Name);
        }

        [Fact]
        public static void Bulk_Should_Index_In_Default_Index()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var command = new ElasticsearchCommand<Person>(options, null);
            command.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, null, null);
            var persons = new List<Person>
            {
                new Person { Id = "1", Name = "Thiago Barradas", Age = 27, CreateDate = new DateTime(2019, 01, 01) },
                new Person { Id = "2", Name = "Raphael Barradas", Age = 29, CreateDate = new DateTime(2018, 12, 01) }
            };

            // act
            var result = command.Bulk(persons);

            // assert
            Assert.True(result.IsValid);
            Assert.Equal("Bulk", ElasticClientMock.LastElasticClientAction);
            Assert.Equal(options.DefaultIndexName, ElasticClientMock.LastProcessedIndexes[0]);
            Assert.Equal(options.DefaultIndexName, ElasticClientMock.LastProcessedIndexes[1]);
            Assert.Equal(persons[0].Id, ElasticClientMock.LastProcessedPersons[0].Id);
            Assert.Equal(persons[0].Name, ElasticClientMock.LastProcessedPersons[0].Name);
            Assert.Equal(persons[1].Id, ElasticClientMock.LastProcessedPersons[1].Id);
            Assert.Equal(persons[1].Name, ElasticClientMock.LastProcessedPersons[1].Name);
        }

        [Fact]
        public static async Task BulkAsync_Should_Index_In_Default_Index()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var command = new ElasticsearchCommand<Person>(options, null);
            command.ElasticClient = ElasticClientMock.GetElasticClientMock(options, null, null, null);
            var persons = new List<Person>
            {
                new Person { Id = "3", Name = "Bernardo Esbérard", Age = 36, CreateDate = new DateTime(2020, 03, 26) },
                new Person { Id = "4", Name = "Mariana Esbérard", Age = 33, CreateDate = new DateTime(2019, 08, 07) }
            };

            // act
            var result = await command.BulkAsync(persons);

            // assert
            Assert.True(result.IsValid);
            Assert.Equal("BulkAsync", ElasticClientMock.LastElasticClientAction);
            Assert.Equal(options.DefaultIndexName, ElasticClientMock.LastProcessedIndexes[0]);
            Assert.Equal(options.DefaultIndexName, ElasticClientMock.LastProcessedIndexes[1]);
            Assert.Equal(persons[0].Id, ElasticClientMock.LastProcessedPersons[0].Id);
            Assert.Equal(persons[0].Name, ElasticClientMock.LastProcessedPersons[0].Name);
            Assert.Equal(persons[1].Id, ElasticClientMock.LastProcessedPersons[1].Id);
            Assert.Equal(persons[1].Name, ElasticClientMock.LastProcessedPersons[1].Name);
        }

        [Fact]
        public static void Bulk_Should_Index_In_Default_Index_With_Period()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var command = new ElasticsearchCommand<Person>(options, Person.GenerateIndexName);
            command.ElasticClient = ElasticClientMock.GetElasticClientMock(options, Person.GenerateIndexName, null, null);

            var persons = new List<Person>
            {
                new Person { Id = "1", Name = "Thiago Barradas", Age = 27, CreateDate = new DateTime(2019, 01, 01) },
                new Person { Id = "2", Name = "Raphael Barradas", Age = 29, CreateDate = new DateTime(2018, 12, 01) }
            };

            // act
            var result = command.Bulk(persons);

            // assert
            Assert.True(result.IsValid);
            Assert.Equal("Bulk", ElasticClientMock.LastElasticClientAction);
            Assert.Equal(options.DefaultIndexName + "-2019-01", ElasticClientMock.LastProcessedIndexes[0]);
            Assert.Equal(options.DefaultIndexName + "-2018-12", ElasticClientMock.LastProcessedIndexes[1]);
            Assert.Equal(persons[0].Id, ElasticClientMock.LastProcessedPersons[0].Id);
            Assert.Equal(persons[0].Name, ElasticClientMock.LastProcessedPersons[0].Name);
            Assert.Equal(persons[1].Id, ElasticClientMock.LastProcessedPersons[1].Id);
            Assert.Equal(persons[1].Name, ElasticClientMock.LastProcessedPersons[1].Name);
        }

        [Fact]
        public static async Task BulkAsync_Should_Index_In_Default_Index_With_Period()
        {
            // arrage
            var options = ElasticClientMock.GetOptions();
            var command = new ElasticsearchCommand<Person>(options, Person.GenerateIndexName);
            command.ElasticClient = ElasticClientMock.GetElasticClientMock(options, Person.GenerateIndexName, null, null);

            var persons = new List<Person>
            {
                new Person { Id = "3", Name = "Bernardo Esbérard", Age = 36, CreateDate = new DateTime(2020, 03, 26) },
                new Person { Id = "4", Name = "Mariana Esbérard", Age = 33, CreateDate = new DateTime(2019, 08, 07) }
            };

            // act
            var result = await command.BulkAsync(persons);

            // assert
            Assert.True(result.IsValid);
            Assert.Equal("BulkAsync", ElasticClientMock.LastElasticClientAction);
            Assert.Equal(options.DefaultIndexName + "-2020-03", ElasticClientMock.LastProcessedIndexes[0]);
            Assert.Equal(options.DefaultIndexName + "-2019-08", ElasticClientMock.LastProcessedIndexes[1]);
            Assert.Equal(persons[0].Id, ElasticClientMock.LastProcessedPersons[0].Id);
            Assert.Equal(persons[0].Name, ElasticClientMock.LastProcessedPersons[0].Name);
            Assert.Equal(persons[1].Id, ElasticClientMock.LastProcessedPersons[1].Id);
            Assert.Equal(persons[1].Name, ElasticClientMock.LastProcessedPersons[1].Name);
        }
    }
}
