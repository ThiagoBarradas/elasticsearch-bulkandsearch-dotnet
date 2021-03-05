using Elasticsearch.BulkAndSearch.Helpers;
using Elasticsearch.BulkAndSearch.Models;
using Elasticsearch.BulkAndSearch.Tests.TestUtilities;
using Nest;
using Xunit;

namespace Elasticsearch.BulkAndSearch.Tests.Helpers
{
    public static class SearchDescriptorHelperTests
    {
        [Fact]
        public static void AddPaging_Should_Add_When_Param_Is_NotNull()
        {
            // arrange
            var descriptor = new SearchDescriptor<object>();
            SearchOptions options = new SearchOptions { Page = 3, Size = 13 };

            // act
            descriptor.AddPaging(options);

            // assert
            var resultAsString = SerializeUtil.Serialize(descriptor);
            Assert.Equal("{\"from\":26,\"size\":13}", resultAsString);
        }

        [Fact]
        public static void AddPaging_Should_Not_Add_When_Param_Is_Null()
        {
            // arrange
            var descriptor = new SearchDescriptor<object>();
            SearchOptions options = null;

            // act
            descriptor.AddPaging(options);

            // assert
            var resultAsString = SerializeUtil.Serialize(descriptor);
            Assert.Equal("{}", resultAsString);
        }

        [Fact]
        public static void AddSorting_Should_Add_Asc_Sorting_When_Param_Is_NotNull_And_SearchOptions()
        {
            // arrange
            var descriptor = new SearchDescriptor<object>();
            SearchOptions options = new SearchOptions { SortField = "createDate", SortMode = Models.SortMode.ASC };

            // act
            descriptor.AddSorting(options);

            // assert
            var resultAsString = SerializeUtil.Serialize(descriptor);
            Assert.Equal("{\"sort\":[{\"createDate\":{\"order\":\"asc\"}}]}", resultAsString);
        }

        [Fact]
        public static void AddSorting_Should_Add_Desc_Sorting_When_Param_Is_NotNull_And_ScrollOptions()
        {
            // arrange
            var descriptor = new SearchDescriptor<object>();
            ScrollOptions options = new ScrollOptions { SortField = "createDate", SortMode = Models.SortMode.DESC };

            // act
            descriptor.AddSorting(options);

            // assert
            var resultAsString = SerializeUtil.Serialize(descriptor);
            Assert.Equal("{\"sort\":[{\"createDate\":{\"order\":\"desc\"}}]}", resultAsString);
        }

        [Fact]
        public static void AddSorting_Should_Not_Add_When_Param_Is_Null()
        {
            // arrange
            var descriptor = new SearchDescriptor<object>();
            SearchOptions options = null;

            // act
            descriptor.AddSorting(options);

            // assert
            var resultAsString = SerializeUtil.Serialize(descriptor);
            Assert.Equal("{}", resultAsString);
        }

        [Fact]
        public static void AddQuery_Should_Add_When_Param_Is_NotNull()
        {
            // arrange
            var descriptor = new SearchDescriptor<object>();
            var query = Query<object>.Term("test_f", "test_v");

            // act
            descriptor.AddQuery(query);

            // assert
            var resultAsString = SerializeUtil.Serialize(descriptor);
            Assert.Equal("{\"query\":{\"term\":{\"test_f\":{\"value\":\"test_v\"}}}}", resultAsString);
        }

        [Fact]
        public static void AddQuery_Should_Not_Add_When_Param_Is_Null()
        {
            // arrange
            var descriptor = new SearchDescriptor<object>();
            QueryContainer query = null;

            // act
            descriptor.AddQuery(query);

            // assert
            var resultAsString = SerializeUtil.Serialize(descriptor);
            Assert.Equal("{}", resultAsString);
        }

        [Fact]
        public static void AddScroll_Should_Add_When_Param_Is_NotNull()
        {
            // arrange
            var descriptor = new SearchDescriptor<object>();
            ScrollOptions options = new ScrollOptions { Size = 100, Scroll = "10m" };

            // act
            descriptor.AddScroll(options);

            // assert
            var resultAsString = SerializeUtil.Serialize(descriptor);
            Assert.Equal("{\"size\":100}", resultAsString);
        }

        [Fact]
        public static void AddScroll_Should_Not_Add_When_Param_Is_Null()
        {
            // arrange
            var descriptor = new SearchDescriptor<object>();
            ScrollOptions options = null;

            // act
            descriptor.AddScroll(options);

            // assert
            var resultAsString = SerializeUtil.Serialize(descriptor);
            Assert.Equal("{}", resultAsString);
        }

        [Fact]
        public static void AddFieldsFilter_Should_Add_WithFields_When_Param_Is_NotNull()
        {
            // arrange
            var descriptor = new SearchDescriptor<object>();
            var fieldsFilter = FieldsFilter.WithFields(new[] { "name" });

            // act
            descriptor.AddFieldsFilter(fieldsFilter);

            // assert
            var resultAsString = SerializeUtil.Serialize(descriptor);
            Assert.Equal("{\"_source\":{\"includes\":[\"name\"]}}", resultAsString);
        }

        [Fact]
        public static void AddFieldsFilter_Should_Add_WithoutFields_When_Param_Is_NotNull()
        {
            // arrange
            var descriptor = new SearchDescriptor<object>();
            var fieldsFilter = FieldsFilter.WithoutFields(new[] { "name" });

            // act
            descriptor.AddFieldsFilter(fieldsFilter);

            // assert
            var resultAsString = SerializeUtil.Serialize(descriptor);
            Assert.Equal("{\"_source\":{\"excludes\":[\"name\"]}}", resultAsString);
        }

        [Fact]
        public static void AddFieldsFilter_Should_Not_Add_When_Param_Is_Null()
        {
            // arrange
            var descriptor = new SearchDescriptor<object>();
            FieldsFilter fieldsFilter = null;

            // act
            descriptor.AddFieldsFilter(fieldsFilter);

            // assert
            var resultAsString = SerializeUtil.Serialize(descriptor);
            Assert.Equal("{}", resultAsString);
        }
    }
}
