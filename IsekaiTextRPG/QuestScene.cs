using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading; // Thread.Sleep을 위해 추가

public class QuestScene : GameScene
{
    public override string SceneName => "퀘스트"; // 씬 이름 정의

    private QuestManager questManager;
    private List<Quest> currentQuests;
    private Quest? selectedQuest; // Nullable로 변경

    // 퀘스트 목록 화면에서 상세 화면으로, 상세 화면에서 다시 목록 화면으로 전환하기 위한 플래그
    private bool isViewingDetails;

    public QuestScene()
    {
        questManager = QuestManager.instance;
        // 메인 메뉴로 돌아가거나, 상세 퀘스트 화면에서 목록으로 돌아가는 것은
        // StartScene() 내부의 로직에서 처리
    }

    public override GameScene? StartScene()
    {
        Console.Clear();
        isViewingDetails = false; // 항상 퀘스트 목록 화면으로 시작

        GameScene? nextScene = this; // 기본적으로 현재 씬 유지

        while (nextScene == this) // 현재 씬이 아닌 다른 씬으로 전환될 때까지 반복
        {
            Console.Clear(); // 화면을 매번 지워줍니다.

            if (!isViewingDetails) // 퀘스트 목록 화면
            {
                nextScene = DisplayQuestListAndHandleInput(); // 목록 표시 및 입력 처리
            }
            else // 퀘스트 상세 정보 화면
            {
                nextScene = DisplayQuestDetailsAndHandleInput(selectedQuest); // 상세 정보 표시 및 입력 처리
            }
        }
        return nextScene; // 다음 씬 반환 (null이면 게임 종료)
    }

    // 퀘스트 목록을 표시하고 사용자 입력을 처리하여 다음 씬(또는 현재 씬)을 반환하는 메서드
    private GameScene? DisplayQuestListAndHandleInput()
    {
        currentQuests = questManager.GetAllQuests();
        // 완료된 퀘스트 > 보상 받은 퀘스트 > 진행 중 퀘스트 > 미수락 퀘스트 순으로 정렬
        currentQuests = currentQuests
            .OrderBy(q => q.IsRewarded ? 1 : (q.IsCompleted ? 2 : (q.IsAccepted ? 3 : 4)))
            .ToList();

        List<string> menuOptions = new List<string>();
        if (!currentQuests.Any())
        {
            menuOptions.Add("현재 이용 가능한 퀘스트가 없습니다.");
        }
        else
        {
            for (int i = 0; i < currentQuests.Count; i++)
            {
                string status = "";
                if (currentQuests[i].IsRewarded) status = "(보상 받음)";
                else if (currentQuests[i].IsCompleted) status = "(완료!)";
                else if (currentQuests[i].IsAccepted) status = "(진행 중)";
                else status = "(미수락)";

                menuOptions.Add($"{i + 1}. {currentQuests[i].Name} {status}");
            }
        }

        // 0번 옵션 (돌아가기) 추가
        menuOptions.Add("0. 돌아가기");

        UI.DrawTitledBox(SceneName, menuOptions); // 씬 이름과 퀘스트 목록을 박스 안에

        Console.Write(">> ");
        int? choice = InputHelper.InputNumber(0, currentQuests.Count); // 0부터 퀘스트 개수까지 입력 허용

        if (choice == null) // 유효하지 않은 입력
        {
            Console.WriteLine("잘못된 입력입니다. 다시 선택해주세요.");
            Thread.Sleep(800);
            return this; // 현재 씬 유지
        }
        else if (choice == 0) // 돌아가기 (이전 씬)
        {
            return prevScene; // GameScene의 prevScene 필드를 사용 (마을 씬으로 돌아감)
        }
        else if (choice >= 1 && choice <= currentQuests.Count) // 특정 퀘스트 선택
        {
            selectedQuest = currentQuests[choice.Value - 1];
            isViewingDetails = true; // 상세 보기 모드로 전환
            return this; // 현재 씬 유지 (상세 화면으로 전환)
        }
        else
        {
            Console.WriteLine("유효하지 않은 퀘스트 번호입니다.");
            Thread.Sleep(800);
            return this; // 현재 씬 유지
        }
    }

