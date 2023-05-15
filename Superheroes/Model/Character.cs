namespace Superheroes.Model;

public class Character
{
    public string Name { get; set; }
    public double Score { get; set; }
    public CharacterType Type { get; set; }
    public string Weakness { get; set; }
}

public enum CharacterType
{
    Hero,
    Villain
}