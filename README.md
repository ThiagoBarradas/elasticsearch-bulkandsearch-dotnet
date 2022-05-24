[![Build Status](https://barradas.visualstudio.com/Contributions/_apis/build/status/ThiagoBarradas.elasticsearch-bulkandsearch-dotnet?branchName=develop)](https://barradas.visualstudio.com/Contributions/_build/latest?definitionId=16&branchName=master)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Easy.Elasticsearch.BulkAndSearch.svg)](https://www.nuget.org/packages/Easy.Elasticsearch.BulkAndSearch/)
[![NuGet Version](https://img.shields.io/nuget/v/Easy.Elasticsearch.BulkAndSearch.svg)](https://www.nuget.org/packages/Easy.Elasticsearch.BulkAndSearch/)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=ThiagoBarradas_elasticsearch-bulkandsearch-dotnet&metric=alert_status)](https://sonarcloud.io/dashboard?id=ThiagoBarradas_elasticsearch-bulkandsearch-dotnet)
<!-- [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=ThiagoBarradas_elasticsearch-bulkandsearch-dotnet&metric=coverage)](https://sonarcloud.io/dashboard?id=ThiagoBarradas_elasticsearch-bulkandsearch-dotnet) -->

# Elasticsearch Bulk and Search!

Elasticsearch Bulk and Search is a high level library to make easy basic query operations (get, search and scroll) and index operations (single index or bulk), besides paging, sorting and query buider.

# Sample

Sample Entity Class (Person)
```c#
public class Person
{
    public string Id { get; set; }

    public DateTime CreateDate { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }

    public static string GenerateIndexName(string defaultIndex, Person person)
    {
        var suffix = person.CreateDate.ToString("yyyy-MM");

        return $"{defaultIndex}-{suffix}";
    }
}
```

Init commad and query objects
```c#

var elasticsearchOptions = new ElasticsearchOptions
{
    WriteUrl = "http://localhost:9200",
    ReadUrl = "http://localhost:9200",
    DefaultTypeName = "docs",
    TimeoutInSeconds = 60,
    MaximumRetries = 5,
    User = "user",
    Pass = "pass"
};

IElasticsearchCommand<Person> PersonCommand = new ElasticsearchCommand<Person>(elasticsearchOptions, Person.GenerateIndexName);
IElasticsearchQuery<Person> PersonQuery = new ElasticsearchQuery<Person>(elasticsearchOptions);

```

Using GenerateIndexName you can customize index per document based on your properties (used to create timed index);

Basic commands
```c#
// bulk
var persons = new List<Person>
{
    { new Person { Id = "1", Name = "Thiago Barradas", Age = 27, CreateDate = new DateTime(2019, 01, 01) } },
    { new Person { Id = "8", Name = "Ralph Legal", Age = 29, CreateDate = new DateTime(2018, 12, 01) } },
    { new Person { Id = "9", Name = "James Bond", Age = 30, CreateDate = new DateTime(2018, 12, 10) } },
    { new Person { Id = "10", Name = "John Doe", Age = 54, CreateDate = new DateTime(2018, 11, 01) } },
    { new Person { Id = "11", Name = "Jow Troll Moon Do", Age = 58, CreateDate = new DateTime(2018, 05, 25) } }
};

PersonCommand.Bulk(persons);

// upsert
var otherPerson = new Person { Id = "2", Name = "Rafael Barradas", Age = 25, CreateDate = new DateTime(2018, 12, 01) };

PersonCommand.Upsert(otherPerson);
```

Basic queries
```c#
// get by id 
Person personX = PersonQuery.Get("8");
Person personY = PersonQuery.Get(4);

// search 
var searchOptions = new SearchOptions
{
    Page = 1,
    Size = 1,
    SortField = "name.keyword",
    SortMode = SortMode.ASC
};

var query = Query<Person>.DateRange(i => i.Field("createDate").LessThan("2018-12-01"));

var searchResult = PersonQuery.Search(query, searchOptions);

// scroll
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
```

When some error occurred, an exception is thrown;

## Install via NuGet

```
PM> Install-Package Easy.Elasticsearch.BulkAndSearch
```

## How can I contribute?
Please, refer to [CONTRIBUTING](.github/CONTRIBUTING.md)

## Found something strange or need a new feature?
Open a new Issue following our issue template [ISSUE_TEMPLATE](.github/ISSUE_TEMPLATE.md)

## Changelog
See in [nuget version history](https://www.nuget.org/packages/Elasticsearch.BulkAndSearch)

## Did you like it? Please, make a donate :)

if you liked this project, please make a contribution and help to keep this and other initiatives, send me some Satochis.

BTC Wallet: `1G535x1rYdMo9CNdTGK3eG6XJddBHdaqfX`

![1G535x1rYdMo9CNdTGK3eG6XJddBHdaqfX](https://i.imgur.com/mN7ueoE.png)
