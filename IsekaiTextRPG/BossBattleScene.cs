using IsekaiTextRPG;
using System;
using System.Collections.Generic;

public class BossBattleScene : BattleBase
{
    private Player player;
    private Enemy boss;

    public override string SceneName => "보스 전투";

    public BossBattleScene() { }

    public BossBattleScene(Enemy boss)
    {
        this.boss = boss;
    }

    public override GameScene? StartScene()
    {
        heros.Clear();
        prevScene = SceneManager.Instance.scenes[SceneManager.SceneType.BossDungeonScene];
        CooldownSetting();

        Console.Clear();
        player = GameManager.player;

        if (boss.CurrentHP <= 0)
            boss.CurrentHP = boss.MaxHP;

        BattleLogger.Init();
        BattleLogger.Log("보스 전투 시작");

        bool isHidden = boss.Name == "최강 7조";

        if (boss is BossClass.Boss asciiBoss && !string.IsNullOrEmpty(asciiBoss.AsciiArt))
        {
            DrawBossIntro(asciiBoss.Name);
            ShowAsciiWithCombatUI(asciiBoss, player);
        }

        while (player.CurrentHP > 0 && boss.CurrentHP > 0)
        {
            TickCooldowns();
            bool continueBattle = PlayerPhase(player, new List<Enemy> { boss }, isBossBattle: true, isHidden);
            if (!continueBattle) return prevScene;
            if (boss.CurrentHP <= 0) break;

            BossAttackPhase();
            Console.ReadKey();

            if (player.CurrentHP <= 0)
            {
                Console.Clear();
                player.CurrentHP = 1;
                DefeatMsg();
                return SceneManager.Instance.scenes[SceneManager.SceneType.TownScene];
            }

            if (boss is BossClass.Boss redrawBoss && !string.IsNullOrEmpty(redrawBoss.AsciiArt))
            {
                ShowAsciiWithCombatUI(redrawBoss, player);
            }
        }

        DrawBattleResult(boss, player);
        return prevScene;
    }

    private void BossAttackPhase()
    {
        if (boss is BossClass.Boss bossEnemy)
        {
            int damage = bossEnemy.PerformAttack(player);
            string? skillName = bossEnemy.LastUsedSkillName;

            var logs = new List<string>
            {
                $"{boss.Name}의 공격!",
                skillName != null ? $"{boss.Name}이(가) 스킬 [{skillName}]을(를) 사용했다!" : null,
                $"{player.Name}이(가) {damage}의 피해를 입었습니다!"
            };
            logs.RemoveAll(s => s == null);
            UI.DrawBox(logs);
            logs.ForEach(BattleLogger.Log);
        }
        else
        {
            EnemyAttack(boss, player);
        }
    }
    public void DrawBossIntro(string bossName)
    {
        List<string> talk;
        ConsoleColor color;

        switch (bossName)
        {
            case "핑크빈":
                talk = new List<string>
                {
                    "(핑크빈이 포효한다!)",
                    "삐요옹~ 나 화났어! 이 세계는 이제 내 거야!"
                };
                break;

            case "쿠크세이튼":
                talk = new List<string>
                {
                    "(쿠크세이튼이 비웃는다...)",
                    "세이튼 : 여러분, 혜성처럼 등장한 머저리들을 소개하겠습니다!",
                    "쿠크 : 판이 깔렸으니 신나게 놀아보자고!"
                };
                break;

            case "안톤":
                talk = new List<string>
                {
                    "(불타는 심연 속에서 안톤이 눈을 뜬다)",
                    "지옥의 화염이... 너를 삼킬 것이다!"
                };
                break;
            case "최강 7조":
                talk = new List<string>
                {
                    "(알 수 없는 4인조가 서있다.)",
                    "후후... 여기까지 오셨군요. 과연 저희를 이길 수 있을까요?"
                };
                break;

            default:
                talk = new List<string> { "(정체불명의 존재가 등장했다...)" };
                break;
        }

        UI.DrawBox(talk);
        Console.ReadKey();
        Console.ForegroundColor = GetBossColor(bossName);
    }
    public static ConsoleColor GetBossColor(string bossName)
    {
        return bossName switch
        {
            "핑크빈" => ConsoleColor.Magenta,
            "쿠크세이튼" => ConsoleColor.Yellow,
            "안톤" => ConsoleColor.Red,
            "최강 7조" => ConsoleColor.White,
            _ => ConsoleColor.Gray
        };
    }
    public static void ShowAsciiWithCombatUI(BossClass.Boss asciiBoss, Player player)
    {
        Console.Clear();
        
        string[] asciiLines = asciiBoss.AsciiArt.Split('\n');
        int asciiHeight = asciiLines.Length;
        int rightStartX = 70;
        int currentY = 0;
        bool isHidden = asciiBoss.Name == "최강 7조";
        
        Console.ForegroundColor = GetBossColor(asciiBoss.Name);
        for (int i = 0; i < asciiHeight; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.WriteLine(asciiLines[i]);
        }
        Console.ResetColor();

        var bossInfo = new List<string>
        {
            $"Lv.{asciiBoss.Level} {asciiBoss.Name}",
            $"HP: {asciiBoss.CurrentHP} / {asciiBoss.MaxHP}",
            $"ATK: {asciiBoss.Attack} | DEF: {asciiBoss.Defense}"
        };

        var menu = new List<string>
        {
            "1. 공격",
            "2. 스킬",
            "3. 아이템 사용",
            "4. 나의 현재 스탯 보기"
        };

        if (isHidden) menu.Add("5. 누군가를 부르기?");
        menu.Add("0. 도망가기");

        var bonusAtk = player.EquippedItems.Where(i => i.IsEquip).Sum(i => i.Attack);
        var bonusDef = player.EquippedItems.Where(i => i.IsEquip).Sum(i => i.Defense);
        var bonusHp = player.EquippedItems.Where(i => i.IsEquip).Sum(i => i.Hp);
        var bonusMp = player.EquippedItems.Where(i => i.IsEquip).Sum(i => i.Mp);

        var playerInfo = new List<string>
        {
            $"Lv.{player.Level} {player.Name} ({Player.JobsKorean(player.Job)})",
            $"HP: {player.CurrentHP} / {player.MaxHP + bonusHp}",
            $"MP: {player.CurrentMP} / {player.MaxMP + bonusMp}",
            $"ATK: {player.BaseAttack + bonusAtk} | DEF: {player.BaseDefense + bonusDef}"
        };

        UI.DrawTitledBoxAt("보스 정보", bossInfo, rightStartX, currentY);
        currentY += 6 + bossInfo.Count;
        UI.DrawBoxAt(menu, rightStartX, currentY);
        currentY += 4 + menu.Count;

        if (isHidden)
        {
            List<string> herosString = new();
            if (heros.Count > 0)
            {
                foreach (var hero in heros)
                {
                    herosString.Add(hero.ToHeroString());
                }
                UI.DrawBoxAt(herosString, rightStartX, currentY);
            }
            currentY += 4 + herosString.Count;
        }
        
        UI.DrawTitledBoxAt("플레이어 정보", playerInfo, rightStartX, currentY);
        Console.SetCursorPosition(0, asciiHeight);
    }

}
