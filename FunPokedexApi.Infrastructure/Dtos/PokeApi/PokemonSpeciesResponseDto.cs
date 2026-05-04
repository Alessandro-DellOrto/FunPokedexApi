using System.Text.Json.Serialization;

namespace FunPokedexApi.Infrastructure.DTOs.PokeApi;

public class PokemonSpeciesResponseDto
{
    [JsonPropertyName("is_legendary")]
    public bool IsLegendary { get; init; }

    [JsonPropertyName("habitat")]
    public HabitatDto? Habitat { get; init; }

    [JsonPropertyName("flavor_text_entries")]
    public List<FlavorTextEntryDto> FlavorTextEntries { get; init; } = [];
}

public class HabitatDto
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}

public class FlavorTextEntryDto
{
    [JsonPropertyName("flavor_text")]
    public string FlavorText { get; init; } = string.Empty;

    [JsonPropertyName("language")]
    public LanguageDto? Language { get; init; }
}

public class LanguageDto
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}