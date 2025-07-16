using System;
using System.Collections.Generic;
using System.Linq;

public class NormalBattleScene : GameScene
{
    public override string SceneName => "일반 전투";

    public override GameScene? StartScene()
    {
        Console.Clear();
        Console.WriteLine("전투를 마주쳤습니다!");

        BattleLogger.Init();
        BattleLogger.Log("전투 시작");

        List<Enemy> enemies = Enemy.GenerateEnemies();
        Player player = GameManager.player;

        while (player.CurrentHP > 0 && enemies.Any(e => e.CurrentHP > 0))
        {
            Console.Clear();

            bool continueBattle = BattleBase.PlayerPhase(player, enemies, isBossBattle: false);
            if (!continueBattle) return prevScene;

            foreach (var enemy in enemies.Where(e => e.CurrentHP > 0))
            {
                BattleBase.EnemyAttack(enemy, player);
            }

            Console.ReadKey();
        }

        Console.Clear();

        if (player.CurrentHP <= 0)
        {
            BattleLogger.Log("당신은 쓰러졌습니다... 게임 오버.");
            Console.WriteLine("당신은 쓰러졌습니다... 게임 오버.");
        }
        else
        {
            BattleBase.DrawBattleResult(enemies, player);
        }

        Console.WriteLine("\n0. 다음");
        Console.ReadLine();
        return prevScene;
    }
}
