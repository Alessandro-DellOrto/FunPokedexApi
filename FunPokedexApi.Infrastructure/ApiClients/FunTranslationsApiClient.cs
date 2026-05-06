using System.Net;
using System.Text;
using System.Text.Json;
using FunPokedexApi.Application.Interfaces;
using FunPokedexApi.Infrastructure.DTOs.FunTranslations;

namespace FunPokedexApi.Infrastructure.ApiClients
{
    public class FunTranslationsApiClient : IFunTranslationsApiClient
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FunTranslationsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> TranslateToYodaAsync(string text, CancellationToken cancellationToken = default)
            => await TranslateAsync("translate/yoda", text, cancellationToken);

        public async Task<string?> TranslateToShakespeareAsync(string text, CancellationToken cancellationToken = default)
            => await TranslateAsync("translate/shakespeare", text, cancellationToken);

        private async Task<string?> TranslateAsync(string endpoint, string text, CancellationToken cancellationToken)
        {
            var payload = new StringContent(
                JsonSerializer.Serialize(new { text }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(endpoint, payload, cancellationToken);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)//maybe log this
                return null;

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var dto = JsonSerializer.Deserialize<TranslationResponseDto>(content, JsonOptions);

            return string.IsNullOrWhiteSpace(dto?.Contents?.Translated)
                ? null
                : dto.Contents.Translated;
        }
    }
}