using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GameManager
{
    public static GameManager instance;
    public static SceneManager sceneManager = new SceneManager();
    public static Player player;
    public int selectedSlot;


    private readonly string saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");

    private string GetSlotPath(int slot) => Path.Combine(saveDir, $"slot{slot}.json");


    public void NewPlayerData(int slot = 1)
    {
        Console.Clear();
        string name;
        do
        {
           UI.DrawBox(new List<string> { "플레이어 이름을 입력하세요 (2글자 이상)" });
           Console.Write(">> ");
           name = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(name))
            {
                UI.DrawBox(new List<string> { "이름은 반드시 입력해야 합니다. 다시 시도하세요." });
            }
            else if (name.Length < 2)
            {
                UI.DrawBox(new List<string> { "이름은 2글자 이상이어야 합니다. 다시 시도하세요." });
            }
            else if (name.Length > 12)
            {
                UI.DrawBox(new List<string> { "이름은 12글자 이하이어야 합니다. 다시 시도하세요." });
            }
            else
            {
                break;
            }
           Console.ReadKey();
           Console.Clear();
        } 
        while (true);
    
        player = new Player(name);
        var welcome = new List<string> { $"{player.Name} 님 환영합니다!" };
        UI.DrawBox(welcome);
        SavePlayerData(slot);
        Console.ReadKey();
    }

    public GameManager()
    {
        SkillManager.Initialize(); // 스킬 초기화

        if (instance == null)
        {
            instance = this;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }
        else
        {
            throw new Exception("GameManager Instance는 하나만 존재할 수 있습니다.");
        }

        Directory.CreateDirectory(saveDir);
    }


    private void OnProcessExit(object? sender, EventArgs e)
    {
        SavePlayerData(selectedSlot);
    }

    public void LoadPlayerData(int slot)
    {
        List<string> strings = new();

        string path = GetSlotPath(slot);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            player = JsonSerializer.Deserialize<Player>(json);
            player.LoadSkillsFromJson();
            strings.Add($"슬롯 {slot}에서 불러오기 완료.");
            UI.DrawBox(strings);
        }
        else
        {
            strings.Add($"슬롯 {slot}에 저장된 데이터가 없습니다. 새로 생성합니다.");
            UI.DrawBox(strings);
            Console.ReadKey();
            NewPlayerData(slot);
        }
        selectedSlot = slot;
        Console.ReadKey();
    }

    public void ShowSaveSlots()
    {
        Console.Clear();
        List<string> strings = new();

        for (int i = 1; i <= 4; i++)
        {
            string path = GetSlotPath(i);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                Player? p = JsonSerializer.Deserialize<Player>(json);
                strings.Add($"{i}. {p?.Name} (Lv.{p?.Level}) - {Player.JobsKorean((Player.Jobs)p?.Job)}");
            }
            else
            {
                strings.Add($"{i}. [빈 슬롯]");
            }
        }
        UI.DrawTitledBox("저장 슬롯", null);
        UI.DrawLeftAlignedBox(strings);
    }


    public void SavePlayerData(int slot)
    {
        List<string> strings = new();
        string json = JsonSerializer.Serialize(player, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(GetSlotPath(slot), json);
        strings.Add($"슬롯 {slot}에 저장 완료!");
        UI.DrawBox(strings);
        Console.ReadKey();
    }

    public void DeleteSlot(int slot)
    {
        List<string> strings = new();
        string path = GetSlotPath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            strings.Add($"슬롯 {slot} 삭제 완료.");
        }
        else
        {
            strings.Add($"슬롯 {slot}에 저장된 데이터가 없습니다.");
        }
        UI.DrawBox(strings);
        Console.ReadKey();
    }

    public void GameStart()
    {
        sceneManager.Start();
    }

    public void GameExit()
    {
        Console.WriteLine("게임을 종료합니다.");
        Environment.Exit(0);
    }
}