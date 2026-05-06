using FunPokedexApi.Application.Interfaces;
using FunPokedexApi.Application.Models;

namespace FunPokedexApi.Endpoints;

public static class PokemonEndpoints
{
    public static void MapPokemonEndpoints(this WebApplication app)
    {
        app.MapGet("/pokemon/{name}", async (string name, IPokemonService pokemonService, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(name))
                return Results.BadRequest("Pokemon name cannot be empty.");

            var pokemon = await pokemonService.GetPokemonAsync(name, cancellationToken);

            return pokemon is null
                ? Results.NotFound($"Pokemon '{name}' not found.")
                : Results.Ok(pokemon);
        })
        .WithName("GetPokemon")
        .WithTags("Pokemon")
        .Produces<Pokemon>()
        .Produces(404)
        .Produces(400);

        app.MapGet("/pokemon/translated/{name}", async (string name, IPokemonService pokemonService, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(name))
                return Results.BadRequest("Pokemon name cannot be empty.");

            var pokemon = await pokemonService.GetTranslatedPokemonAsync(name, cancellationToken);

            return pokemon is null
                ? Results.NotFound($"Pokemon '{name}' not found.")
                : Results.Ok(pokemon);
        })
        .WithName("GetTranslatedPokemon")
        .WithTags("Pokemon")
        .Produces<Pokemon>()
        .Produces(404)
        .Produces(400);
    }
}