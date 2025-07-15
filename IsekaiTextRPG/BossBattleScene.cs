using System;
using System.Collections.Generic;
using System.Linq;

public class BossBattleScene
{
    private Player player;
    private Enemy boss;
    private Random rng = new Random();

    public BossBattleScene(Player player, Enemy boss)
    {
        this.player = player ?? throw new ArgumentNullException(nameof(player));
        this.boss = boss ?? throw new ArgumentNullException(nameof(boss));
    }

    public void StartBattle()
    {
        Console.Clear();
        Console.WriteLine("보스 전투 시작!");

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
                    SkillScene.Start(player, boss);
                    break;

                case "3":
                    ItemScene.Start(player);
                    break;

                case "4":
                    StatScene.Show(player);
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
        Console.WriteLine($"\n{player.Name}의 공격!");

        if (boss.TryDodge())
        {
            Console.WriteLine($"{boss.Name}이(가) 공격을 회피했습니다!");
            return;
        }

        int baseDamage = player.BaseAttack + player.EquippedItems.Sum(i => i.Attack);
        float variance = (float)Math.Ceiling(baseDamage * 0.1f);
        int min = (int)(baseDamage - variance);
        int max = (int)(baseDamage + variance + 1);
        int finalDamage = rng.Next(min, max);

        if (player.IsCriticalHit())
        {
            Console.WriteLine("★ 치명타! ★");
            finalDamage = (int)Math.Ceiling(finalDamage * player.CriticalDamage);
        }

        int damage = Math.Max(finalDamage - boss.Defense, 0);
        boss.CurrentHP -= damage;

        Console.WriteLine($"{boss.Name}에게 {damage}의 피해를 입혔습니다!");
    }

    private void EnemyPhase()
    {
        Console.WriteLine($"\n{boss.Name}의 공격!");

        if (player.TryDodge())
        {
            Console.WriteLine($"{player.Name}이(가) 공격을 회피했습니다!");
            return;
        }

        int bossDamage = boss.Attack;
        int defense = player.BaseDefense + player.EquippedItems.Sum(i => i.Defense);
        int damage = Math.Max(bossDamage - defense, 0);
        int beforeHP = player.CurrentHP;
        player.CurrentHP -= damage;

        Console.WriteLine($"{player.Name}을(를) 맞췄습니다. [데미지 : {damage}]");
        Console.WriteLine($"Lv.{player.Level} {player.Name}");
        Console.WriteLine($"HP {beforeHP} -> {Math.Max(player.CurrentHP, 0)}");
    }

    private void ShowBattleUI()
    {
        string bossHP = boss.CurrentHP > 0 ? boss.CurrentHP.ToString() : "Dead";
        Console.WriteLine($"Lv.{boss.Level} {boss.Name} HP: {bossHP} | 공격력: {boss.Attack} | 방어력: {boss.Defense}");
        Console.WriteLine("\n1. 공격\n2. 스킬 (스킬 지정화면으로감)\n3. 아이템 사용하기\n4. 나의 현재 스탯 보기\n0. 도망가기");
    }

    private void ShowResult()
    {
        Console.WriteLine("\n전투 결과");

        if (boss.CurrentHP <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Victory!");
            Console.ResetColor();

            Console.WriteLine($"{boss.Name}을(를) 처치했습니다!");

            Console.WriteLine($"\n[캐릭터 정보]");
            Console.WriteLine($"Lv.{player.Level} {player.Name}");
            Console.WriteLine($"HP {player.MaxHP} -> {player.CurrentHP}");

            Console.WriteLine("\n[획득 보상]");
            Console.WriteLine($"{boss.RewardGold} Gold");
            Console.WriteLine($"{boss.RewardExp} EXP");

            player.Gold += boss.RewardGold;
            player.GainExp(boss.RewardExp);

            foreach (var item in boss.RewardItems)
            {
                if (item != null)
                {
                    player.Inventory.Add(item);
                    Console.WriteLine($"{item.Name}");
                }
            }
        }
        else
        {
            Console.WriteLine("You Lose...");
            Console.WriteLine($"{boss.Name}에게 패배하셨습니다.");
        }

        Console.WriteLine("\n0. 다음");
        Console.ReadLine();
    }
}