public class Enemy
{
    public int Level { get; set; }
    public string Name { get; set; }
    public int MaxHP { get; set; }
    public int CurrentHP { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public float DodgeRate { get; set; }
    public int RewardGold { get; set; }
    public int RewardExp { get; set; }

    public List<Item> RewardItems { get; set; }

    public Enemy(int level, string name, int hp, int attack, int defense, float dodgeRate, int rewardGold, int rewardExp)
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
        if (currentHP <= 0) //음수로 나누는 경우 대비
        {
            currentHP = 0;
        }
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
    
    public static List<Enemy> GenerateEnemies()
    {
        var random = new Random();

        List<Enemy> monsterPresets = new List<Enemy>()
        {
            new Enemy(1, "슬라임", 10, 5, 1, 0.05f, 30, 10),
            new Enemy(2, "고블린", 10, 7, 2, 0.03f, 50, 20),
            new Enemy(3, "늑대", 10, 9, 3, 0.08f, 70, 30),
            new Enemy(4, "좀비", 10, 8, 4, 0.01f, 60, 25),
            new Enemy(5, "오크", 30, 12, 5, 0.02f, 100, 40)
        };

        int count = random.Next(1, 5);

        List<Enemy> selectedEnemies = new List<Enemy>();
        for (int i = 0; i < count; i++)
        {
            Enemy baseEnemy = monsterPresets[random.Next(monsterPresets.Count)];
            Enemy copy = new Enemy(
                baseEnemy.Level,
                baseEnemy.Name,
                baseEnemy.MaxHP,
                baseEnemy.Attack,
                baseEnemy.Defense,
                baseEnemy.DodgeRate,
                baseEnemy.RewardGold,
                baseEnemy.RewardExp
            )
            {
                CurrentHP = baseEnemy.MaxHP
            };
            selectedEnemies.Add(copy);
        }

        return selectedEnemies;
    }


}