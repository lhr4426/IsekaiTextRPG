using System;
using System.Collections.Generic;
using System.Linq;
using static IsekaiTextRPG.BossClass;

public class BattleBase : GameScene
{
    public static List<Player> heros = new List<Player>();

    private Random rng = new Random();

    public Dictionary<Skill, int> playerCooldowns = new Dictionary<Skill, int>();

    public override string SceneName => throw new NotImplementedException();

    public void CooldownSetting()
    {
        foreach (var skill in GameManager.player.Skills)
        {
            playerCooldowns[skill] = 0;
        }
    }

    public void TickCooldowns()
    {
        foreach (var skill in playerCooldowns)
        {
            if (playerCooldowns[skill.Key] > 0)
            {
                playerCooldowns[skill.Key]--;
            }
        }
    }


    public void PlayerAttack(Player player, Enemy target, Skill? skill = null)
    {
        List<string> logs = new() { $"{player.Name}의 공격!" };

        if (target.TryDodge())
        {
            logs.Add($"{target.Name}이(가) 공격을 회피했습니다!");
            UI.DrawBox(logs);
            BattleLogger.Log(logs.Last());
            return;
        }

        int baseDamage = player.BaseAttack + player.EquippedItems.Sum(i => i.Attack);
        if (skill != null)
        {
            baseDamage += skill.Damage;
            player.CurrentMP -= skill.ManaCost;
        }

        int finalDamage = rng.Next((int)(baseDamage * 0.9f), (int)(baseDamage * 1.1f) + 1);

        if (player.IsCriticalHit())
        {
            logs.Add("★ 치명타! ★");
            finalDamage = (int)(finalDamage * player.CriticalDamage);
        }

        if (heros.Count > 0)
        {
            foreach (var hero in heros)
            {
                logs.Add($"{hero.Name}의 합공! (추가 공격력 : {hero.TotalAttack})");
                finalDamage += hero.TotalAttack;
            }
        }

        int damage = Math.Max(finalDamage - target.Defense, 0);
        target.CurrentHP -= damage;
        logs.Add($"{target.Name}에게 {damage}의 피해를 입혔습니다!");
        if (target.CurrentHP <= 0)
        {
            logs.Add($"{target.Name}이(가) 쓰러졌습니다!");
            QuestManager.UpdateKillQuestProgress(target.Name);
        }

        UI.DrawBox(logs);
        logs.ForEach(BattleLogger.Log);
    }

    public void EnemyAttack(Enemy enemy, Player player)
    {
        List<string> logs = new() { $"{enemy.Name}의 공격!" };

        if (player.TryDodge())
        {
            logs.Add($"{player.Name}이(가) 공격을 회피했습니다!");
            UI.DrawBox(logs);
            BattleLogger.Log(logs.Last());
            return;
        }

        int damage = Math.Max(enemy.Attack - (player.BaseDefense + player.EquippedItems.Sum(i => i.Defense)), 0);
        int beforeHP = player.CurrentHP;
        player.CurrentHP -= damage;
        if (player.CurrentHP < 0)
        {
            player.CurrentHP = 0;
        }
        logs.Add($"{player.Name}을(를) 맞췄습니다. [데미지 : {damage}]");
        logs.Add($"HP {beforeHP} → {Math.Max(player.CurrentHP, 0)}");

        UI.DrawBox(logs);
        logs.ForEach(BattleLogger.Log);
    }

    public bool UseItem(List<Item> inven, Player player)
    {
        Console.Write("사용할 아이템의 번호를 선택하세요 ( 0: 취소 ) >> ");
        int? input = InputHelper.InputNumber(0, inven.Count);
        if (input == null || input == 0 || input > inven.Count) return false;

        Item selected = inven[(int)input - 1];
        List<string> logs = new();
        bool used = false;

        if (selected.Hp > 0)
        {
            int healHp = Math.Min(selected.Hp, player.MaxHP - player.CurrentHP);
            player.CurrentHP += healHp;
            logs.Add($"체력이 {healHp} 회복되었습니다.");
            used = true;
        }

        if (selected.Mp > 0)
        {
            int healMp = Math.Min(selected.Mp, player.MaxMP - player.CurrentMP);
            player.CurrentMP += healMp;
            logs.Add($"마나가 {healMp} 회복되었습니다.");
            used = true;
        }

        if (used && selected.Type == Item.ItemType.Usable)
        {
            selected.ItemCount--;

            if (selected.ItemCount <= 0)
            {
                player.Inventory.Remove(selected);
                logs.Add($"{selected.Name}을(를) 모두 사용했습니다.");
            }
            else
            {
                logs.Add($"{selected.Name}을(를) 사용했습니다. 남은 개수: {selected.ItemCount}개.");
            }
        }

        UI.DrawBox(logs);
        logs.ForEach(BattleLogger.Log);
        return used;
    }

