using IsekaiTextRPG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class BossDungeonScene : GameScene
{
    public override string SceneName => "보스던전 입구";

    // TODO : 던전 내부 씬 만들어지면 그거 연결하고 EndScene으로 변경해야함
    public override GameScene? StartScene()
    {
        Console.Clear();
        return HandleBossMenu();
    }

    private GameScene? HandleBossMenu() // 보스 던전 메뉴 처리
    {
        List<string> contents = new()
            {
                "보스 던전에서는 강력한 적이 등장합니다.",
                "",
                "1. 핑크빈 (난이도: 하)",
                "2. 쿠크세이튼 (난이도: 중)",
                "3. 안톤 (난이도: 상)",
                "?. ??? (난이도 : 최상)",
                "0. 던전 입구로 돌아가기"
            };

        UI.DrawTitledBox(SceneName, contents);
        Console.Write(">> ");
        int? input = InputHelper.InputNumber(0, 7);// 사용자 입력을 받아 숫자로 변환 (0 ~ 3 범위)

        switch (input)
        {
            case 1:
                return new BossBattleScene(BossClass.GetBossList()[0]);
            case 2:
                return new BossBattleScene(BossClass.GetBossList()[1]);
            case 3:
                return new BossBattleScene(BossClass.GetBossList()[2]);
            case 7:
                return new BossBattleScene(BossClass.GetBossList()[3]);
            case 0:
                return prevScene;
            default:
                Console.WriteLine("잘못된 입력입니다.");
                Console.ReadKey();
                return this;
        }
    }
}

