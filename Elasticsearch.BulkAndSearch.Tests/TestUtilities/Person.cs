using System;

namespace Elasticsearch.BulkAndSearch.Tests
{
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
}
