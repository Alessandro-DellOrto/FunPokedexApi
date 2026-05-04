using FunPokedexApi.Application.Interfaces;
using FunPokedexApi.Application.Services;
using FunPokedexApi.Infrastructure.ApiClients;
using Microsoft.Extensions.DependencyInjection;

namespace FunPokedexApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpClient<IPokeApiClient, PokeApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
            });

            services.AddHttpClient<IFunTranslationsApiClient, FunTranslationsApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.funtranslations.com/");
            });

            services.AddScoped<IPokemonService, PokemonService>();

            return services;
        }
    }
}
