using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Xunit;

namespace Superheroes.Tests.Data.CharactersProvider;

public class GetCharactersShould
{
    private readonly ICharactersProvider _charactersProvider;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private const string TestURI = "https://s3.eu-west-2.amazonaws.com/build-circle/characters.json";
    
    public GetCharactersShould()
    {
        const string testJsonFile = @"{
        ""items"": [{
            ""name"": ""Batman"",
            ""score"": 8.3,
            ""type"": ""hero"",
            ""weakness"": ""Joker""
        },
        {
            ""name"": ""Joker"",
            ""score"": 8.2,
            ""type"": ""villain""
        }";
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var magicHttpClient = new HttpClient(_mockHttpMessageHandler.Object);
        SetupGetStringAsync(_mockHttpMessageHandler, testJsonFile);
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(config => config[It.IsAny<string>()])
            .Returns(TestURI).Verifiable();
        _charactersProvider = new Superheroes.CharactersProvider(_mockConfiguration.Object, magicHttpClient);
    }
    
    [Fact]
    public async Task ReturnAllCharacters()
    {
        var characters = await _charactersProvider.GetCharactersAsync();

        string[] expectedCharacters = {"Joker", "Batman"};
        characters.Length.Should().Be(2);
        characters.Select(character => character.Name).ToArray().Should().BeEquivalentTo(expectedCharacters);
    }
    
    [Fact]
    public async Task CallCorrectURI()
    {
        await _charactersProvider.GetCharactersAsync();

        _mockConfiguration.Verify(config => config[It.IsAny<string>()], Times.Once);
    }

    //HttpClient.GetStringAsync() is not mockable so we have to mock a protected method it uses
    private void SetupGetStringAsync(Mock<HttpMessageHandler> mockMessageHandler, string returnString)
    {
        mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(returnString)
            })
            .Verifiable();
    }
}