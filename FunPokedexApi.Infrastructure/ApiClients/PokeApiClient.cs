using FunPokedexApi.Application.Interfaces;
using FunPokedexApi.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunPokedexApi.Infrastructure.ApiClients
{
    public class PokeApiClient : IPokeApiClient
    {
        private readonly HttpClient _httpClient;

        public PokeApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<Pokemon?> GetPokemonAsync(string name, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
