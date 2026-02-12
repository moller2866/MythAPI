using Microsoft.AspNetCore.Mvc;
using MythApi.Gods.Interfaces;
using MythApi.Common.Database.Models;
using MythApi.Common.Models;
using MythApi.Gods.Models;

namespace MythApi.Endpoints.v1;
public static class Gods {
    public static void RegisterGodEndpoints(this IEndpointRouteBuilder endpoints) {
        
        var gods = endpoints.MapGroup("/api/v1/gods");


        gods.MapGet("", GetAlllGods);
        gods.MapGet("{id}", (int id, IGodRepository repository) => repository.GetGodAsync(new GodParameter(id)));
        gods.MapGet("search/{name}", (string name, IGodRepository repository, [FromQuery] bool includeAliases = false) => repository.GetGodByNameAsync(new GodByNameParameter(name, includeAliases)));
        gods.MapPost("", AddOrUpdateGods);
    }

    public static Task<List<God>> AddOrUpdateGods(List<GodInput> gods, IGodRepository repository) => repository.AddOrUpdateGods(gods);

    public static async Task<IResult> GetAlllGods(
        IGodRepository repository,
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null)
    {
        // If pagination parameters are not provided, return all gods (backward compatibility)
        if (page == null && pageSize == null)
        {
            var allGods = await repository.GetAllGodsAsync();
            return Results.Ok(allGods);
        }

        // Use pagination
        var pagination = new PaginationParameters
        {
            Page = page ?? 1,
            PageSize = pageSize ?? 10
        };

        var pagedResult = await repository.GetAllGodsAsync(pagination);
        return Results.Ok(pagedResult);
    }
}