using System.Net;
using System.Text;
using System.Text.Json;
using FunPokedexApi.Infrastructure.ApiClients;
using Moq;
using Moq.Protected;

namespace FunPokedexApi.UnitTests.Clients;

public class FunTranslationsClientTests
{
    // Helper method to create a FunTranslationsApiClient with a mocked HttpMessageHandler
    private static FunTranslationsApiClient CreateClient(HttpResponseMessage response)
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
            BaseAddress = new Uri("https://api.funtranslations.com/")
        };

        return new FunTranslationsApiClient(httpClient);
    }

    private static StringContent JsonContent(object obj) =>
        new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    [Fact]
    public async Task TranslateToYodaAsync_ValidText_ReturnsTranslatedText()
    {
        // Arrange
        var apiResponse = new
        {
            contents = new { translated = "Rare, this Pokemon is." }
        };

        var client = CreateClient(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent(apiResponse)
        });

        // Act
        var result = await client.TranslateToYodaAsync("This Pokemon is rare.");

        // Assert
        Assert.Equal("Rare, this Pokemon is.", result);
    }

    [Fact]
    public async Task TranslateToShakespeareAsync_ValidText_ReturnsTranslatedText()
    {
        // Arrange
        var apiResponse = new
        {
            contents = new { translated = "A grass Pokemon, forsooth." }
        };

        var client = CreateClient(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent(apiResponse)
        });

        // Act
        var result = await client.TranslateToShakespeareAsync("A grass Pokemon.");

        // Assert
        Assert.Equal("A grass Pokemon, forsooth.", result);
    }

    [Fact]
    public async Task TranslateToYodaAsync_RateLimit_ReturnsNull()
    {
        // Arrange
        var client = CreateClient(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.TooManyRequests
        });

        // Act
        var result = await client.TranslateToYodaAsync("This Pokemon is rare.");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TranslateToShakespeareAsync_RateLimit_ReturnsNull()
    {
        // Arrange
        var client = CreateClient(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.TooManyRequests
        });

        // Act
        var result = await client.TranslateToShakespeareAsync("A grass Pokemon.");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TranslateToYodaAsync_ServerError_ReturnsNull()
    {
        // Arrange
        var client = CreateClient(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        });

        // Act
        var result = await client.TranslateToYodaAsync("This Pokemon is rare.");

        // Assert
        Assert.Null(result);
    }
}