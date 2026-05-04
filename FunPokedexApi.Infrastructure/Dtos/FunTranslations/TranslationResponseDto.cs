using System.Text.Json.Serialization;

namespace FunPokedexApi.Infrastructure.DTOs.FunTranslations;

public class TranslationResponseDto
{
    [JsonPropertyName("contents")]
    public TranslationContentsDto? Contents { get; init; }
}

public class TranslationContentsDto
{
    [JsonPropertyName("translated")]
    public string Translated { get; init; } = string.Empty;
}