    // 퀘스트 상세 정보를 표시하고 사용자 입력을 처리하여 다음 씬(또는 현재 씬)을 반환하는 메서드
    private GameScene? DisplayQuestDetailsAndHandleInput(Quest? quest)
    {
        if (quest == null)
        {
            UI.DrawBox(new List<string> { "잘못된 퀘스트입니다." });
            Console.WriteLine("\n");
            Console.WriteLine(" 아무 키나 눌러 목록으로 돌아가기 ");
            Console.WriteLine(">> ");
            Console.ReadKey(true); // 아무 키나 누르기 기다림
            isViewingDetails = false; // 목록으로 돌아감
            return this; // 현재 씬 유지
        }

        List<string> detailsContents = new List<string>();
        detailsContents.Add($"<설명>");
        string[] descriptionLines = quest.Description.Split('\n');
        foreach (string line in descriptionLines)
        {
            detailsContents.Add(line);
        }
        detailsContents.Add(""); // 공백 줄

        detailsContents.Add("<완료 조건>");
        detailsContents.Add($"- {quest.CompletionCondition}");
        if (quest.Type == QuestType.MonsterHunt && quest.IsAccepted)
        {
            detailsContents.Add($"{quest.GetProgressString()}");
        }
        detailsContents.Add(""); // 공백 줄

        detailsContents.Add("<보상>");
        foreach (var reward in quest.Rewards)
        {
            detailsContents.Add($"- {reward.ItemName} {reward.Quantity}개");
        }

        UI.DrawTitledBox($"퀘스트: {quest.Name}", detailsContents); // 퀘스트 이름과 상세 정보를 박스 안에

        List<string> actionOptions = new List<string>();
        if (quest.IsRewarded)
        {
            actionOptions.Add("  [보상 수령 완료]");
            actionOptions.Add("0. 돌아가기");
        }
        else if (quest.IsCompleted)
        {
            actionOptions.Add("1. 보상 받기");
            actionOptions.Add("0. 돌아가기");
        }
        else if (quest.IsAccepted)
        {
            actionOptions.Add("1. 퀘스트 포기");
            actionOptions.Add("0. 돌아가기");
        }
        else
        {
            actionOptions.Add("1. 퀘스트 수락");
            actionOptions.Add("0. 돌아가기");
        }

        UI.DrawLeftAlignedBox(actionOptions); // 행동 옵션을 박스 안에
        Console.Write(">> ");

        int maxChoice = (quest.IsRewarded) ? 0 : 1; // 보상 완료 상태면 0만 유효, 아니면 1까지 유효
        int? choice = InputHelper.InputNumber(0, maxChoice);

        if (choice == null) // 유효하지 않은 입력
        {
            Console.WriteLine("잘못된 입력입니다. 다시 선택해주세요.");
            Thread.Sleep(800);
            return this; // 현재 씬 유지
        }
        else if (choice == 0) // 돌아가기 (목록으로)
        {
            isViewingDetails = false; // 목록 보기 모드로 전환
            return this; // 현재 씬 유지
        }
        else if (choice == 1) // 1번 옵션 (수락, 포기, 보상 받기)
        {
            if (quest.IsCompleted) // 보상 받기
            {
                questManager.GiveQuestRewards(quest);
                Thread.Sleep(1000); // 메시지 확인 시간
                // 보상 받은 후에는 상세 화면 유지하며 "보상 수령 완료" 텍스트 표시
            }
            else if (quest.IsAccepted) // 퀘스트 포기
            {
                if (questManager.AbandonQuest(quest.Name))
                {
                    Console.WriteLine($"\n'{quest.Name}' 퀘스트를 포기했습니다.");
                    quest.IsAccepted = false; // 상태 업데이트
                    Thread.Sleep(1000);
                    isViewingDetails = false; // 목록으로 돌아감
                }
            }
            else // 퀘스트 수락
            {
                if (questManager.AcceptQuest(quest.Name))
                {
                    Console.WriteLine($"\n'{quest.Name}' 퀘스트를 수락했습니다!");
                    Thread.Sleep(1000);
                    isViewingDetails = false; // 목록으로 돌아감
                }
            }
            return this; // 현재 씬 유지
        }
        else
        {
            Console.WriteLine("유효하지 않은 선택입니다.");
            Thread.Sleep(800);
            return this; // 현재 씬 유지
        }
    }
}