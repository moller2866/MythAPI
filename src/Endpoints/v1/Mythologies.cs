using Microsoft.AspNetCore.Mvc;
using MythApi.Common.Database.Models;
using MythApi.Common.Models;
using MythApi.Mythologies.Interfaces;

public static class Mythologies
{
    public static void RegisterMythologiesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var mythologies = endpoints.MapGroup("/api/v1/mythologies");

        mythologies.MapGet("", GetAllMythologies);
    }

    public static async Task<IResult> GetAllMythologies(
        IMythologyRepository repository,
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null)
    {
        // If pagination parameters are not provided, return all mythologies (backward compatibility)
        if (page == null && pageSize == null)
        {
            var allMythologies = await repository.GetAllMythologiesAsync();
            return Results.Ok(allMythologies);
        }

        // Use pagination
        var pagination = new PaginationParameters
        {
            Page = page ?? 1,
            PageSize = pageSize ?? 10
        };

        var pagedResult = await repository.GetAllMythologiesAsync(pagination);
        return Results.Ok(pagedResult);
    }
}
