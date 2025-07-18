using System;
using System.Collections.Generic;
using IsekaiTextRPG;
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

    public Enemy(int level, string name, int hp, int attack, int defense, float dodgeRate, int rewardGold, int rewardExp, List<Item>? rewardItems = null)
    {
        Level = level;
        Name = name;
        MaxHP = hp;
        CurrentHP = MaxHP;
        Attack = attack;
        Defense = defense;
        DodgeRate = dodgeRate;
        RewardGold = rewardGold;
        RewardExp = rewardExp;
        RewardItems = rewardItems ?? new List<Item>();
    }
    public void DrawHealthBar(int currentHP, int maxHP, bool isDead = false, int barWidth = 20)
    {
        if (currentHP < 0) currentHP = 0;

        float ratio = (float)currentHP / maxHP;
        int filledLength = (int)(barWidth * ratio);
        int emptyLength = barWidth - filledLength;

        string filled = new string('█', filledLength);
        string empty = new string('─', emptyLength);

        string hpStatus = isDead ? "Dead" : $"{currentHP}/{maxHP}";

        if (isDead)
            Console.ForegroundColor = ConsoleColor.DarkGray;

        Console.WriteLine($"HP: [{filled}{empty}] {hpStatus}");

        if (isDead)
            Console.ResetColor();
    }
    public void showEnemyInfo()
    {
        bool isDead = CurrentHP <= 0;

        if (isDead)
            Console.ForegroundColor = ConsoleColor.DarkGray;

        Console.WriteLine($"Lv.{Level} {Name} (공격력: {Attack} / 방어력: {Defense})");

        if (isDead)
            Console.ResetColor();

        DrawHealthBar(CurrentHP, MaxHP, isDead);
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
            // 레벨, 이름, 최대 HP, 공격력, 방어력, 회피율, 보상 골드, 보상 경험치
            new Enemy(1, "슬라임", 10, 5, 1, 0.05f, 30, 10),
            new Enemy(2, "고블린", 10, 7, 2, 0.03f, 50, 20),
            new Enemy(3, "하얀 늑대", 10, 9, 3, 0.08f, 70, 30),
            new Enemy(4, "주황 버섯", 10, 8, 4, 0.01f, 60, 25),
            new Enemy(5, "모코코", 15, 12, 5, 0.02f, 100, 40),
            new Enemy(6, "스켈레톤", 20, 15, 6, 0.04f, 150, 60),
        };

        int count = random.Next(1, 6);

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
                CurrentHP = baseEnemy.MaxHP,
                RewardItems = new List<Item>(baseEnemy.RewardItems ?? new List<Item>()) // baseEnemy의 RewardItems를 새로운 리스트로 복사
            };
            selectedEnemies.Add(copy);
        }

        return selectedEnemies;
    }


}