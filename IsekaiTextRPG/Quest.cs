using System;
using System.Collections.Generic;
using System.Linq;

// 퀘스트의 상태를 나타내는 열거형
public enum QuestState
{
    NotStarted, // 아직 시작 안 함
    InProgress, // 진행 중
    Completed,  // 완료됨 (보상 받기 전)
    Rewarded    // 보상까지 받은 최종 완료
}

public class Quest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public QuestState State { get; set; }

    // 퀘스트 목표
    public string ObjectiveDescription { get; set; } // 예: "미니언 5마리 처치"
    public int RequiredCount { get; set; }
    public int CurrentCount { get; set; }

    // 퀘스트 보상
    public int RewardGold { get; set; }
    public string? RewardItem { get; set; } // 아이템 이름으로 보상

    public Quest() { }

    public Quest(int id, string title, string description, string objective, int requiredCount, int rewardGold, string? rewardItem = null)
    {
        Id = id;
        Title = title;
        Description = description;
        State = QuestState.NotStarted;
        ObjectiveDescription = objective;
        RequiredCount = requiredCount;
        CurrentCount = 0;
        RewardGold = rewardGold;
        RewardItem = rewardItem;
    }

    // 복사 생성자: 플레이어가 퀘스트를 받을 때 원본은 그대로 두고 복사본을 만들어 전달하기 위함
    public Quest(Quest original)
    {
        Id = original.Id;
        Title = original.Title;
        Description = original.Description;
        State = original.State;
        ObjectiveDescription = original.ObjectiveDescription;
        RequiredCount = original.RequiredCount;
        CurrentCount = original.CurrentCount;
        RewardGold = original.RewardGold;
        RewardItem = original.RewardItem;
    }
}