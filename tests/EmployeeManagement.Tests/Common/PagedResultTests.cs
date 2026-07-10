using EmployeeManagement.Application.Common;
using Xunit;

namespace EmployeeManagement.Tests.Common;

public class PagedResultTests
{
    [Theory]
    [InlineData(25, 10, 3)]
    [InlineData(20, 10, 2)]
    [InlineData(1, 10, 1)]
    [InlineData(0, 10, 0)]
    public void TotalPages_IsCalculatedCorrectly(int totalCount, int pageSize, int expectedTotalPages)
    {
        var result = new PagedResult<string> { TotalCount = totalCount, PageSize = pageSize };

        Assert.Equal(expectedTotalPages, result.TotalPages);
    }

    [Fact]
    public void HasPreviousPage_IsFalse_OnFirstPage()
    {
        var result = new PagedResult<string> { PageNumber = 1, PageSize = 10, TotalCount = 30 };

        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public void HasNextPage_IsFalse_OnLastPage()
    {
        var result = new PagedResult<string> { PageNumber = 3, PageSize = 10, TotalCount = 30 };

        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }
}
