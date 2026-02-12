namespace MythApi.Common.Models;

/// <summary>
/// Parameters for pagination requests
/// </summary>
public class PaginationParameters
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 10;
    
    private int _pageSize = DefaultPageSize;
    private int _page = 1;

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Number of items per page (default: 10, max: 100)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? DefaultPageSize : value);
    }

    /// <summary>
    /// Calculate the number of items to skip
    /// </summary>
    public int Skip => (Page - 1) * PageSize;
}
