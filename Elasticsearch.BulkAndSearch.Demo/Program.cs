using Elasticsearch.BulkAndSearch.Demo.Models;
using Elasticsearch.BulkAndSearch.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elasticsearch.BulkAndSearch.Demo
{
    static class Program
    {
        static IElasticsearchCommand<Person> PersonCommand { get; set; }

        static IElasticsearchQuery<Person> PersonQuery { get; set; }

        static void Main(string[] args)
        {
            // init
            var elasticsearchOptions = new ElasticsearchOptions
            {
                WriteUrl = "http://localhost:9200",
                ReadUrl = "http://localhost:9200",
                DefaultIndexName = "my-index-barradas",
                TimeoutInSeconds = 60,
                MaximumRetries = 5,
                User = "user",
                Pass = "pass"
            };

            PersonCommand = new ElasticsearchCommand<Person>(elasticsearchOptions, Person.GenerateIndexName);
            PersonQuery = new ElasticsearchQuery<Person>(elasticsearchOptions);

            try
            {
                PersonQuery.Get("zz");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            // bulk
            Console.Write("Bulk:");
            var persons = new List<Person>
            {
                { new Person { Id = "1", Name = "Thiago Barradas", Age = 27, CreateDate = new DateTime(2019, 01, 01) } },
                { new Person { Id = "8", Name = "Ralph Legal", Age = 29, CreateDate = new DateTime(2018, 12, 01) } },
                { new Person { Id = "9", Name = "James Bond", Age = 30, CreateDate = new DateTime(2018, 12, 10) } },
                { new Person { Id = "10", Name = "John Doe", Age = 54, CreateDate = new DateTime(2018, 11, 01) } },
                { new Person { Id = "11", Name = "Jow Troll Moon Do", Age = 58, CreateDate = new DateTime(2018, 05, 25) } }
            };

            persons.ForEach(person => Console.WriteLine(person.Name));
            PersonCommand.Bulk(persons);

            // upsert
            Console.WriteLine("Upsert:");
            var otherPerson = new Person { Id = "2", Name = "Rafael Tests", Age = 25, CreateDate = new DateTime(2018, 12, 01) };

            Console.WriteLine(otherPerson.Name);
            PersonCommand.Upsert(otherPerson);

            // get by id 
            Console.WriteLine("Get:");
            Person personX = PersonQuery.Get("8"); // null
            Person personY = PersonQuery.Get("4"); // John Doe

            Console.WriteLine(personX?.Name);
            Console.WriteLine(personY?.Name);

            // search 
            Console.WriteLine("Search:");
            var searchOptions = new SearchOptions
            {
                Page = 1,
                Size = 1,
                SortField = "name.keyword",
                SortMode = BulkAndSearch.Models.SortMode.ASC
            };

            var query = Query<Person>.DateRange(i => i.Field("createDate").LessThan("2018-12-01"));

            var searchResult = PersonQuery.Search(query, searchOptions);
            searchResult.Items.ToList().ForEach(person => Console.WriteLine(person.Name));

            // scroll
            Console.WriteLine("Scroll:");
            var scrollOptions = new ScrollOptions
            {
                Scroll = "1m",
                Size = 2,
                SortField = "name.keyword",
                SortMode = BulkAndSearch.Models.SortMode.ASC
            };

            List<Person> scrollPersons = new List<Person>();

            var scrollResult = PersonQuery.Scroll(query, scrollOptions);
            while (scrollResult.Items.Any())
            {
                scrollPersons.AddRange(scrollResult.Items);

                scrollResult = PersonQuery.Scroll(query, scrollOptions);
            }

            scrollPersons.ForEach(person => Console.WriteLine(person.Name));
        }
    }
}