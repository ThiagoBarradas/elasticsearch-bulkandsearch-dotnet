using Elasticsearch.BulkAndSearch.Models;
using Nest;
using System.Linq;

namespace Elasticsearch.BulkAndSearch.Helpers
{
    public static class SearchDescriptorHelper
    {
        public static SearchDescriptor<T> AddPaging<T>(this SearchDescriptor<T> descriptor, SearchOptions options) where T : class
        {
            if (options != null)
            {
                var size = options.Size;
                var from = (options.Page - 1) * options.Size;

                descriptor.From(from).Size(size);
            }

            return descriptor;
        }

        public static SearchDescriptor<T> AddScroll<T>(this SearchDescriptor<T> descriptor, ScrollOptions options) where T : class
        {
            if (options != null)
            {
                var size = options.Size;

                descriptor.Size(size).Scroll(options.Scroll);
            }

            return descriptor;
        }

        public static SearchDescriptor<T> AddSorting<T>(this SearchDescriptor<T> descriptor, SearchBaseOptions options) where T : class
        {
            if (options?.SortField != null)
            {
                var sortOrder = (options.SortMode == Models.SortMode.ASC)
                    ? SortOrder.Ascending
                    : SortOrder.Descending;

                descriptor.Sort(sr => sr.Field(options.SortField, sortOrder));
            }

            return descriptor;
        }

        public static SearchDescriptor<T> AddQuery<T>(this SearchDescriptor<T> descriptor, QueryContainer query) where T : class
        {
            if (query != null)
            {
                descriptor.Query(r => query);
            }

            return descriptor;
        }

        public static SearchDescriptor<T> AddFieldsFilter<T>(this SearchDescriptor<T> descriptor, FieldsFilter fieldsFilter) where T : class
        {
            if (fieldsFilter == null)
            {
                return descriptor;
            }

            return descriptor.Source(s =>
            {
                if (fieldsFilter.HasFieldsToInclude)
                {
                    s.Includes(i => i.Fields(fieldsFilter.ToInclude.ToArray()));
                }

                if (fieldsFilter.HasFieldsToExclude)
                {
                    s.Excludes(e => e.Fields(fieldsFilter.ToExclude.ToArray()));
                }

                return s;
            });
        }
    }
}
