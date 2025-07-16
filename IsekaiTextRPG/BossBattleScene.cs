using System;
using System.Collections.Generic;

public class BossBattleScene : GameScene
{
    private Player player;
    private Enemy boss;

    public override string SceneName => "보스 전투";

    public BossBattleScene() { }

    public override GameScene? StartScene()
    {
        Console.Clear();
        player = GameManager.player;
        boss ??= new Enemy(1, "임시 Boss", 10, 10, 5, 0, 10, 50);

        if (boss.CurrentHP <= 0)
        {
            boss.CurrentHP = boss.MaxHP;
        }
        BattleLogger.Init();
        BattleLogger.Log("보스 전투 시작");

        UI.DrawTitledBox("보스 전투 시작!", new List<string> { $"{boss.Name} Lv.{boss.Level}" });

        while (player.CurrentHP > 0 && boss.CurrentHP > 0)
        {
            bool continueBattle = BattleBase.PlayerPhase(player, new List<Enemy> { boss }, isBossBattle: true);
            if (!continueBattle) return prevScene;
            if (boss.CurrentHP <= 0) break;

            BattleBase.EnemyAttack(boss, player);
            Console.ReadKey();

            if (player.CurrentHP <= 0)
            {
                BattleLogger.Log($"{player.Name}은(는) 쓰러졌습니다... 게임 오버.");
                break;
            }
        }

        BattleBase.DrawBattleResult(boss, player);
        Console.WriteLine("\n0. 다음");
        Console.ReadLine();
        return prevScene;
    }
}
