using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class SceneManager
{
    public enum SceneType
    {
        FirstScene,
        TownScene,
        StatScene,
        InvenScene,
        SkillScene,
        ShopScene,
        RestScene,
        DungeonScene,
    }

    public static SceneManager Instance { get; private set; }

    public Dictionary<SceneType, GameScene> scenes = new Dictionary<SceneType, GameScene>()
    {
        // TODO : 씬 만들기
        
        { SceneType.FirstScene, new FirstScene() },
        { SceneType.TownScene, new TownScene() },
        { SceneType.StatScene, new StatScene() },
        { SceneType.InvenScene, new InventoryScene() },
        // { SceneType.SkillScene, new SkillScene() },
        // { SceneType.GuildScene, new GuildScene() },
        // { SceneType.ShopScene, new ShopScene() },
        { SceneType.RestScene, new RestScene() },
        // { SceneType.DungeonScene, new DungeonScene() }
        
    };

    public SceneManager()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            throw new InvalidOperationException("SceneManager 인스턴스는 하나만 생성할 수 있습니다.");
        }
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        SceneSetting();
    }

    private void SceneSetting()
    {
        
        scenes[SceneType.FirstScene].SetNextScene(scenes[SceneType.TownScene]);
        
        scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.StatScene]);
        scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.InvenScene]);
        // scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.SkillScene]);
        // scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.GuildScene]);
        // scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.ShopScene]);
        scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.RestScene]);
        // scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.DungeonScene]);

        scenes[SceneType.StatScene].SetPrevScene(scenes[SceneType.TownScene]);
        scenes[SceneType.InvenScene].SetPrevScene(scenes[SceneType.TownScene]);
        // scenes[SceneType.SkillScene].SetPrevScene(scenes[SceneType.TownScene]);
        // scenes[SceneType.GuildScene].SetPrevScene(scenes[SceneType.TownScene]);
        // scenes[SceneType.ShopScene].SetPrevScene(scenes[SceneType.TownScene]);
        scenes[SceneType.RestScene].SetPrevScene(scenes[SceneType.TownScene]);
        // scenes[SceneType.DungeonScene].SetPrevScene(scenes[SceneType.TownScene]);
        
    }


    // TODO : 이부분 추후에 GameManager로 이동 필요
    public void Start()
    {
        GameScene? currentScene = scenes[SceneType.FirstScene];
        while (currentScene != null)
        {
            currentScene = currentScene.StartScene();
        }
    }


}