    public static void DrawBossLast(string bossName)
    {
        List<string> talk;
        ConsoleColor color = ConsoleColor.Yellow;

        switch (bossName)
        {
            case "핑크빈":
                talk = new List<string>
                {
                    "삐요옹... 나 진짜 진심 아니었는데... 다음엔 안 져줄 거야!"
                };
                break;
            case "쿠크세이튼":
                talk = new List<string>
                {
                    "좋아. 오늘은 여기까지! 계속 분발해! 자만하지 말고! 우리가 널 응원하고 있으니까 말이야.",
                    "그럼 혼돈이 가득 차오를 때 다시 보자고!"
                };
                break;
            case "안톤":
                talk = new List<string>
                {
                    "이대로... 사라지진 않아...",
                    "지옥의 화염은... 언젠가 다시 타오를 것이다..."
                };
                break;
            case "최강 7조":
                talk = new List<string>
                {
                    "지금까지 이세계 RPG를 플레이 해 주셔서 감사합니다!",
                    "더이상 이 세계는 이세계가 아닌, 당신의 세계입니다!"
                };
                break;

            default:
                talk = new List<string> { "(...)" };
                break;
        }

        UI.DrawBox(talk);
        Console.WriteLine("보상을 받으려면 아무키나 입력...");
        Console.ReadKey();
        Console.ForegroundColor = color;
    }

    public void DrawBattleResult(Enemy boss, Player player)
    {
        if (boss.CurrentHP <= 0)
        {
            Console.Clear();
            DrawBossLast(boss.Name);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Victory!");
            Console.ResetColor();
            QuestManager.UpdateKillQuestProgress(boss.Name);

            int previousLevel = player.Level;

            player.Gold += boss.RewardGold;
            player.GainExp(boss.RewardExp);
            boss.RewardItems?.ForEach(item => player.Inventory.Add(item));

            List<string> rewardLines = new()
            {
                $"{boss.Name}을(를) 처치했습니다!",
                $"획득 골드 : {boss.RewardGold} G",
                $"획득 경험치 : {boss.RewardExp} EXP"
            };

            if (player.Level > previousLevel)
            {
                rewardLines.Add($"[레벨 업!] {player.Level} 레벨이 되었습니다!");
            }

            rewardLines.Add("");
            rewardLines.Add($"레벨: {player.Level}");
            rewardLines.Add($"경험치: {player.Exp} / {player.MaxExp}");
            rewardLines.Add("");


            BattleLogger.Log(rewardLines[0]);
            BattleLogger.Log(rewardLines[1]);
            BattleLogger.Log(rewardLines[2]);


            if (boss.RewardItems is { Count: > 0 })
            {
                rewardLines.Add("획득 아이템:");
                foreach (var item in boss.RewardItems)
                {
                    rewardLines.Add($"- {item.Name}");
                    BattleLogger.Log($"아이템 획득: {item.Name}");
                }
            }

            UI.DrawTitledBox("보상 획득", rewardLines);
            Console.WriteLine("0. 다음");
            Console.ReadKey();
            BattleLogger.CloseLogger();
        }
    }

