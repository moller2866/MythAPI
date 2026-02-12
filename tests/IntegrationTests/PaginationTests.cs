using System.Net.Http.Json;
using MythApi.Common.Database.Models;
using MythApi.Common.Models;

namespace IntegrationTests;

[TestFixture]
public class PaginationTests
{
    private CustomWebApplicationFactory<Program> _factory;
    private HttpClient _httpClient;

    [SetUp]
    public void SetUp()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _httpClient = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetAllGods_WithoutPagination_ShouldReturnList()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/gods");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var gods = await response.Content.ReadFromJsonAsync<List<God>>();
        Assert.That(gods, Is.Not.Null);
    }

    [Test]
    public async Task GetAllGods_WithPagination_ShouldReturnPagedResult()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/gods?page=1&pageSize=2");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var pagedResult = await response.Content.ReadFromJsonAsync<PagedResult<God>>();
        
        Assert.That(pagedResult, Is.Not.Null);
        Assert.That(pagedResult.Page, Is.EqualTo(1));
        Assert.That(pagedResult.PageSize, Is.EqualTo(2));
        Assert.That(pagedResult.Items.Count, Is.LessThanOrEqualTo(2));
        Assert.That(pagedResult.TotalCount, Is.GreaterThan(0));
        Assert.That(pagedResult.TotalPages, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetAllGods_WithLargePageSize_ShouldBeConstrainedToMaxSize()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/gods?page=1&pageSize=200");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var pagedResult = await response.Content.ReadFromJsonAsync<PagedResult<God>>();
        
        Assert.That(pagedResult, Is.Not.Null);
        Assert.That(pagedResult.PageSize, Is.EqualTo(100), "PageSize should be constrained to max of 100");
    }

    [Test]
    public async Task GetAllGods_WithInvalidPage_ShouldDefaultToPage1()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/gods?page=0&pageSize=5");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var pagedResult = await response.Content.ReadFromJsonAsync<PagedResult<God>>();
        
        Assert.That(pagedResult, Is.Not.Null);
        Assert.That(pagedResult.Page, Is.EqualTo(1), "Page should default to 1 when invalid");
    }

    [Test]
    public async Task GetAllMythologies_WithoutPagination_ShouldReturnList()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/mythologies");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var mythologies = await response.Content.ReadFromJsonAsync<List<Mythology>>();
        Assert.That(mythologies, Is.Not.Null);
    }

    [Test]
    public async Task GetAllMythologies_WithPagination_ShouldReturnPagedResult()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/mythologies?page=1&pageSize=1");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var pagedResult = await response.Content.ReadFromJsonAsync<PagedResult<Mythology>>();
        
        Assert.That(pagedResult, Is.Not.Null);
        Assert.That(pagedResult.Page, Is.EqualTo(1));
        Assert.That(pagedResult.PageSize, Is.EqualTo(1));
        Assert.That(pagedResult.Items.Count, Is.LessThanOrEqualTo(1));
        Assert.That(pagedResult.TotalCount, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetAllGods_PaginationMetadata_ShouldBeCorrect()
    {
        // Get all gods first to know total count
        var allResponse = await _httpClient.GetAsync("/api/v1/gods");
        var allGods = await allResponse.Content.ReadFromJsonAsync<List<God>>();
        var totalCount = allGods?.Count ?? 0;

        if (totalCount == 0)
        {
            Assert.Inconclusive("No gods in database to test pagination");
            return;
        }

        // Act - Get first page with page size of 1
        var response = await _httpClient.GetAsync("/api/v1/gods?page=1&pageSize=1");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var pagedResult = await response.Content.ReadFromJsonAsync<PagedResult<God>>();
        
        Assert.That(pagedResult, Is.Not.Null);
        Assert.That(pagedResult.TotalCount, Is.EqualTo(totalCount));
        Assert.That(pagedResult.TotalPages, Is.EqualTo(totalCount));
        Assert.That(pagedResult.HasNext, Is.EqualTo(totalCount > 1));
        Assert.That(pagedResult.HasPrevious, Is.False);
    }

    [Test]
    public async Task GetAllGods_PageBeyondTotalPages_ShouldReturnEmptyItems()
    {
        // Act - Request a page that's way beyond what exists
        var response = await _httpClient.GetAsync("/api/v1/gods?page=999&pageSize=10");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var pagedResult = await response.Content.ReadFromJsonAsync<PagedResult<God>>();
        
        Assert.That(pagedResult, Is.Not.Null);
        Assert.That(pagedResult.Items.Count, Is.EqualTo(0));
        Assert.That(pagedResult.HasNext, Is.False);
    }
}
