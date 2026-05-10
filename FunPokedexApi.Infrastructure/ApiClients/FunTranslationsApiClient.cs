using FunPokedexApi.Application.Interfaces;
using FunPokedexApi.Infrastructure.DTOs.FunTranslations;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;

namespace FunPokedexApi.Infrastructure.ApiClients
{
    public class FunTranslationsApiClient : IFunTranslationsApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FunTranslationsApiClient> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FunTranslationsApiClient(HttpClient httpClient, ILogger<FunTranslationsApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
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

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogWarning("FunTranslations rate limit exceeded for endpoint '{Endpoint}'.", endpoint);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("FunTranslations returned {StatusCode} for endpoint '{Endpoint}'.", (int)response.StatusCode, endpoint);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var dto = JsonSerializer.Deserialize<TranslationResponseDto>(content, JsonOptions);

            return string.IsNullOrWhiteSpace(dto?.Contents?.Translated)
                ? null
                : dto.Contents.Translated;
        }
    }
}