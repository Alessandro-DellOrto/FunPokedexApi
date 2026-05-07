using FunPokedexApi.Application.Interfaces;
using FunPokedexApi.Application.Models;
using Microsoft.Extensions.Logging;

namespace FunPokedexApi.Application.Services;

public class PokemonService : IPokemonService
{
    private readonly IPokeApiClient _pokeApiClient;
    private readonly IFunTranslationsApiClient _funTranslationsClient;
    private readonly ILogger<PokemonService> _logger;
    private const string YodaHabitat = "cave";

    public PokemonService(IPokeApiClient pokeApiClient, IFunTranslationsApiClient funTranslationsClient, ILogger<PokemonService> logger)
    {
        _pokeApiClient = pokeApiClient;
        _funTranslationsClient = funTranslationsClient;
        _logger = logger;
    }

    public async Task<Pokemon?> GetPokemonAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _pokeApiClient.GetPokemonAsync(name, cancellationToken);
    }

    public async Task<Pokemon?> GetTranslatedPokemonAsync(string name, CancellationToken cancellationToken = default)
    {
        Pokemon? pokemon = await _pokeApiClient.GetPokemonAsync(name, cancellationToken);
        
        if(pokemon is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(pokemon.Description))
        {
            return pokemon;
        }


        bool useYoda = pokemon.IsLegendary ||
            string.Equals(pokemon.Habitat, YodaHabitat, StringComparison.InvariantCultureIgnoreCase);

        string? translated = null;
        try
        {
            translated = useYoda
            ? await _funTranslationsClient.TranslateToYodaAsync(pokemon.Description, cancellationToken)
            : await _funTranslationsClient.TranslateToShakespeareAsync(pokemon.Description, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Translation failed for pokemon '{Name}', falling back to original description.", name);
        }

        if (!string.IsNullOrWhiteSpace(translated))
        {
            return pokemon with { Description = translated };
        }
        else
        {          
            return pokemon;
        }

    }
}