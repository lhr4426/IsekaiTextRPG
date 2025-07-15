using System;
using System.Collections.Generic;

public class LevelDungeonScene : GameScene
{
    public override string SceneName => "레벨던전 화면";

    public override GameScene? StartScene()
    {
        Console.Clear();
        Console.WriteLine("레벨업 던전\n");
        Console.WriteLine("이곳에서 당신은 실력을 쌓고 경험치를 얻을 수 있습니다.");
        Console.WriteLine("보스 소탕 전 몬스터를 처치하며 전투 감각을 익히세요.\n");
        Console.WriteLine("1. 입장 <전투 씬으로 이동 후 잡몹들 등장>");
        Console.WriteLine("0. 나가기 <던전 입구 창으로 이동>\n");
        Console.WriteLine("원하시는 행동을 입력해주세요.");

        int? index = InputHelper.InputNumber(0, 1);

         switch (index)
         {
             case 0:
                return prevScene;                  
             case 1:
                Console.WriteLine("\n[전투 던전에 입장합니다!]");
                Console.ReadKey();

                if (nextScenes.Count > 0)
                    return nextScenes[0];
                else
                {
                    Console.WriteLine("전투 씬이 연결되어 있지 않습니다.");
                    Console.ReadKey();
                    return this;
                }
            default:
                 break;
         }

        
        return EndScene();
    }
}