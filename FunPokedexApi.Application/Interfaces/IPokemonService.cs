using FunPokedexApi.Application.Models;

namespace FunPokedexApi.Application.Interfaces;

public interface IPokemonService
{
    Task<Pokemon?> GetPokemonAsync(string name, CancellationToken cancellationToken = default);
    Task<Pokemon?> GetTranslatedPokemonAsync(string name, CancellationToken cancellationToken = default);
}