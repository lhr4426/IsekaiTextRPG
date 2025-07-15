using System;
using System.Collections.Generic;

public class StatScene : GameScene
{
    public override string SceneName => "스테이터스 화면";

    public override GameScene? StartScene()
    {
        GameManager.player.ShowStatus();

        string? str = Console.ReadLine();
        
        if (int.TryParse(str, out int index))
        {
            switch (index)
            {
                case 0: //메인화면으로 이동 처리 예정
                    return null;
                default:
                    break;
            }

        }
        return this;
    }
}