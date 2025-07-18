using System;
using System.Collections.Generic;
using System.Linq;

// 전직소 씬 클래스
public class JeonJikScene : GameScene
{
    // 씬 이름 오버라이드
    public override string SceneName => "전직소";

    // 씬 시작 시 실행되는 메서드
    public override GameScene? StartScene()
    {
        Console.Clear();
        PrintJobs();       // 직업 목록 출력
        return ChangeClass(); // 전직 처리
    }

    // 직업 목록 출력
    public void PrintJobs()
    {
        List<string> strings = new();

        // '전직(Presitge)' 제외한 직업 정렬 (전직 가능 여부 + 증표 순서대로)
        var sortedJobs = Enum.GetValues(typeof(Player.Jobs))
            .Cast<Player.Jobs>()
            .Where(j => j != Player.Jobs.Prestige)
            .OrderBy(job => GetJobSortPriority(job))
            .ToList();

        strings.Add("[직업 목록]");

        // 직업 리스트 출력 문자열 구성
        for (int i = 0; i < sortedJobs.Count; i++)
        {
            Player.Jobs job = sortedJobs[i];
            string needItemText = "";

            // 해당 직업 전직에 필요한 아이템 텍스트 구성
            string? requiredItem = Player.JobItemName(job);
            if (requiredItem != null)
            {
                needItemText = $"(필요: {requiredItem})";
            }

            // 현재 직업이면 마킹
            string currentJobMark = (GameManager.player.Job == job) ? "[현재 직업]" : "";

            // 리스트에 줄 추가
            strings.Add($"{i + 1}. {Player.JobsKorean(job),-10} | {currentJobMark,-10} | {(CanChangeClass(job) ? "전직 가능" : "전직 불가")} {needItemText}");
        }

        // 출력 UI 처리
        UI.DrawTitledBox(SceneName, null);
        UI.DrawLeftAlignedBox(strings);
        Console.Write("전직할 직업을 선택 (0 : 돌아가기) >> ");
    }

    // 직업 정렬 우선순위 계산 함수
    private int GetJobSortPriority(Player.Jobs job)
    {
        int priority = 0;

        // 전직 가능하면 우선순위 높음
        if (CanChangeClass(job)) priority -= 1000;

        // 필요한 아이템 종류에 따라 정렬 우선순위 부여
        string? itemName = Player.JobItemName(job);
        if (itemName == "전직의 증표 I") priority += 1;
        else if (itemName == "전직의 증표 II") priority += 2;
        else if (itemName == "전직의 증표 III") priority += 3;
        else priority += 4;

        return priority;
    }

    // 플레이어의 직업 변경 처리 함수
    private GameScene? ChangeClass()
    {
        List<string> strings = new();

        // 정렬된 직업 목록
        var sortedJobs = Enum.GetValues(typeof(Player.Jobs))
            .Cast<Player.Jobs>()
            .Where(j => j != Player.Jobs.Prestige)
            .OrderBy(job => GetJobSortPriority(job))
            .ToList();

        int? input = InputHelper.InputNumber(0, sortedJobs.Count);

        if (input == 0) return prevScene;

        // 올바른 번호 입력 시
        if (input > 0 && input <= sortedJobs.Count)
        {
            var selectedJob = sortedJobs[(int)input - 1];

            // 이미 같은 직업이면 메시지 출력 후 리턴
            if (GameManager.player.Job == selectedJob)
            {
                strings.Add($"이미 {Player.JobsKorean(selectedJob)} 직업입니다.");
                UI.DrawBox(strings);
                Console.ReadKey();
                return this;
            }

            // 전직 조건 만족 시
            if (CanChangeClass(selectedJob))
            {
                string? needItemName = Player.JobItemName(selectedJob);

               // 아이템이 필요하면 제거
                if (needItemName != null)
                {
                    Item? itemToRemove = GameManager.player.Inventory.FirstOrDefault(x => x.Name == needItemName);
                    if (itemToRemove != null)
                    {
                        if (itemToRemove.ItemCount > 1)
                        {
                            itemToRemove.ItemCount--; // 수량만 감소
                        }
                        else
                        {
                            GameManager.player.Inventory.Remove(itemToRemove); // 수량이 1개면 제거
                        }

                        strings.Add($"'{itemToRemove.Name}'을(를) 사용하여 전직했습니다.");
                    }
                }


                // 직업 변경 처리
                GameManager.player.Job = selectedJob;
                strings.Add($"'{Player.JobsKorean(selectedJob)}' 직업으로 전직하였습니다!");
                UI.DrawBox(strings);
                Console.ReadKey();
                return prevScene;
            }
            else
            {
                // 전직 불가 사유 메시지 출력
                string? requiredItem = Player.JobItemName(selectedJob);
                if (requiredItem != null)
                {
                    strings.Add($"{Player.JobsKorean(selectedJob)} 직업으로 전직하기 위한 '{requiredItem}'(이)가 없습니다.");
                }
                else
                {
                    strings.Add($"{Player.JobsKorean(selectedJob)} 직업으로 전직할 수 없습니다. (전직 조건 미달성)");
                }
                UI.DrawBox(strings);
                Console.ReadKey();
                return this;
            }
        }
        else
        {
            // 잘못된 입력 처리
            Console.WriteLine("잘못된 입력입니다. 아무 키나 눌러 계속...");
            Console.ReadKey();
            return this;
        }
    }

    // 특정 직업으로 전직 가능한지 여부 판단
    private bool CanChangeClass(Player.Jobs job)
    {
        // 현재 직업과 동일하면 false
        if (GameManager.player.Job == job) return false;

        // 플레이 해 주셔서 감사합니다 << 아이템 있으면 true
        if (GameManager.player.Inventory.Any(x => x.Name == "플레이 해 주셔서 감사합니다")) return true;


        // 전직에 필요한 아이템이 존재할 경우 인벤토리에 있는지 확인
        string? needItemName = Player.JobItemName(job);
        if (needItemName != null)
        {
            return GameManager.player.Inventory.Any(x => x.Name == needItemName);
        }
        else
        {
            // 전직(Presitge) 제외한 다른 직업은 조건 없이 가능
            return job == Player.Jobs.Prestige ? false : true;
        }
    }
}