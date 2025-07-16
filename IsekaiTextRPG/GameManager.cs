using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GameManager
{
    public static GameManager instance;
    public static SceneManager sceneManager = new SceneManager();
    public static Player player;

    public string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlayerData.json");

    public GameManager()
    {
        ConfigureConsoleWindow(); // 콘솔 크기 설정
        SkillManager.Initialize(); // 스킬 초기화

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new Exception("GameManager Instance는 하나만 존재할 수 있습니다.");
        }
    }

    // 콘솔 크기 조절 메서드
    public static void ConfigureConsoleWindow()
    {
        try
        {
            int targetWidth = 120;
            int targetHeight = 30;

            if (Console.LargestWindowWidth >= targetWidth)
            {
                Console.WindowWidth = targetWidth;
                Console.BufferWidth = targetWidth;
            }

            if (Console.LargestWindowHeight >= targetHeight)
            {
                Console.WindowHeight = targetHeight;
                Console.BufferHeight = targetHeight;
            }
        }
        catch (IOException)
        {
            // 실패해도 무시
        }
    }

    public void NewPlayerData()
    {
        Console.WriteLine("플레이어 이름을 설정해 주세요.");
        Console.Write(">> ");
        string? name = Console.ReadLine();
        player = new Player(name == null ? "Player" : name);
        Console.WriteLine($"{player.Name} 님 환영합니다!");
        SavePlayerData();
    }

    public void LoadPlayerData()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            player = JsonSerializer.Deserialize<Player>(json);
            player.LoadSkillsFromJson(); // 스킬 상태 복구
        }
        else
        {
            Console.WriteLine("저장된 데이터가 없습니다.");
            Console.WriteLine("새 캐릭터를 생성합니다.");
            NewPlayerData();
        }
    }

    public void SavePlayerData()
    {
        string json = JsonSerializer.Serialize(player, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
        Console.WriteLine("플레이어 데이터 저장 완료.");
    }

    public void GameStart()
    {
        sceneManager.Start();
    }

    public void GameExit()
    {
        Console.WriteLine("게임을 종료합니다.");
        SavePlayerData();
        Environment.Exit(0);
    }
}