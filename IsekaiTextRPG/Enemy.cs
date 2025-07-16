public class Enemy
{
    public int Level { get;  set; }
    public string Name { get;  set; }
    public int MaxHP { get;  set; }
    public int CurrentHP { get; set; }
    public int Attack { get;  set; }
    public int Defense { get;  set; }
    public float DodgeRate { get; set; }
    public int RewardGold { get;  set; }
    public int RewardExp { get; set; }

    public List<Item> RewardItems { get; set; }

    public Enemy(int level, string name, int hp, int attack, int defense,float dodgeRate, int rewardGold, int rewardExp)
    {
        Level = level;
        Name = name;
        MaxHP = hp;
        Attack = attack;
        Defense = defense;
        DodgeRate = dodgeRate;
        RewardGold = rewardGold;
        RewardExp = rewardExp;
    }
    public void DrawHealthBar(int currentHP, int maxHP, int barWidth = 20)
    {
        float ratio = (float)currentHP / maxHP;
        int filledLength = (int)(barWidth * ratio);
        int emptyLength = barWidth - filledLength;

        string filled = new string('█', filledLength);
        string empty = new string('─', emptyLength);

        Console.WriteLine($"HP: [{filled}{empty}] {currentHP}/{maxHP}");
    }
    public void showEnemyInfo()
    {
        Console.WriteLine($"Lv.{Level} {Name} (공격력: {Attack} / 방어력: {Defense})");
        DrawHealthBar(CurrentHP, MaxHP);
    }

    public bool TryDodge()
    {
        Random rand = new Random();
        float a = (float)rand.NextDouble();
        return a < DodgeRate;
    }
}