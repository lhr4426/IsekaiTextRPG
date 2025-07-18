using System;
using System.Collections.Generic;
using System.Linq;

public class NormalBattleScene : BattleBase
{
    public override string SceneName => "일반 전투";

    public override GameScene? StartScene()
    {
        Console.Clear();
        Console.WriteLine("전투를 마주쳤습니다!");
        CooldownSetting();
        BattleLogger.Init();
        BattleLogger.Log("전투 시작");

        List<Enemy> enemies = Enemy.GenerateEnemies();
        Player player = GameManager.player;

        while (player.CurrentHP > 0 && enemies.Any(e => e.CurrentHP > 0))
        {
            TickCooldowns();
            Console.Clear();

            bool continueBattle = PlayerPhase(player, enemies, isBossBattle: false, isHidden: false);
            if (!continueBattle) return prevScene;

            foreach (var enemy in enemies.Where(e => e.CurrentHP > 0))
            {
                EnemyAttack(enemy, player);
            }

            Console.ReadKey();
        }

        Console.Clear();

        if (player.CurrentHP <= 0)
        {
            player.CurrentHP = 1;
            DefeatMsg();
            return SceneManager.Instance.scenes[SceneManager.SceneType.TownScene];
        }
        else
        {
            DrawBattleResult(enemies, player);
        }

        Console.WriteLine("\n0. 다음");
        Console.ReadLine();
        return prevScene;
    }
}
