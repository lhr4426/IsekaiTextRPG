public class Enemy
{
    public int Level { get; }
    public string Name { get; }
    public int Hp { get; }
    public int Attack { get; }
    public int Defense { get; }

    public Enemy(int level, string name, int hp, int attack, int defense)
    {
        Level = level;
        Name = name;
        Hp = hp;
        Attack = attack;
        Defense = defense;
    }

    public void showEnemyInfo()
    {
        Console.WriteLine($"Lv.{Level} {Name} (HP: {Hp} / ATK: {Attack} / DEF: {Defense})");
    }
}