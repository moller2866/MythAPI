using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MythApi.Gods.Interfaces;
using MythApi.Common.Database.Models;
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

    public static async Task<IResult> AddOrUpdateGods(List<GodInput> gods, IGodRepository repository)
    {
        var errors = new Dictionary<string, string[]>();

        for (var i = 0; i < gods.Count; i++)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(gods[i]);
            if (!Validator.TryValidateObject(gods[i], context, validationResults, validateAllProperties: true))
            {
                foreach (var result in validationResults)
                {
                    var key = $"[{i}].{result.MemberNames.FirstOrDefault() ?? "Unknown"}";
                    errors[key] = [result.ErrorMessage ?? "Invalid value."];
                }
            }
        }

        if (errors.Count > 0)
            return Results.ValidationProblem(errors);

        var godsResult = await repository.AddOrUpdateGods(gods);
        return Results.Ok(godsResult);
    }

    public static Task<IList<God>> GetAlllGods(IGodRepository repository) => repository.GetAllGodsAsync();
}