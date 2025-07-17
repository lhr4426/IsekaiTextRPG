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
            title: "마을을 위협하는 슬라임 처치",
            description: "이봐! 마을 근처에 슬라임들이 너무 많아졌다고 생각하지 않나?\n마을 주민들의 안전을 위해서라도 저것들 수를 좀 줄여야 한다고!\n모험가인 자네가 좀 처치해주게!",
            objective: "슬라임", // 목표 몬스터 이름
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

    // 레벨 퀘스트 진행도를 업데이트하는 함수를 추가
    public static void UpdateLevelQuestProgress(int currentLevel)
    {
        if (GameManager.player == null)
        {
            Console.WriteLine("[경고] QuestManager: 플레이어 객체가 아직 초기화되지 않았습니다. 레벨 퀘스트 업데이트를 건너뜀.");
            return;
        }

        foreach (var quest in GameManager.player.InProgressQuests)
        {
            // 퀘스트가 진행 중이고, 목표가 "레벨 달성"이며, 아직 완료되지 않았을 경우
            if (quest.State == QuestState.InProgress && quest.ObjectiveDescription == "레벨 달성")
            {
                // 현재 플레이어 레벨이 퀘스트 목표 레벨에 도달했는지 확인
                if (currentLevel >= quest.RequiredCount)
                {
                    // 퀘스트 현재 카운트를 목표치로 설정하여 완료 처리
                    quest.CurrentCount = quest.RequiredCount;
                    quest.State = QuestState.Completed;
                    Console.WriteLine($"[퀘스트 완료] '{quest.Title}' 퀘스트를 완료했습니다!");
                }
                else
                {
                    // 목표 레벨에 도달하지 않았다면 현재 레벨을 카운트로 업데이트
                    quest.CurrentCount = currentLevel;
                }
            }
        }
    }

    // 플레이어의 퀘스트 진행도를 업데이트하는 함수들
    public static void UpdateKillQuestProgress(string monsterName)
    {
        if (GameManager.player == null) return;

        foreach (var quest in GameManager.player.InProgressQuests)
        {
            if (quest.Type == QuestType.Kill &&
                quest.ObjectiveDescription == monsterName &&
                quest.State == QuestState.InProgress)
            {
                quest.CurrentCount++;
                Console.WriteLine($"[퀘스트 진행] {quest.Title}: {quest.CurrentCount}/{quest.RequiredCount}");

                if (quest.CurrentCount >= quest.RequiredCount)
                {
                    quest.State = QuestState.Completed;
                    Console.WriteLine($"[퀘스트 완료] '{quest.Title}' 퀘스트를 완료했습니다!");
                }
            }
        }
    }
    // 퀘스트를 플레이어에게 제안하는 메서드
    public static void OfferQuest(int questId)
    {
        Quest? masterQuest = GetQuestById(questId);
        if (masterQuest == null)
        {
            Console.WriteLine("해당 ID의 퀘스트가 없습니다.");
            return;
        }

        if (GameManager.player == null) // Null 체크 추가
        {
            Console.WriteLine("[경고] QuestManager: 플레이어 객체가 아직 초기화되지 않아 퀘스트를 제안할 수 없습니다.");
            return;
        }

        // 플레이어가 이미 진행 중이거나 완료/보상 받은 퀘스트인지 확인
        if (GameManager.player.InProgressQuests.Any(q => q.Id == questId) ||
            GameManager.player.RewardedQuestIds.Contains(questId))
        {
            Console.WriteLine($"플레이어는 이미 '{masterQuest.Title}' 퀘스트를 알고 있습니다.");
            return;
        }

        Console.WriteLine($"\n[퀘스트 제안]: {masterQuest.Title}");
        Console.WriteLine(masterQuest.Description);
        Console.WriteLine($"목표: {masterQuest.ObjectiveDescription} {masterQuest.RequiredCount} (현재 {masterQuest.CurrentCount})");
        Console.WriteLine($"보상: {masterQuest.RewardGold} 골드");
        if (masterQuest.RewardItem != null)
        {
            Console.WriteLine($"         {masterQuest.RewardItem}");
        }
        Console.WriteLine("이 퀘스트를 수락하시겠습니까? (Y/N)");
        Console.Write(">> ");
        string? input = Console.ReadLine()?.ToUpper();

        if (input == "Y")
        {
            masterQuest.State = QuestState.InProgress;
            GameManager.player.InProgressQuests.Add(masterQuest);
            Console.WriteLine($"'{masterQuest.Title}' 퀘스트를 수락했습니다!");

            // 퀘스트 수락 시 레벨 퀘스트라면 현재 레벨을 기준으로 한번 업데이트
            if (masterQuest.ObjectiveDescription == "레벨 달성")
            {
                UpdateLevelQuestProgress(GameManager.player.Level);
            }
        }
        else
        {
            Console.WriteLine("퀘스트를 거절했습니다.");
        }
        Console.ReadKey();
    }

    // 퀘스트 완료 시도 메서드
    public static void TryCompleteQuest(int questId)
    {
        if (GameManager.player == null) return; // Null 체크 추가
        Quest? playerQuest = GameManager.player.InProgressQuests.FirstOrDefault(q => q.Id == questId);

        if (playerQuest == null)
        {
            Console.WriteLine("진행 중인 해당 퀘스트가 없습니다.");
            return;
        }

        if (playerQuest.State == QuestState.Completed)
        {
            Console.WriteLine($"'{playerQuest.Title}' 퀘스트를 완료했습니다! 보상을 받으시겠습니까? (Y/N)");
            Console.Write(">> ");
            string? input = Console.ReadLine()?.ToUpper();

            if (input == "Y")
            {
                GameManager.player.Gold += playerQuest.RewardGold;
                if (playerQuest.RewardItem != null)
                {
                    // 여기에 아이템을 플레이어 인벤토리에 추가하는 로직 필요
                    Console.WriteLine($"'{playerQuest.RewardItem}'을(를) 획득했습니다.");
                }

                playerQuest.State = QuestState.Rewarded;
                GameManager.player.RewardedQuestIds.Add(playerQuest.Id);
                GameManager.player.InProgressQuests.Remove(playerQuest);
                Console.WriteLine("보상을 획득했습니다!");
            }
            else
            {
                Console.WriteLine("보상을 받지 않았습니다. 퀘스트는 완료 상태로 유지됩니다.");
            }
        }
        else if (playerQuest.State == QuestState.InProgress)
        {
            Console.WriteLine($"'{playerQuest.Title}' 퀘스트는 아직 진행 중입니다. (현재: {playerQuest.CurrentCount}/{playerQuest.RequiredCount})");
        }
        else if (playerQuest.State == QuestState.Rewarded)
        {
            Console.WriteLine("이미 보상을 받은 퀘스트입니다.");
        }
        Console.ReadKey();
    }
}