    public void DrawBattleResult(List<Enemy> enemies, Player player)
    {
        Console.Clear();
        bool allDefeated = enemies.All(e => e.CurrentHP <= 0);

        if (allDefeated)
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

            int previousLevel = player.Level;

            player.Gold += totalGold;
            player.GainExp(totalExp);
            totalItems.ForEach(i => player.Inventory.Add(i));

            List<string> rewards = new()
            {
                "모든 적을 처치했습니다!",
                $"획득 골드 : {totalGold} G",
                $"획득 경험치 : {totalExp} EXP"
            };

            if (player.Level > previousLevel)
            {
                rewards.Add($"[레벨 업!] {player.Level} 레벨이 되었습니다!");
            }

            rewards.Add("");
            rewards.Add($"레벨: {player.Level}");
            rewards.Add($"경험치: {player.Exp} / {player.MaxExp}");
            rewards.Add("");

            BattleLogger.Log(rewards[0]);
            BattleLogger.Log(rewards[1]);
            BattleLogger.Log(rewards[2]);

            if (totalItems.Count > 0)
            {
                rewards.Add("획득 아이템:");
                foreach (var item in totalItems)
                {
                    rewards.Add($"- {item.Name}");
                    BattleLogger.Log($"아이템 획득: {item.Name}");
                }
            }

            UI.DrawTitledBox("보상 획득", rewards);
            Console.WriteLine("0. 다음");
            Console.ReadKey();
            BattleLogger.CloseLogger();
        }
    }

    public void DefeatMsg(string name = "적")
    {
        BattleLogger.Log($"{name}에게 패배하였습니다.");
        UI.DrawTitledBox("패배", new List<string>
        {
            "You Lose...",
            $"{name}에게 패배하셨습니다."
        });
        Console.ReadKey();
        BattleLogger.CloseLogger();
    }

    public void ShowNormalBattleUI(Player player, List<Enemy> enemies)
    {
        int bonusAtk = player.EquippedItems.Where(i => i.IsEquip && i.Attack != 0).Sum(i => i.Attack);
        int bonusDef = player.EquippedItems.Where(i => i.IsEquip && i.Defense != 0).Sum(i => i.Defense);
        int bonusHp = player.EquippedItems.Where(i => i.IsEquip && i.Hp != 0).Sum(i => i.Hp);
        int bonusMp = player.EquippedItems.Where(i => i.IsEquip && i.Mp != 0).Sum(i => i.Mp);

        List<string> info = new()
        {
            $"Lv.{player.Level} {player.Name} ({Player.JobsKorean(player.Job)})",
            $"HP: {player.CurrentHP} / {player.MaxHP + bonusHp} | MP: {player.CurrentMP} / {player.MaxMP + bonusMp}",
            $"ATK: {player.BaseAttack + bonusAtk} | DEF: {player.BaseDefense + bonusDef}"
        };

        List<string> menu = new()
        {
            "1. 공격 ",
            "2. 스킬 ",
            "3. 아이템 사용",
            "4. 나의 현재 스탯 보기",
            "0. 도망가기"
        };
        for (int i = 0; i < enemies.Count; i++)
        {
            Console.Write($"{i + 1}. ");
            enemies[i].showEnemyInfo();
        }
        UI.DrawBox(menu);
        UI.DrawTitledBox("플레이어 정보", info);
    }

    public void SummonHeros()
    {
        List<int> slots = new List<int>() { 1, 2, 3, 4 };
        foreach (int slot in slots)
        {
            if (slot == GameManager.instance.selectedSlot) continue;
            Player hero = GameManager.instance.GetPlayerData(slot);
            if (hero == null) continue;
            heros.Add(hero);
        }
    }

    public bool PlayerPhase(Player player, List<Enemy> enemies, bool isBossBattle, bool isHidden)
    {
        while (true)
        {
            // Console.Clear();
            if (isBossBattle && enemies.Count == 1)
            {
                // ShowBossBattleUI(player,enemies[0]);
            }

            else
            {
                ShowNormalBattleUI(player, enemies);
            }

            Console.Write("\n원하시는 행동을 입력해주세요.\n>> ");
            int? input;
            if (isHidden) input = InputHelper.InputNumber(0, 5);
            else input = InputHelper.InputNumber(0, 4);


            switch (input)
            {
                case 1:
                    if (enemies.Count == 1)
                    {
                        Enemy boss = enemies[0];
                        BattleLogger.Log($"{player.Name}이(가) 일반 공격을 선택했습니다.");
                        PlayerAttack(player, boss);
                        return true;
                    }
                    else
                    {
                        List<string> strings = new();

                        List<Enemy> alive = enemies.Where(e => e.CurrentHP > 0).ToList();
                        for (int i = 0; i < alive.Count; i++)
                        {
                            strings.Add($"{i + 1}. {alive[i].Name} (HP: {alive[i].CurrentHP}/{alive[i].MaxHP})");
                        }

                        UI.DrawLeftAlignedBox(strings);
                        Console.Write("공격할 대상의 번호를 선택하세요 (0 : 취소) >> ");

                        int? targetInput = InputHelper.InputNumber(0, alive.Count);
                        if (targetInput == null || targetInput == 0) break;

                        Enemy target = alive[(int)targetInput - 1];
                        BattleLogger.Log($"{player.Name}이(가) {target.Name}을(를) 공격합니다.");
                        PlayerAttack(player, target);
                        return true;
                    }

                case 2:
                    SkillInvenScene skillScene = (SkillInvenScene)SceneManager.Instance.scenes[SceneManager.SceneType.SkillInvenScene];
                    skillScene.PrintSkills();

                    List<Skill> skills = player.Skills;
                    Skill? selectedSkill = null;

                    Console.Write("사용할 스킬의 번호를 입력하세요 (0 : 취소) >> ");

                    int? skillInput = InputHelper.InputNumber(0, skills.Count);
                    if (skillInput == null || skillInput == 0 || skillInput > skills.Count) break;

                    selectedSkill = skills[(int)skillInput - 1];

                    if (selectedSkill.ManaCost > player.CurrentMP)
                    {
                        UI.DrawBox(new List<string> { "스킬을 사용하기 위한 마나가 부족합니다." });
                        Console.ReadKey();
                        break;
                    }

                    if (playerCooldowns[selectedSkill] != 0)
                    {
                        UI.DrawBox(new List<string> { $"{selectedSkill.Name} 스킬은 {playerCooldowns[selectedSkill]} 턴 이후에 사용 가능합니다." });
                        Console.ReadKey();
                        break;
                    }
                    else
                    {
                        playerCooldowns[selectedSkill] = selectedSkill.Cooldown;
                    }

                    if (enemies.Count == 1)
                    {
                        Enemy boss = enemies[0];
                        BattleLogger.Log($"{player.Name}이(가) 스킬 [{selectedSkill.Name}] 사용을 시도합니다.");
                        PlayerAttack(player, boss, selectedSkill);
                    }
                    else
                    {
                        List<string> strings = new();
                        List<Enemy> alive = enemies.Where(e => e.CurrentHP > 0).ToList();
                        for (int i = 0; i < alive.Count; i++)
                        {
                            strings.Add($"{i + 1}. {alive[i].Name} (HP: {alive[i].CurrentHP}/{alive[i].MaxHP})");
                        }

                        UI.DrawLeftAlignedBox(strings);

                        Console.Write("스킬 사용할 대상의 번호를 선택하세요 (0 : 취소) >> ");
                        int? targetInput = InputHelper.InputNumber(0, alive.Count);

                        if (targetInput == null || targetInput == 0) break;

                        Enemy target = alive[(int)targetInput - 1];
                        BattleLogger.Log($"{player.Name}이(가) 스킬 [{selectedSkill.Name}] 사용을 시도합니다.");
                        PlayerAttack(player, target, selectedSkill);
                    }

                    return true;

                case 3:
                    InventoryScene invenScene = (InventoryScene)SceneManager.Instance.scenes[SceneManager.SceneType.InvenScene];
                    invenScene.PrintUsableItems(out List<Item> inven);
                    if (UseItem(inven, player))
                    {
                        BattleLogger.Log($"{player.Name}이(가) 아이템을 사용했습니다.");
                        return true;
                    }
                    break;

                case 4:
                    player.ShowStatus();
                    Console.WriteLine("아무 키나 입력하여 돌아가기");
                    Console.ReadKey();
                    break;

                case 5:
                    if (!isHidden) break;
                    else
                    {
                        Console.WriteLine("당신은 혼자가 아님을 직감했습니다. 누군가의 도움을 간절히 빌어봅니다.");
                        SummonHeros();
                        return true;
                    }

                case 0:
                    Console.Write("정말 도망가시겠습니까? (Y/N): ");
                    string? confirm = Console.ReadLine()?.Trim().ToUpper();
                    if (confirm == "Y")
                    {
                        BattleLogger.Log($"{player.Name}이(가) 도망쳤습니다.");
                        Console.WriteLine("당신은 도망쳤습니다.");
                        BattleLogger.CloseLogger();
                        return false;
                    }
                    else
                    {
                        Console.WriteLine("도망을 취소했습니다.");
                        Console.ReadKey();
                    }
                    break;

                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    break;
            }
        }
    }

    public override GameScene? StartScene()
    {
        throw new NotImplementedException();
    }
}