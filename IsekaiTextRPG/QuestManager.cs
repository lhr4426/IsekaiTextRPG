using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class QuestManager
{
    // 게임 내 모든 퀘스트의 원본 목록
    private static Dictionary<int, Quest> masterQuests = new Dictionary<int, Quest>();

    public static void Initialize()
    {
        // 여기에 게임에 등장할 모든 퀘스트를 미리 만들어 둡니다.
        masterQuests.Add(1, new Quest(
            id: 1,
            title: "마을을 위협하는 미니언 처치",
            description: "이봐! 마을 근처에 미니언들이 너무 많아졌다고 생각하지 않나?\n마을 주민들의 안전을 위해서라도 저것들 수를 좀 줄여야 한다고!\n모험가인 자네가 좀 처치해주게!",
            objective: "미니언", // 목표 몬스터 이름
            requiredCount: 5,
            rewardGold: 50,
            rewardItem: "쓸만한 방패"
        ));

        masterQuests.Add(2, new Quest(
            id: 2,
            title: "장비를 장착해보자",
            description: "맨몸으로 싸우는 건 위험하다네. 기본적인 장비라도 갖추는 게 어떤가?",
            objective: "장비 장착", // 특별한 목표 타입
            requiredCount: 1,
            rewardGold: 100
        ));

        masterQuests.Add(3, new Quest(
            id: 3,
            title: "더욱 더 강해지기!",
            description: "레벨 5를 달성하여 당신의 강함을 증명해보세요.",
            objective: "레벨 달성", // 특별한 목표 타입
            requiredCount: 5,
            rewardGold: 500
        ));
    }

    // ID로 퀘스트 원본 정보를 가져오는 함수
    public static Quest? GetQuestById(int id)
    {
        return masterQuests.ContainsKey(id) ? new Quest(masterQuests[id]) : null; // 복사본을 반환
    }

    // 모든 퀘스트 목록을 반환하는 함수
    public static List<Quest> GetAllQuests()
    {
        return masterQuests.Values.Select(q => new Quest(q)).ToList();
    }

    // 플레이어의 퀘스트 진행도를 업데이트하는 함수들
    public static void UpdateKillQuestProgress(string monsterName)
    {
        foreach (var quest in GameManager.player.InProgressQuests)
        {
            if (quest.State == QuestState.InProgress && quest.ObjectiveDescription == monsterName)
            {
                quest.CurrentCount++;
                if (quest.CurrentCount >= quest.RequiredCount)
                {
                    quest.State = QuestState.Completed;
                }
            }
        }
    }
}