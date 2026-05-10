using FunPokedexApi.Validators;

namespace FunPokedexApi.UnitTests.Validators;

public class PokemonNameValidatorTests
{
    [Theory]
    [InlineData("mewtwo")]
    [InlineData("mr-mime")]
    [InlineData("nidoran-f")]
    [InlineData("porygon-z")]
    [InlineData("ho-oh")]
    public void IsValid_ValidName_ReturnsTrue(string name)
    {
        Assert.True(PokemonNameValidator.IsValid(name));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("dd f")]
    [InlineData("nidoran♀")]
    [InlineData("type: null")]
    [InlineData("mr. mime")]
    public void IsValid_InvalidName_ReturnsFalse(string? name)
    {
        Assert.False(PokemonNameValidator.IsValid(name));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("150")]
    [InlineData("9999")]
    public void IsValid_NumericId_ReturnsFalse(string name)
    {
        Assert.False(PokemonNameValidator.IsValid(name));
    }

    [Theory]
    [InlineData("porygon2")]
    [InlineData("type-null")]
    public void IsValid_NameWithNumbers_ReturnsTrue(string name)
    {
        Assert.True(PokemonNameValidator.IsValid(name));
    }
}