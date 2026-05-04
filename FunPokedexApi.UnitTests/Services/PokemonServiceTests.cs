using FunPokedexApi.Application.Interfaces;
using FunPokedexApi.Application.Models;
using FunPokedexApi.Application.Services;
using Moq;

namespace FunPokedexApi.UnitTests.Services;

public class PokemonServiceTests
{
    private readonly Mock<IPokeApiClient> _pokeApiClientMock;
    private readonly Mock<IFunTranslationsApiClient> _funTranslationsClientMock;
    private readonly PokemonService _sut; // System Under Test

    public PokemonServiceTests()
    {
        _pokeApiClientMock = new Mock<IPokeApiClient>();
        _funTranslationsClientMock = new Mock<IFunTranslationsApiClient>();
        _sut = new PokemonService(_pokeApiClientMock.Object, _funTranslationsClientMock.Object);
    }

    // -------------------------
    // GetPokemonAsync
    // -------------------------
    [Fact]
    public async Task GetPokemonAsync_ValidName_ReturnsPokemon()
    {
        var expected = new Pokemon
        {
            Name = "mewtwo",
            Description = "A rare Pokemon.",
            Habitat = "rare",
            IsLegendary = true
        };

        _pokeApiClientMock
            .Setup(x => x.GetPokemonAsync("mewtwo", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _sut.GetPokemonAsync("mewtwo");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("mewtwo", result.Name);
        Assert.Equal("A rare Pokemon.", result.Description);
    }

    [Fact]
    public async Task GetPokemonAsync_PokemonNotFound_ReturnsNull()
    {
        // Arrange
        _pokeApiClientMock
            .Setup(x => x.GetPokemonAsync("unknown", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pokemon?)null);

        // Act
        var result = await _sut.GetPokemonAsync("unknown");

        // Assert
        Assert.Null(result);
    }

    // -------------------------
    // GetTranslatedPokemonAsync
    // -------------------------

    [Fact]
    public async Task GetTranslatedPokemonAsync_LegendaryPokemon_UsesYodaTranslation()
    {
        // Arrange
        var pokemon = new Pokemon
        {
            Name = "mewtwo",
            Description = "A rare Pokemon.",
            Habitat = "rare",
            IsLegendary = true
        };

        _pokeApiClientMock
            .Setup(x => x.GetPokemonAsync("mewtwo", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pokemon);

        _funTranslationsClientMock
            .Setup(x => x.TranslateToYodaAsync("A rare Pokemon.", It.IsAny<CancellationToken>()))
            .ReturnsAsync("Rare, this Pokemon is.");

        // Act
        var result = await _sut.GetTranslatedPokemonAsync("mewtwo");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Rare, this Pokemon is.", result.Description);
        _funTranslationsClientMock.Verify(x => x.TranslateToYodaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _funTranslationsClientMock.Verify(x => x.TranslateToShakespeareAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetTranslatedPokemonAsync_CaveHabitat_UsesYodaTranslation()
    {
        // Arrange
        var pokemon = new Pokemon
        {
            Name = "zubat",
            Description = "Lives in caves.",
            Habitat = "cave",
            IsLegendary = false
        };

        _pokeApiClientMock
            .Setup(x => x.GetPokemonAsync("zubat", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pokemon);

        _funTranslationsClientMock
            .Setup(x => x.TranslateToYodaAsync("Lives in caves.", It.IsAny<CancellationToken>()))
            .ReturnsAsync("In caves, it lives.");

        // Act
        var result = await _sut.GetTranslatedPokemonAsync("zubat");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("In caves, it lives.", result.Description);
        _funTranslationsClientMock.Verify(x => x.TranslateToYodaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _funTranslationsClientMock.Verify(x => x.TranslateToShakespeareAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetTranslatedPokemonAsync_NotCaveHabitatOrLegendary_UsesShakespeareTranslation()
    {
        // Arrange
        var pokemon = new Pokemon
        {
            Name = "bulbasaur",
            Description = "A grass Pokemon.",
            Habitat = "grassland",
            IsLegendary = false
        };

        _pokeApiClientMock
            .Setup(x => x.GetPokemonAsync("bulbasaur", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pokemon);

        _funTranslationsClientMock
            .Setup(x => x.TranslateToShakespeareAsync("A grass Pokemon.", It.IsAny<CancellationToken>()))
            .ReturnsAsync("A grass Pokemon, forsooth.");

        // Act
        var result = await _sut.GetTranslatedPokemonAsync("bulbasaur");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("A grass Pokemon, forsooth.", result.Description);
        _funTranslationsClientMock.Verify(x => x.TranslateToShakespeareAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _funTranslationsClientMock.Verify(x => x.TranslateToYodaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetTranslatedPokemonAsync_TranslationFails_FallsBackToOriginalDescription()
    {
        // Arrange
        var pokemon = new Pokemon
        {
            Name = "mewtwo",
            Description = "A rare Pokemon.",
            Habitat = "rare",
            IsLegendary = true
        };

        _pokeApiClientMock
            .Setup(x => x.GetPokemonAsync("mewtwo", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pokemon);

        _funTranslationsClientMock
            .Setup(x => x.TranslateToYodaAsync("A rare Pokemon.", It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null); //rate limit or error

        // Act
        var result = await _sut.GetTranslatedPokemonAsync("mewtwo");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("A rare Pokemon.", result.Description); // original description
    }

    [Fact]
    public async Task GetTranslatedPokemonAsync_PokemonNotFound_ReturnsNull()
    {
        // Arrange
        _pokeApiClientMock
            .Setup(x => x.GetPokemonAsync("unknown", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pokemon?)null);

        // Act
        var result = await _sut.GetTranslatedPokemonAsync("unknown");

        // Assert
        Assert.Null(result);
    }
}