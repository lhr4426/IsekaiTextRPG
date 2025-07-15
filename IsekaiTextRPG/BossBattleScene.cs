using System;
using System.Collections.Generic;
using System.Linq;

public class BossBattleScene : GameScene
{
    private Player player;
    private Enemy boss;
    private Random rng = new Random();


    public override string SceneName => "보스 전투";

    public override GameScene? StartScene()
    {
        this.player = GameManager.player;
        this.boss = boss ?? throw new ArgumentNullException(nameof(boss));
        return prevScene;
    }

    public BossBattleScene()
    {

    }

    public BossBattleScene(Enemy boss)
    {   
        // this.boss = boss ?? throw new ArgumentNullException(nameof(boss));
    }
    
    
    public void StartBattle()
    {
        Console.Clear();

        /*
        // 보스 ASCII 아트 출력
        if (!string.IsNullOrEmpty(boss.AsciiArt))
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(boss.AsciiArt);
            Console.ResetColor();
        }
        */

        // 보스 이름 출력 박스
        UI.DrawTitledBox("보스 전투 시작!", new List<string>
        {
            $"{boss.Name} Lv.{boss.Level}"
        });

        while (player.CurrentHP > 0 && boss.CurrentHP > 0)
        {
            ShowBattleUI();

            Console.Write("\n원하시는 행동을 입력해주세요.\n>> ");
            string? input = Console.ReadLine();
            Console.Clear();

            switch (input)
            {
                case "1":
                    PlayerAttack();
                    break;

                case "2":
                    // SkillScene.Start(player, boss);
                    break;

                case "3":
                    // ItemScene.Start(player);
                    break;

                case "4":
                    GameManager.player.ShowStatus();
                    break;

                case "0":
                    Console.Write("정말 도망가시겠습니까? (Y/N): ");
                    string? confirm = Console.ReadLine()?.Trim().ToUpper();
                    if (confirm == "Y")
                    {
                        Console.WriteLine("도망쳤습니다. 보스에게서 탈출합니다!");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("도망을 취소했습니다.");
                        break;
                    }

                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    continue;
            }

            if (boss.CurrentHP <= 0)
            {
                boss.CurrentHP = 0;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\n{boss.Name}이(가) 쓰러졌습니다!");
                Console.ResetColor();
                return;
            }

            EnemyPhase();

            if (player.CurrentHP <= 0)
            {
                player.CurrentHP = 0;
                Console.WriteLine($"\n당신은 쓰러졌습니다... 게임 오버.");
                break;
            }
        }

        ShowResult();
    }

    private void PlayerAttack()
    {
        List<string> logs = new();
        logs.Add($"{player.Name}의 공격!");

        if (boss.TryDodge())
        {
            logs.Add($"{boss.Name}이(가) 공격을 회피했습니다!");
            UI.DrawBox(logs);
            return;
        }

        int baseDamage = player.BaseAttack + player.EquippedItems.Sum(i => i.Attack);
        float variance = (float)Math.Ceiling(baseDamage * 0.1f);
        int min = (int)(baseDamage - variance);
        int max = (int)(baseDamage + variance + 1);
        int finalDamage = rng.Next(min, max);

        if (player.IsCriticalHit())
        {
            logs.Add("★ 치명타! ★");
            finalDamage = (int)Math.Ceiling(finalDamage * player.CriticalDamage);
        }

        int damage = Math.Max(finalDamage - boss.Defense, 0);
        boss.CurrentHP -= damage;

        logs.Add($"{boss.Name}에게 {damage}의 피해를 입혔습니다!");
        UI.DrawBox(logs);
    }

    private void EnemyPhase()
    {
        List<string> logs = new();
        logs.Add($"{boss.Name}의 공격!");

        if (player.TryDodge())
        {
            logs.Add($"{player.Name}이(가) 공격을 회피했습니다!");
            UI.DrawBox(logs);
            return;
        }

        int bossDamage = boss.Attack;
        int defense = player.BaseDefense + player.EquippedItems.Sum(i => i.Defense);
        int damage = Math.Max(bossDamage - defense, 0);
        int beforeHP = player.CurrentHP;
        player.CurrentHP -= damage;

        logs.Add($"{player.Name}을(를) 맞췄습니다. [데미지 : {damage}]");
        logs.Add($"HP {beforeHP} → {Math.Max(player.CurrentHP, 0)}");
        UI.DrawBox(logs);
    }

    private void ShowBattleUI()
    {
        string bossHP = boss.CurrentHP > 0 ? boss.CurrentHP.ToString() : "Dead";

        List<string> info = new()
        {
            $"Lv.{boss.Level} {boss.Name}",
            $"HP: {bossHP} / {boss.MaxHP}",
            $"ATK: {boss.Attack} | DEF: {boss.Defense}"
        };

        List<string> menu = new()
        {
            "1. 공격",
            "2. 스킬 (스킬 지정 화면으로 이동)",
            "3. 아이템 사용",
            "4. 나의 현재 스탯 보기",
            "0. 도망가기"
        };

        UI.DrawTitledBox("보스 정보", info);
        UI.DrawBox(menu);
    }

    private void ShowResult()
    {
        Console.WriteLine("\n전투 결과");

        if (boss.CurrentHP <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Victory!");
            Console.ResetColor();

            player.Gold += boss.RewardGold;
            player.GainExp(boss.RewardExp);
            boss.RewardItems?.ForEach(item => player.Inventory.Add(item));

            List<string> rewardLines = new()
            {
                $"{boss.Name}을(를) 처치했습니다!",
                $"획득 골드 : {boss.RewardGold} G",
                $"획득 경험치 : {boss.RewardExp} EXP",
            };

            if (boss.RewardItems != null && boss.RewardItems.Count > 0)
            {
                rewardLines.Add("획득 아이템:");
                foreach (var item in boss.RewardItems)
                {
                    rewardLines.Add($"- {item.Name}");
                }
            }

            UI.DrawTitledBox("보상 획득", rewardLines);
        }
        else
        {
            UI.DrawTitledBox("패배", new List<string>
            {
                "You Lose...",
                $"{boss.Name}에게 패배하셨습니다."
            });
        }

        Console.WriteLine("\n0. 다음");
        Console.ReadLine();
    }
}
    