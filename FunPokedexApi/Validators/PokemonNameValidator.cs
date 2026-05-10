using System.Text.RegularExpressions;

namespace FunPokedexApi.Validators;

public static class PokemonNameValidator
{
    private static readonly Regex ValidNameRegex = new(@"^[a-zA-Z0-9\-]+$", RegexOptions.Compiled);


    public static bool IsValid(string? pokemonName) =>
            !string.IsNullOrWhiteSpace(pokemonName) &&
            ValidNameRegex.IsMatch(pokemonName) &&
            !pokemonName.All(char.IsDigit);
}