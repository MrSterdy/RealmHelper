namespace RealmHelper.Client.WebUi.Utils;

public static class PaginationHelper
{
    public static (int start, int end, int pageSize) GetIndexes(int currentPage, int totalCount)
    {
        var start = Math.Max(0, currentPage - 1) * Constants.PageSize;
        if (start >= totalCount)
            start = Math.Min(totalCount - Constants.PageSize, start - Constants.PageSize);
        
        var end = Math.Min(start + Constants.PageSize, totalCount);

        return (start, end, end - start);
    }

    public static int GetCurrentPage(int endIndex) =>
        (int)Math.Ceiling(endIndex / (float)Constants.PageSize);

    public static bool IsLastPage(int endIndex, int totalCount) =>
        endIndex >= totalCount;
}