using System.Net;
using System.Text.Json;
using FunPokedexApi.Application.Interfaces;
using FunPokedexApi.Application.Models;
using FunPokedexApi.Infrastructure.DTOs.PokeApi;

namespace FunPokedexApi.Infrastructure.ApiClients
{
    public class PokeApiClient : IPokeApiClient
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public PokeApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Pokemon?> GetPokemonAsync(string name, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"pokemon-species/{name.ToLowerInvariant()}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var dto = JsonSerializer.Deserialize<PokemonSpeciesResponseDto>(content, JsonOptions);

            if (dto is null)
                return null;

            var description = dto.FlavorTextEntries
                .FirstOrDefault(x => x.Language?.Name == "en")
                ?.FlavorText
                .Replace("\n", " ")
                .Replace("\f", " ")
                .Trim();

            return new Pokemon
            {
                Name = name.ToLowerInvariant(),
                Description = description ?? string.Empty,
                Habitat = dto.Habitat?.Name ?? string.Empty,
                IsLegendary = dto.IsLegendary
            };
        }
    }
}