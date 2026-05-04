namespace FunPokedexApi.Application.Interfaces;

public interface IFunTranslationsApiClient
{
    Task<string?> TranslateToYodaAsync(string text, CancellationToken cancellationToken = default);
    Task<string?> TranslateToShakespeareAsync(string text, CancellationToken cancellationToken = default);
}