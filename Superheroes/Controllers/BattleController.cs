using System;
using System.Threading.Tasks;
using Functional.Option;
using Microsoft.AspNetCore.Mvc;
using Superheroes.Model;

namespace Superheroes.Controllers
{
    [Route("battle")]
    public class BattleController : Controller
    {
        private readonly ICharactersProvider _charactersProvider;

        public BattleController(ICharactersProvider charactersProvider)
        {
            _charactersProvider = charactersProvider;
        }

        public async Task<IActionResult> Get(string hero, string villain)
        {
            var characters = await _charactersProvider.GetCharactersAsync();
            Option<Character> heroOption = Option.None;
            Option<Character> villainOption = Option.None;
            
            foreach(var character in characters)
            {
                if (heroOption.HasValue && villainOption.HasValue) break;

                if (character.Name.Equals(hero, StringComparison.OrdinalIgnoreCase))
                    heroOption = character;
                else if (character.Name.Equals(villain, StringComparison.OrdinalIgnoreCase))
                    villainOption = character;
            }

            if (!heroOption.HasValue || !villainOption.HasValue) return NotFound();
            if (heroOption.Value.Type != CharacterType.Hero) return BadRequest();
            if (villainOption.Value.Type != CharacterType.Villain) return BadRequest();

            double heroScore = heroOption.Value.Score, villainScore = villainOption.Value.Score;
            if (heroOption.Value.Weakness == villain) heroScore--;
            if (villainOption.Value.Weakness == hero) villainScore--;

            return heroScore > villainScore
                ? Ok(CharacterResponse.FromCharacter(heroOption.Value))
                : Ok(CharacterResponse.FromCharacter(villainOption.Value));

        }
    }
}