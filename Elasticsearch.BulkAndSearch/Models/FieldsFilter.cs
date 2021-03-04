using System;
using System.Collections.Generic;
using System.Linq;

namespace Elasticsearch.BulkAndSearch.Models
{
    public class FieldsFilter
    {
        public IEnumerable<string> ToInclude { get; }
        public bool HasFieldsToInclude => ToInclude.Any();

        public IEnumerable<string> ToExclude { get; }
        public bool HasFieldsToExclude => ToExclude.Any();

        public FieldsFilter(IEnumerable<string> toInclude, IEnumerable<string> toExclude)
        {
            this.ToInclude = toInclude ?? throw new ArgumentException(nameof(toInclude));
            this.ToExclude = toExclude ?? throw new ArgumentException(nameof(toExclude));
        }

        public static FieldsFilter WithFields(IEnumerable<string> toInclude) => new FieldsFilter(toInclude, new List<string>(0));
        public static FieldsFilter WithoutFields(IEnumerable<string> toExclude) => new FieldsFilter(new List<string>(0), toExclude);
    }
}
