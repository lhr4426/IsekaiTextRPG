using System;
using System.Collections.Generic;

public enum QuestType
{
    MonsterHunt,
    Gathering,
    Delivery
}

public class Quest
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public QuestType Type { get; private set; }
    public string CompletionCondition { get; private set; }
    public List<Reward> Rewards { get; private set; }
    public bool IsAccepted { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsRewarded { get; set; }

    public string TargetMonster { get; private set; }
    public int RequiredCount { get; set; }
    public int CurrentCount { get; set; }

    public Quest(string name, string description, QuestType type, string condition, List<Reward> rewards, string targetMonster = "", int requiredCount = 0)
    {
        Name = name;
        Description = description;
        Type = type;
        CompletionCondition = condition;
        Rewards = rewards;
        IsAccepted = false;
        IsCompleted = false;
        IsRewarded = false;

        TargetMonster = targetMonster;
        RequiredCount = requiredCount;
        CurrentCount = 0;
    }

    public class Reward
    {
        public string ItemName { get; private set; }
        public int Quantity { get; private set; }

        public Reward(string itemName, int quantity)
        {
            ItemName = itemName;
            Quantity = quantity;
        }
    }

    public void UpdateProgress(string monsterName)
    {
        if (IsAccepted && !IsCompleted && Type == QuestType.MonsterHunt && TargetMonster == monsterName)
        {
            CurrentCount++;
            if (CurrentCount >= RequiredCount)
            {
                IsCompleted = true;
            }
        }
    }

    public string GetProgressString()
    {
        if (Type == QuestType.MonsterHunt)
        {
            return $" - {TargetMonster} {CurrentCount}/{RequiredCount}";
        }
        return "";
    }
}