using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Superheroes.Model;

namespace Superheroes
{
    internal class CharactersProvider : ICharactersProvider
    {
        private const string CharactersUri = "https://s3.eu-west-2.amazonaws.com/build-circle/characters.json";
        readonly HttpClient _client = new HttpClient();
        

        public async Task<Character[]> GetCharacters()
        {
            var response = await _client.GetAsync(CharactersUri);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<JObject>(responseJson);

            return responseObject.Value<JArray>("items").ToObject<Character[]>();
        }
    }

}