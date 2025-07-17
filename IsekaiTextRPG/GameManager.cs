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


    public readonly string saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");

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
        Tutorial();
        // TODO : 튜토리얼 부분으로 대체 예정
        /*
        var welcome = new List<string> { $"{player.Name} 님 환영합니다!" };
        UI.DrawBox(welcome);
        */
        SavePlayerData(slot);
        QuestManager.UpdateLevelQuestProgress(player.Level);
    }

    private void Tutorial()
    {
        Console.Clear();
        List<string> strings = new List<string>()
        {
        $"{GameManager.player.Name}은(는) 로아, 메이플, 던파를 쉬지않고 돌려가면서 플레이 하는 심각한 게임 중독자입니다.",
        "특히, 레이드가 업데이트 되는 날이면 레@불과 몬@터를 먹어가며 몸을 혹사시켰습니다."
        };
        UI.TypeWriter(UI.DrawBox(strings, true));
        Console.ReadKey();

        Console.Clear();
        strings.Clear();
        strings = new List<string>()
        {
            $"오늘도 {GameManager.player.Name}은(는) 그런 중독자 라이프를 즐기고 있었습니다.",
            "그러나... 뒤바뀐 생활 패턴과 카페인 과다는 상상치 못한 결과를 초래했습니다."
        };
        UI.TypeWriter(UI.DrawBox(strings, true));
        Console.ReadKey();

        Console.Clear();
        strings.Clear();
        strings = new List<string>()
        {
            $"{GameManager.player.Name}은(는) 눈앞이 깜깜해졌습니다."
        };
        UI.TypeWriter(UI.DrawBox(strings, true));
        Console.ReadKey();

        Console.Clear();
        strings.Clear();
        strings = new List<string>()
        {
            "시간이 얼마나 지났을까요?",
            "무거웠던 눈꺼풀이 가벼워지기 시작합니다.",
            "그리고... 낯선 누군가의 목소리가 들리기 시작합니다."
        };
        UI.TypeWriter(UI.DrawBox(strings, true));
        Console.ReadKey();

        Console.Clear();
        strings.Clear();
        strings = new List<string>()
        {
            "\" 이봐! 지금 그렇게 누워있을 시간이 없다고! 어서 일어나! \" ",
            "저 사람은 누군데 저렇게 소리를 지르는 걸까요?",
            "아니, 그나저나 여긴 지금 어딜까요...?",
            "군데군데 묘하게 익숙한 느낌이 듭니다..."
        };
        UI.TypeWriter(UI.DrawBox(strings, true));
        Console.ReadKey();

        Console.Clear();
        strings.Clear();
        strings = new List<string>()
        {
            "주변을 둘러보니, 아까 소리지른 것으로 추정되는 사람과 슬라임, 고블린이... 어?",
            "메이플과 던파에서 보던 몬스터가 보입니다...?",
        };
        UI.TypeWriter(UI.DrawBox(strings, true));
        Console.ReadKey();

        Console.Clear();
        strings.Clear();
        strings = new List<string>()
        {
            "\" 지금 몬스터들 안보여!? 에휴... 이래서 환생자들은 못 써먹겠다니까... \"",
            "\" 빨리 일어나. 저 옆에 보이는 마을로 곧장 뛰어. 가서 퀘스트 게시판이나 확인해 봐. \"",
            "\" 매번 귀찮게 구는구만... \"",
        };
        UI.TypeWriter(UI.DrawBox(strings, true));
        Console.ReadKey();

        Console.Clear();
        strings.Clear();
        strings = new List<string>()
        {
            "당신은 저 성질 더러워보이는 사람의 발언에 정신을 차리고 옆을 확인해 봅니다.",
        };
        UI.TypeWriter(UI.DrawBox(strings, true));
        Console.ReadKey();
    }

    public GameManager()
    {
        SkillManager.Initialize(); // 스킬 초기화
        QuestManager.Initialize(); // 퀘스트 초기화

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
            QuestManager.UpdateLevelQuestProgress(player.Level);
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
                strings.Add($" {i}. {p?.Name} (Lv.{p?.Level}) - {Player.JobsKorean((Player.Jobs)p?.Job)}");
            }
            else
            {
                strings.Add($" {i}. [빈 슬롯]");
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