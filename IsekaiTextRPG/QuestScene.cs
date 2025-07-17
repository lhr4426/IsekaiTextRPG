using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class QuestScene : GameScene
{
    public override string SceneName => "퀘스트";
    private List<Quest> availableQuests;

    public override GameScene? StartScene()
    {
        while (true)
        {
            Console.Clear();
            ShowQuestList();

            Console.WriteLine("\n원하시는 퀘스트를 선택해주세요.");
            Console.Write(">> ");

            int? choice = InputHelper.InputNumber(0, availableQuests.Count);
            if (choice == null) continue;
            if (choice == 0) return prevScene;

            ShowQuestDetails(availableQuests[choice.Value - 1]);
        }
    }

    private void ShowQuestList()
    {
        availableQuests = QuestManager.GetAllQuests()
            .Where(q => !GameManager.player.RewardedQuestIds.Contains(q.Id))
            .ToList();

        List<string> questDisplayList = new List<string>();
        for (int i = 0; i < availableQuests.Count; i++)
        {
            var q = availableQuests[i];
            var playerQuest = GameManager.player.InProgressQuests.FirstOrDefault(pq => pq.Id == q.Id);

            string status = "";
            if (playerQuest != null)
            {
                status = playerQuest.State == QuestState.Completed ? " (완료!)" : " (진행 중)";
            }
            questDisplayList.Add($"{i + 1}. {q.Title}{status}");
        }

        UI.DrawTitledBox(SceneName, questDisplayList);
        Console.WriteLine("0. 나가기");
    }

    private void ShowQuestDetails(Quest quest)
    {
        Console.Clear();
        var playerQuest = GameManager.player.InProgressQuests.FirstOrDefault(pq => pq.Id == quest.Id);

        string title = $"퀘스트 - {quest.Title}";
        List<string> contents = new List<string>();
        contents.Add(quest.Description);
        contents.Add("");

        int current = playerQuest?.CurrentCount ?? 0;
        contents.Add($"- {quest.ObjectiveDescription} ({Math.Min(current, quest.RequiredCount)}/{quest.RequiredCount})");
        contents.Add("");

        contents.Add("- 보상 -");
        if (quest.RewardItem != null) contents.Add($"{quest.RewardItem} x 1");
        contents.Add($"{quest.RewardGold}G");

        UI.DrawTitledBox(title, contents);

        if (playerQuest == null)
        {
            HandleNotStartedQuest(quest);
        }
        else if (playerQuest.State == QuestState.InProgress)
        {
            Console.WriteLine("1. 돌아가기");
            Console.Write(">> ");
            InputHelper.InputNumber(1, 1);
        }
        else if (playerQuest.State == QuestState.Completed)
        {
            HandleCompletedQuest(playerQuest);
        }
    }

    private void HandleNotStartedQuest(Quest quest)
    {
        Console.WriteLine("1. 수락");
        Console.WriteLine("2. 거절");
        Console.Write(">> ");
        int? choice = InputHelper.InputNumber(1, 2);
        if (choice == 1)
        {
            quest.State = QuestState.InProgress;
            GameManager.player.InProgressQuests.Add(quest);
            UI.DrawBox(new List<string> { "퀘스트를 수락했습니다." });
            Console.ReadKey();
        }
    }

    private void HandleCompletedQuest(Quest quest)
    {
        Console.WriteLine("1. 보상 받기");
        Console.WriteLine("2. 돌아가기");
        Console.Write(">> ");
        int? choice = InputHelper.InputNumber(1, 2);
        if (choice == 1)
        {
            GameManager.player.Gold += quest.RewardGold;

            quest.State = QuestState.Rewarded;
            GameManager.player.RewardedQuestIds.Add(quest.Id);
            GameManager.player.InProgressQuests.Remove(quest);

            UI.DrawBox(new List<string> { "보상을 받았습니다!" });
            Console.ReadKey();
        }
    }
}