using System.Net;
using System.Text;
using System.Text.Json;
using FunPokedexApi.Infrastructure.ApiClients;
using Moq;
using Moq.Protected;

namespace FunPokedexApi.UnitTests.Clients;

public class PokeApiClientTests
{
    // Helper method to create a PokeApiClient with a mocked HttpMessageHandler
    private static PokeApiClient CreateClient(HttpResponseMessage response)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://pokeapi.co/api/v2/")
        };

        return new PokeApiClient(httpClient);
    }

    private static StringContent JsonContent(object obj) =>
        new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    [Fact]
    public async Task GetPokemonAsync_ValidName_ReturnsMappedPokemon()
    {
        // Arrange
        var apiResponse = new
        {
            is_legendary = true,
            habitat = new { name = "rare" },
            flavor_text_entries = new[]
            {
                new
                {
                    flavor_text = "A psychic Pokemon.",
                    language = new { name = "en" }
                }
            }
        };

        var client = CreateClient(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent(apiResponse)
        });

        // Act
        var result = await client.GetPokemonAsync("mewtwo");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("mewtwo", result.Name);
        Assert.Equal("rare", result.Habitat);
        Assert.True(result.IsLegendary);
        Assert.Equal("A psychic Pokemon.", result.Description);
    }

    [Fact]
    public async Task GetPokemonAsync_NotFound_ReturnsNull()
    {
        // Arrange
        var client = CreateClient(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        });

        // Act
        var result = await client.GetPokemonAsync("unknownpokemon");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPokemonAsync_NoEnglishDescription_ReturnsEmptyDescription()
    {
        // Arrange
        var apiResponse = new
        {
            is_legendary = false,
            habitat = new { name = "grassland" },
            flavor_text_entries = new[]
            {
                new
                {
                    flavor_text = "Un Pokemon de hierba.",
                    language = new { name = "es" }
                }
            }
        };

        var client = CreateClient(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent(apiResponse)
        });

        // Act
        var result = await client.GetPokemonAsync("bulbasaur");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.Description);
    }

    [Fact]
    public async Task GetPokemonAsync_DescriptionWithSpecialChars_CleansThem()
    {
        // Arrange
        var apiResponse = new
        {
            is_legendary = false,
            habitat = new { name = "grassland" },
            flavor_text_entries = new[]
            {
                new
                {
                    flavor_text = "A grass\nPokemon\fwith newlines.",
                    language = new { name = "en" }
                }
            }
        };

        var client = CreateClient(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent(apiResponse)
        });

        // Act
        var result = await client.GetPokemonAsync("bulbasaur");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("A grass Pokemon with newlines.", result.Description);
    }

    [Fact]
    public async Task GetPokemonAsync_ServerError_ThrowsException()
    {
        // Arrange
        var client = CreateClient(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        });

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetPokemonAsync("mewtwo"));
    }

    [Fact]
    public async Task GetPokemonAsync_BadRequest_ThrowsArgumentException()
    {
        // Arrange
        var client = CreateClient(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest
        });

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.GetPokemonAsync("dd f"));
    }
}