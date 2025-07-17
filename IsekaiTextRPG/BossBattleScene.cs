using IsekaiTextRPG;
using System;
using System.Collections.Generic;

public class BossBattleScene : BattleBase
{
    private Player player;
    private Enemy boss;

    public override string SceneName => "보스 전투";

    public BossBattleScene() { } 
    public BossBattleScene(Enemy boss)// 생성자에서 보스 객체를 받음
    {
        this.boss = boss;
    }
    public override GameScene? StartScene()
    {
        Console.Clear();
        player = GameManager.player;

        if (boss.CurrentHP <= 0)
        {
            boss.CurrentHP = boss.MaxHP;
        }
        BattleLogger.Init();
        BattleLogger.Log("보스 전투 시작");

        UI.DrawTitledBox("보스 전투 시작!", new List<string> { $"{boss.Name} Lv.{boss.Level}" });

        while (player.CurrentHP > 0 && boss.CurrentHP > 0)
        {
            bool continueBattle = this.PlayerPhase(player, new List<Enemy> { boss }, isBossBattle: true);
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
        }

        DrawBattleResult(boss, player);
        Console.WriteLine("\n0. 다음");
        Console.ReadLine();
        return prevScene;
    }

    private void BossAttackPhase() 
    {
        if (boss is BossClass.Boss bossEnemy)
        {
            int damage = bossEnemy.PerformAttack(player);

            
            List<string> logs = new()
        {
            $"{boss.Name}의 공격!",
            $"{player.Name}이(가) {damage}의 피해를 입었습니다!"
        };
            UI.DrawBox(logs);              
            logs.ForEach(BattleLogger.Log); 
        
        }
        else
        {
            EnemyAttack(boss, player); 
        }
    }
}
