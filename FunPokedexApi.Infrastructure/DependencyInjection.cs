using FunPokedexApi.Application.Interfaces;
using FunPokedexApi.Application.Services;
using FunPokedexApi.Infrastructure.ApiClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FunPokedexApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IPokeApiClient, PokeApiClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["ExternalApis:PokeApi:BaseUrl"]!);
            });

            services.AddHttpClient<IFunTranslationsApiClient, FunTranslationsApiClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["ExternalApis:FunTranslations:BaseUrl"]!);
            });

            services.AddScoped<IPokemonService, PokemonService>();

            return services;
        }
    }
}
