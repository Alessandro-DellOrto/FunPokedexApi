using FunPokedexApi.Application.Models;

namespace FunPokedexApi.Application.Interfaces;

public interface IPokeApiClient
{
    Task<Pokemon?> GetPokemonAsync(string name, CancellationToken cancellationToken = default);
}