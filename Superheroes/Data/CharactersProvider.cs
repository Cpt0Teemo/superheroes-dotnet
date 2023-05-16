using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Superheroes.Model;

[assembly:InternalsVisibleTo("Superheroes.Tests")]
namespace Superheroes
{
    internal class CharactersProvider : ICharactersProvider
    {
        private const string CharactersUri = "ConnectionStrings:AWS_S3_Characters";
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public CharactersProvider(IConfiguration configuration, HttpClient client = null)
        {
            _configuration = configuration;
            _client = client ?? new HttpClient();
        }

        public async Task<Character[]> GetCharactersAsync()
        {
            var response = await _client.GetAsync(_configuration[CharactersUri]);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<JObject>(responseJson);

            return responseObject.Value<JArray>("items").ToObject<Character[]>();
        }
    }

}