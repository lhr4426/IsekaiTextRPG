using System;
using System.Collections.Generic;
using System.Linq;

public class QuestManager
{
    public static QuestManager instance;
    private Dictionary<string, Quest> quests;

    public QuestManager()
    {
        if (instance == null)
        {
            instance = this;
            quests = new Dictionary<string, Quest>();
            InitializeQuests();
        }
        else
        {
            throw new Exception("QuestManager Instance는 하나만 존재할 수 있습니다.");
        }
    }

    private void InitializeQuests()
    {
        List<Quest.Reward> minionQuestRewards = new List<Quest.Reward>
        {
            new Quest.Reward("슬픔한 방패", 1),
            new Quest.Reward("골드", 5)
        };
        Quest minionQuest = new Quest(
            "마을을 위협하는 미니언 처치",
            "이봐! 마을 근처에 미니언들이 너무 많아졌다고 생각하지 않나?\n마을 주민들의 안전을 위해서라도 저것들을 수를 좀 줄여야 한다고!\n모험가인 자네가 좀 처치해주게!",
            QuestType.MonsterHunt,
            "미니언 5마리 처치",
            minionQuestRewards,
            "미니언",
            5
        );
        quests.Add(minionQuest.Name, minionQuest);

        List<Quest.Reward> equipQuestRewards = new List<Quest.Reward>
        {
            new Quest.Reward("낡은 검", 1),
            new Quest.Reward("경험치", 10)
        };
        Quest equipQuest = new Quest(
            "장비를 장착해보자",
            "강한 모험가가 되기 위해선 좋은 장비를 착용하는 것이 기본 중의 기본이지! \n자네가 가지고 있는 장비가 있다면 착용해보게!",
            QuestType.MonsterHunt,
            "무기 장착",
            equipQuestRewards,
            "", 0
        );
        quests.Add(equipQuest.Name, equipQuest);

        List<Quest.Reward> strengthenQuestRewards = new List<Quest.Reward>
        {
            new Quest.Reward("체력 포션", 3),
            new Quest.Reward("경험치", 20)
        };
        Quest strengthenQuest = new Quest(
            "더욱 더 강해지기!",
            "몬스터를 처치하고 레벨을 올려 더욱 더 강해져라! \n강력한 모험가만이 살아남을 수 있다!",
            QuestType.MonsterHunt,
            "레벨 2 달성",
            strengthenQuestRewards,
            "", 0
        );
        quests.Add(strengthenQuest.Name, strengthenQuest);
    }

    public List<Quest> GetAllQuests()
    {
        return quests.Values.ToList();
    }

    public List<Quest> GetAcceptedQuests()
    {
        return quests.Values.Where(q => q.IsAccepted).ToList();
    }

    public Quest GetQuest(string name)
    {
        if (quests.ContainsKey(name))
        {
            return quests[name];
        }
        return null;
    }

    public bool AcceptQuest(string questName)
    {
        Quest quest = GetQuest(questName);
        if (quest != null && !quest.IsAccepted)
        {
            quest.IsAccepted = true;
            return true;
        }
        return false;
    }

    public bool AbandonQuest(string questName)
    {
        Quest quest = GetQuest(questName);
        if (quest != null && quest.IsAccepted && !quest.IsCompleted)
        {
            quest.IsAccepted = false;
            return true;
        }
        return false;
    }

    public void GiveQuestRewards(Quest quest)
    {
        if (quest != null && quest.IsCompleted && !quest.IsRewarded)
        {
            foreach (var reward in quest.Rewards)
            {
                if (reward.ItemName == "골드")
                {
                    GameManager.player.Gold += reward.Quantity;
                    Console.WriteLine($"골드 {reward.Quantity}를 획득했습니다!");
                }
                else if (reward.ItemName == "경험치")
                {
                    GameManager.player.GainExp(reward.Quantity);
                    Console.WriteLine($"경험치 {reward.Quantity}를 획득했습니다!");
                }
                else
                {
                    Console.WriteLine($"{reward.ItemName} {reward.Quantity}개를 획득했습니다!");
                }
            }
            quest.IsRewarded = true;
            Console.WriteLine($"'{quest.Name}' 퀘스트 보상을 수령했습니다!");
        }
    }

    public void NotifyMonsterDefeated(string monsterName)
    {
        foreach (var quest in GetAcceptedQuests())
        {
            quest.UpdateProgress(monsterName);
        }
    }
}