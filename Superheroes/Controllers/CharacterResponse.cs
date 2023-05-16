using Superheroes.Model;

namespace Superheroes
{
    public class CharacterResponse
    {
        public string Name { get; set; }
        public double Score { get; set; }
        public string Type { get; set; }
        
        public static CharacterResponse FromCharacter(Character character) =>
            new CharacterResponse()
            {
                Name = character.Name,
                Score = character.Score,
                Type = nameof(character.Type)
            };
    }
}