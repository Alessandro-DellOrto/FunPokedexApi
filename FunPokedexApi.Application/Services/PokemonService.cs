using FunPokedexApi.Application.Interfaces;
using FunPokedexApi.Application.Models;

namespace FunPokedexApi.Application.Services;

public class PokemonService : IPokemonService
{
    private readonly IPokeApiClient _pokeApiClient;
    private readonly IFunTranslationsApiClient _funTranslationsClient;

    public PokemonService(IPokeApiClient pokeApiClient, IFunTranslationsApiClient funTranslationsClient)
    {
        _pokeApiClient = pokeApiClient;
        _funTranslationsClient = funTranslationsClient;
    }

    public Task<Pokemon?> GetPokemonAsync(string name, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Pokemon?> GetTranslatedPokemonAsync(string name, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}