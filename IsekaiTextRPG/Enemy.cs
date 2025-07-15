public class Enemy
{
    public int Level { get; }
    public string Name { get; }
    public int MaxHP { get; }
    public int CurrentHP { get; set; }
    public int Attack { get; }
    public int Defense { get; }

    public int RewardGold { get; }

    public int RewardExp { get; }

    public List<Item> RewardItems { get; }

    public Enemy(int level, string name, int hp, int attack, int defense, int rewardGold, int rewardExp)
    {
        Level = level;
        Name = name;
        MaxHP = hp;
        Attack = attack;
        Defense = defense;
        RewardGold = rewardGold;
        RewardExp = rewardExp;
    }

    public void showEnemyInfo()
    {
        Console.WriteLine($"Lv.{Level} {Name} (체력: {CurrentHP} / 공격력: {Attack} / 방어력: {Defense})");
    }

    public bool TryDodge()
    {
        return false; // 임시임
    }
}