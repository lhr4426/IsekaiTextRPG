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
        this.boss = boss ?? new Enemy(1, "test", 9999, 5, 5, 0, 100, 20); // test;
        StartBattle();
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
            

            if (!PlayerPhase()) return;

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
        return;
    }


    /// <summary>
    /// 사용자가 도망가면 return false, 도망가지 않으면 true
    /// </summary>
    /// <returns></returns>
    private bool PlayerPhase()
    {
        bool doSomething = false;
        while(!doSomething)
        {
            Console.Clear();
            ShowBattleUI();
            Console.Write("\n원하시는 행동을 입력해주세요.\n>> ");
            int? input = InputHelper.InputNumber(0, 4);
            
            switch (input)
            {
                case 1:
                    PlayerAttack(null);
                    doSomething = true;
                    break;

                case 2:
                    SkillInvenScene skillScene = (SkillInvenScene)SceneManager.Instance.scenes[SceneManager.SceneType.SkillInvenScene];
                    skillScene.PrintSkills();
                    if (UseSkill()) doSomething = true;
                    break;

                case 3:
                    // TODO : 아이템 사용 필요
                    InventoryScene invenScene = (InventoryScene)SceneManager.Instance.scenes[SceneManager.SceneType.InvenScene];
                    invenScene.PrintUsableItems(out List<Item> inven);
                    if (UseItem(inven)) doSomething = true;
                    break;

                case 4:
                    GameManager.player.ShowStatus();
                    Console.ReadKey();
                    break;

                case 0:
                    Console.Write("정말 도망가시겠습니까? (Y/N): ");
                    string? confirm = Console.ReadLine()?.Trim().ToUpper();
                    if (confirm == "Y")
                    {
                        Console.WriteLine("도망쳤습니다. 보스에게서 탈출합니다!");
                        return false;
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
        }
        Console.ReadKey();
        return true;
    }



    /// <summary>
    /// 스킬 사용 성공했으면 true,
    /// 아니면 false
    /// </summary>
    /// <returns></returns>
    private bool UseSkill()
    {
        List<Skill> skills = GameManager.player.Skills;
        int? input = InputHelper.InputNumber(0, skills.Count);
        bool isUsed = false;
        if (input == null) return false;
        if (input == 0) return false;
        if (input > 0 && input < skills.Count)
        {
            PlayerAttack(skills[(int)input - 1]);
            isUsed = true;
        }
        return false;
    }

    /// <summary>
    /// 아이템 사용 성공했으면 true,
    /// 아니면 false
    /// </summary>
    /// <param name="inven"></param>
    /// <returns></returns>
    private bool UseItem(List<Item> inven)
    {
        int? input = InputHelper.InputNumber(0, inven.Count);
        bool isUsed = false;
        if (input == null) return false;
        if (input == 0) return false;
        if (input > 0 && input <= inven.Count)
        {
            

            List<string> strings = new();
            if (inven[(int)input - 1].Hp > 0)
            {
                int healHp = inven[(int)input - 1].Hp;
                Player player = GameManager.player;
                healHp = player.CurrentHP + healHp > player.MaxHP ?
                    player.MaxHP - player.CurrentHP :
                    healHp;

                strings.Add($"체력이 {healHp} 회복되었습니다.");
                GameManager.player.CurrentHP += healHp;
                isUsed = true;
            }

            if (inven[(int)input - 1].Mp > 0)
            {
                int healMp = inven[(int)input - 1].Mp;
                Player player = GameManager.player;
                healMp = player.CurrentHP + healMp > player.MaxHP ?
                    player.MaxHP - player.CurrentHP :
                    healMp;

                strings.Add($"마나가 {healMp} 회복되었습니다.");
                GameManager.player.CurrentMP += healMp;
                isUsed = true;
            }

            UI.DrawBox(strings);
        }
        return isUsed;
    }
     


    private void PlayerAttack(Skill? skill)
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
        if (skill != null) baseDamage += skill.Damage;
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
        Console.ReadKey();
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
    