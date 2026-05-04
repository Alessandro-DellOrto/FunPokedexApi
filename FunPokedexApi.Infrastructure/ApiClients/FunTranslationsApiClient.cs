using FunPokedexApi.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunPokedexApi.Infrastructure.ApiClients
{
    public class FunTranslationsApiClient : IFunTranslationsApiClient
    {
        private readonly HttpClient _httpClient;

        public FunTranslationsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<string?> TranslateToYodaAsync(string text, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<string?> TranslateToShakespeareAsync(string text, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
