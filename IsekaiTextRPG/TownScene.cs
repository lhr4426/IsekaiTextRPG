using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class TownScene : GameScene
{
    public override string SceneName => "마을";

    public override GameScene? StartScene()
    {
        Console.Clear();
        UI.DrawTitledBox(SceneName, null);
        return EndScene();
    }
}
