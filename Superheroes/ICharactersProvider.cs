using System.Threading.Tasks;
using Superheroes.Model;

namespace Superheroes
{
    public interface ICharactersProvider
    {
        Task<Character[]> GetCharacters();
    }
}