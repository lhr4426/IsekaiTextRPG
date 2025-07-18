﻿using System;
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
        JeonJikScene,
        InvenScene,
        SkillInvenScene,
        SkillShopScene,
        QuestScene,
        ShopScene,
        RestScene,
        DungeonEnterScene,
        BossDungeonScene,
        LevelDungeonScene,
        NormalBattleScene,
        BossBattleScene,
    }

    public static SceneManager Instance { get; private set; }
    public static ItemSystem ItemSystem { get; private set; } = new ItemSystem();

    public Dictionary<SceneType, GameScene> scenes = new Dictionary<SceneType, GameScene>()
    {
        // TODO : 씬 만들기
        
        { SceneType.FirstScene, new FirstScene() },
        { SceneType.TownScene, new TownScene() },
        { SceneType.StatScene, new StatScene() },
        { SceneType.JeonJikScene, new JeonJikScene() },
        { SceneType.InvenScene, new InventoryScene() },
        { SceneType.SkillInvenScene, new SkillInvenScene() },
        { SceneType.SkillShopScene, new SkillShopScene() },
        { SceneType.QuestScene, new QuestScene() },
        // { SceneType.GuildScene, new GuildScene() },
        { SceneType.ShopScene, new ShopScene(ItemSystem) },
        { SceneType.RestScene, new RestScene() },
        { SceneType.DungeonEnterScene, new DungeonEnterScene() },
        { SceneType.BossDungeonScene, new BossDungeonScene() },
        { SceneType.LevelDungeonScene, new LevelDungeonScene() },
        { SceneType.NormalBattleScene, new NormalBattleScene() },
        { SceneType.BossBattleScene, new BossBattleScene() },

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
        scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.JeonJikScene]);
        scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.InvenScene]);
        scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.SkillInvenScene]);

        // scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.GuildScene]);
        scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.ShopScene]);
        scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.SkillShopScene]);
        scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.RestScene]);
        scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.QuestScene]);
        // scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.DungeonScene]);
        scenes[SceneType.TownScene].SetNextScene(scenes[SceneType.DungeonEnterScene]);

        scenes[SceneType.DungeonEnterScene].SetNextScene(scenes[SceneType.LevelDungeonScene]);
        scenes[SceneType.DungeonEnterScene].SetNextScene(scenes[SceneType.BossDungeonScene]);
        scenes[SceneType.LevelDungeonScene].SetNextScene(scenes[SceneType.NormalBattleScene]);
        scenes[SceneType.BossDungeonScene].SetNextScene(scenes[SceneType.BossBattleScene]);

        scenes[SceneType.StatScene].SetPrevScene(scenes[SceneType.TownScene]);
        scenes[SceneType.InvenScene].SetPrevScene(scenes[SceneType.TownScene]);
        scenes[SceneType.JeonJikScene].SetPrevScene(scenes[SceneType.TownScene]);
        scenes[SceneType.SkillInvenScene].SetPrevScene(scenes[SceneType.TownScene]);
        scenes[SceneType.SkillShopScene].SetPrevScene(scenes[SceneType.TownScene]);
        // scenes[SceneType.GuildScene].SetPrevScene(scenes[SceneType.TownScene]);
        scenes[SceneType.ShopScene].SetPrevScene(scenes[SceneType.TownScene]);
        scenes[SceneType.RestScene].SetPrevScene(scenes[SceneType.TownScene]);
        scenes[SceneType.QuestScene].SetPrevScene(scenes[SceneType.TownScene]);
        // scenes[SceneType.DungeonScene].SetPrevScene(scenes[SceneType.TownScene]);
        scenes[SceneType.DungeonEnterScene].SetPrevScene(scenes[SceneType.TownScene]);
        scenes[SceneType.BossDungeonScene].SetPrevScene(scenes[SceneType.DungeonEnterScene]);
        scenes[SceneType.LevelDungeonScene].SetPrevScene(scenes[SceneType.DungeonEnterScene]);
        scenes[SceneType.NormalBattleScene].SetPrevScene(scenes[SceneType.LevelDungeonScene]);
        scenes[SceneType.BossBattleScene].SetPrevScene(scenes[SceneType.BossDungeonScene]);

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

