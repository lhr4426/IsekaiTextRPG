using System;
using System.Collections.Generic;

public class StatScene : GameScene
{
    public override string SceneName => "스테이터스 화면";

    public override GameScene? StartScene()
    {
        Console.Clear();
        GameManager.player.ShowStatus();
        return EndScene();
    }
}