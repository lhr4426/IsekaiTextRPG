using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class GameScene
{
    public abstract string SceneName { get; }

    protected GameScene? prevScene;
    protected List<GameScene> nextScenes = new();

    public virtual void SetNextScene(GameScene scene) => nextScenes.Add(scene);
    public virtual void SetPrevScene(GameScene? prevScene) => this.prevScene = prevScene;

    /// <summary>
    /// 실제로 씬 로직이 들어가는 부분입니다.
    /// 마지막에 return EndScene()을 호출해 주시면 됩니다.
    /// </summary>
    /// <returns></returns>
    public abstract GameScene? StartScene();

    public GameScene? EndScene()
    {
        List<string> options = new();
        foreach (var scene in nextScenes)
        {
            options.Add($" {nextScenes.IndexOf(scene) + 1}: {scene.SceneName}");
        }
        options.Add(prevScene != null
                    ? $" 0: {prevScene.SceneName}"
                    : " 0: 게임 종료");

        UI.DrawLeftAlignedBox(options);

        Console.Write(">> ");
        int? nextSceneIdx = InputHelper.InputNumber(0, nextScenes.Count);
        if (nextSceneIdx == 0)
        {
            return prevScene;
        }
        else if (nextSceneIdx > 0 && nextSceneIdx <= nextScenes.Count)
        {
            return nextScenes[(int)nextSceneIdx - 1];
        }
        else return this;



    }

}


