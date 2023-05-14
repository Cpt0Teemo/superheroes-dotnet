using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Superheroes.Model;
using Xunit;

namespace Superheroes.Tests.Controllers.BattleController;

public class GetShould
{
    private readonly Mock<ICharactersProvider> _mockCharactersProvider;
    private readonly HttpClient _client;
    
    public GetShould()
    {
        var mockResponse = new []
        {
                new Character
                {
                    Name = "Batman",
                    Score = 8.3,
                    Type = CharacterType.Hero
                },
                new Character
                {
                    Name = "Superman",
                    Score = 8.2,
                    Type = CharacterType.Hero
                },
                new Character
                {
                    Name = "WonderWoman",
                    Score = 7.2,
                    Type = CharacterType.Hero
                },
                new Character
                {
                    Name = "Joker",
                    Score = 8.2,
                    Type = CharacterType.Villain
                },
                new Character
                {
                    Name = "Penguin",
                    Score = 7.1,
                    Type = CharacterType.Villain
                }
            
        };
        _mockCharactersProvider = new Mock<ICharactersProvider>();
        _mockCharactersProvider
            .Setup(provider => provider.GetCharacters())
            .ReturnsAsync(mockResponse);
        var startup = new WebHostBuilder()
            .UseStartup<Startup>()
            .ConfigureServices(x => 
            {
                x.AddSingleton<ICharactersProvider>(_mockCharactersProvider.Object);
            });
        var testServer = new TestServer(startup);
        _client = testServer.CreateClient();
    }
    
    [Fact]
    public async Task ShouldReturn404IfHeroDoesntExist()
    {
        const string fakeHero = "SpearMan";
        const string realVillain = "Joker";

        var response = await _client.GetAsync(URI(fakeHero, realVillain));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldReturn404IfVillainDoesntExist()
    {
        const string realHero = "Batman";
        const string fakeVillain = "Fratster";

        var response = await _client.GetAsync(URI(realHero, fakeVillain));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldReturn400IfHeroIsntHero()
    {
        const string villain1 = "Penguin";
        const string villain2 = "Joker";

        var response = await _client.GetAsync(URI(villain1, villain2));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task ShouldReturn400IfVillainIsntVillain()
    {
        const string hero1 = "Batman";
        const string hero2 = "Superman";

        var response = await _client.GetAsync(URI(hero1, hero2));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task ShouldReturnHeroWhenStronger()
    {
        const string hero = "Batman";
        const string villain = "Joker";

        var response = await _client.GetAsync(URI(hero, villain));

        await AssertIsOkWithCharacter(response, hero);
    }
    
    [Fact]
    public async Task ShouldReturnVillainWhenStronger()
    {
        const string hero = "WonderWoman";
        const string villain = "Joker";

        var response = await _client.GetAsync(URI(hero, villain));

        await AssertIsOkWithCharacter(response, villain);
    }
    
    [Fact]
    public async Task ShouldReturnVillainWhenEquallyStrong()
    {
        const string hero = "Superman";
        const string villain = "Joker";

        var response = await _client.GetAsync(URI(hero, villain));

        await AssertIsOkWithCharacter(response, villain);
    }

    private string URI(string hero, string villain) => $"battle?hero={hero}&villain={villain}";
    
    private async Task AssertIsOkWithCharacter(HttpResponseMessage response, string name)
    {
        var responseJson = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<JObject>(responseJson);

        responseObject.Value<string>("name").Should().Be(name);
    }

}