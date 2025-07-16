using System;
using System.Collections.Generic;
using System.Linq;

public class BattleScene : GameScene
{
    public override string SceneName => "전투";
    private List<Enemy> enemies = new();

    public override GameScene? StartScene()
    {
        Console.Clear();
        Console.WriteLine("전투를 마주쳤습니다!\n");

        BattleLogger.Init();//로그 초기화
        BattleLogger.Log("전투 시작");

        enemies = Enemy.GenerateEnemies();
        while (GameManager.player.CurrentHP > 0 && enemies.Any(e => e.CurrentHP > 0))
        {
            Console.Clear();

            for (int i = 0; i < enemies.Count; i++)
            {
                Console.Write($"{i + 1}. ");
                enemies[i].showEnemyInfo();
            }

            Console.WriteLine("\n[내정보]");
            Console.WriteLine($"Lv.{GameManager.player.Level} ({GameManager.player.Name}) ({Player.JobsKorean(GameManager.player.Job)})");
            Console.WriteLine($"HP ({GameManager.player.CurrentHP} / {GameManager.player.MaxHP})");

            Console.WriteLine("\n1. 공격 (대상 지정)");
            Console.WriteLine("2. 스킬");
            Console.WriteLine("3. 아이템 사용");
            Console.WriteLine("4. 나의 현재 스탯 보기");
            Console.WriteLine("0. 도망가기\n");
            Console.Write("원하시는 행동을 입력해주세요.\n>> ");

            int? choice = InputHelper.InputNumber(0, 4);
            Console.Clear();

            bool isEnemyTurn = false; //스텟창을 보거나 공격이 아닌 행위를 했을경우 맞지않기위한 변수
            switch (choice)
            {
                case 1:
                    Console.WriteLine("공격할 대상을 선택하세요:");
                    List<int> validIndexes = new();
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (enemies[i].CurrentHP > 0)
                        {
                            validIndexes.Add(i);
                            Console.WriteLine($"{validIndexes.Count}. {enemies[i].Name} (HP: {enemies[i].CurrentHP}/{enemies[i].MaxHP})");
                        }
                    }

                    int? targetSelection = InputHelper.InputNumber(1, validIndexes.Count);
                    if (targetSelection == null) break;

                    int enemyIndex = validIndexes[(int)targetSelection - 1];
                    Enemy target = enemies[enemyIndex];

                    if (target.TryDodge())
                    {
                        Console.WriteLine($"{target.Name}이(가) {GameManager.player.Name}의 공격을 회피했습니다!");
                        BattleLogger.Log($"{target.Name}이(가) {GameManager.player.Name}의 공격을 회피했습니다!");
                    }
                    else
                    {
                        int baseDamage = GameManager.player.BaseAttack + GameManager.player.EquippedItems.Sum(i => i.Attack);
                        float variance = (float)Math.Ceiling(baseDamage * 0.1f);
                        int min = (int)(baseDamage - variance);
                        int max = (int)(baseDamage + variance + 1);
                        int finalDamage = new Random().Next(min, max);

                        if (GameManager.player.IsCriticalHit())
                        {
                            Console.WriteLine("★ 치명타 발동! ★");
                            BattleLogger.Log("★ 치명타 발동! ★");
                            finalDamage = (int)(finalDamage * GameManager.player.CriticalDamage);
                        }

                        int damage = Math.Max(finalDamage - target.Defense, 0);
                        target.CurrentHP -= damage;
                        Console.WriteLine($"{target.Name}에게 {damage}의 피해를 입혔습니다!");
                        BattleLogger.Log($"{target.Name}에게 {damage}의 피해를 입혔습니다!");

                        if (target.CurrentHP <= 0)
                        {
                            Console.WriteLine($"{target.Name}이(가) 쓰러졌습니다!");
                            BattleLogger.Log($"{target.Name}이(가) 쓰러졌습니다!");
                        }
                    }

                    Console.ReadKey();
                    break;

                case 2:
                    Console.WriteLine("스킬 기능 미구현");
                    Console.ReadKey();
                    break;

                case 3:
                    Console.WriteLine("아이템 사용 기능 미구현");
                    Console.ReadKey();
                    break;

                case 4:
                    isEnemyTurn = true;
                    GameManager.player.ShowStatus();
                    Console.ReadKey();
                    break;

                case 0:
                    Console.Write("정말 도망가시겠습니까? (y/n) >> ");
                    string? input = Console.ReadLine();
                    if (input?.ToLower() == "y")
                    {
                        Console.WriteLine("당신은 도망쳤습니다.");
                        BattleLogger.Log("당신은 도망쳤습니다.");
                        Console.ReadKey();
                        return prevScene;
                    }
                    else
                    {
                        Console.WriteLine("도망을 포기했습니다.");
                        Console.ReadKey();
                    }
                    break;

                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ReadKey();
                    break;
            }
            if (!isEnemyTurn)
            {
                foreach (var enemy in enemies.Where(e => e.CurrentHP > 0))
                {
                    if (GameManager.player.TryDodge())
                    {
                        Console.WriteLine($"{GameManager.player.Name}이(가) {enemy.Name}의 공격을 회피했습니다!");
                        BattleLogger.Log($"{GameManager.player.Name}이(가) {enemy.Name}의 공격을 회피했습니다!");
                        continue;
                    }

                    int damage = Math.Max(enemy.Attack - GameManager.player.BaseDefense, 0);
                    GameManager.player.CurrentHP -= damage;
                    Console.WriteLine($"{enemy.Name}의 공격! {damage}의 피해를 입었습니다.");
                    BattleLogger.Log($"{enemy.Name}의 공격! {damage}의 피해를 입었습니다.");
                }
                Console.ReadKey();
            }
        }

        Console.Clear();

        if (GameManager.player.CurrentHP <= 0)
        {
            Console.WriteLine("당신은 쓰러졌습니다... 게임 오버.");
            BattleLogger.Log("당신은 쓰러졌습니다... 게임 오버.");
        }
        else
        {
            ShowResult();
        }

        Console.WriteLine("\n0. 다음");

        return prevScene;
    }
    private void ShowResult()
    {
        Console.WriteLine("\n전투 결과");

        bool allEnemiesDefeated = enemies.All(e => e.CurrentHP <= 0);

        if (allEnemiesDefeated)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Victory!");
            Console.ResetColor();

            int totalGold = enemies.Sum(e => e.RewardGold);
            int totalExp = enemies.Sum(e => e.RewardExp);
            List<Item> totalItems = enemies
                .Where(e => e.RewardItems != null)
                .SelectMany(e => e.RewardItems!)
                .ToList();

            GameManager.player.Gold += totalGold;
            GameManager.player.GainExp(totalExp);
            totalItems.ForEach(item => GameManager.player.Inventory.Add(item));

            List<string> rewardLines = new()
            {
                $"모든 적을 처치했습니다!",
                $"획득 골드 : {totalGold} G",
                $"획득 경험치 : {totalExp} EXP",
            };

            if (totalItems.Count > 0)
            {
                rewardLines.Add("획득 아이템:");
                foreach (var item in totalItems)
                {
                    rewardLines.Add($"- {item.Name}");
                }
            }
            BattleLogger.Log(string.Join("\n", rewardLines));
            UI.DrawTitledBox("보상 획득", rewardLines);
        }
        else
        {
            UI.DrawTitledBox("패배", new List<string>
            {
                "You Lose...",
                "적에게 패배하셨습니다."
            });
        }

        Console.WriteLine("\n0. 다음");
        Console.ReadLine();
    }

}
