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
    /// 마지막에 EndScene을 호출해 주시면 됩니다.
    /// </summary>
    /// <returns></returns>
    public abstract GameScene? StartScene();

    public GameScene? EndScene()
    {
        foreach (var scene in nextScenes)
        {
            Console.WriteLine($"{nextScenes.IndexOf(scene) + 1}: {scene.SceneName}");
        }
        if (prevScene != null)
        {
            Console.WriteLine($"0: {prevScene.SceneName}");
        }
        else
        {
            Console.WriteLine("0: 게임 종료");
        }
        Console.WriteLine("이동할 곳을 선택하세요. (숫자 입력)");

        // TODO : 게임 저장 필요
        // GameManager.instance.SavePlayerData();

        Console.Write(">> ");
        string input = Console.ReadLine()?.Trim() ?? "-1";
        if (int.TryParse(input, out int nextSceneIdx))
        {
            if (nextSceneIdx == 0)
            {
                return prevScene;
            }
            else if (nextSceneIdx > 0 && nextSceneIdx < nextScenes.Count)
            {
                return nextScenes[nextSceneIdx - 1];
            }
            else return this;
        }
        else
        {
            return this;
        }
    }

}


