using System;
using System.Collections.Generic;
using System.Linq;

public class JeonJikScene : GameScene
{
    public override string SceneName => "전직소";

    public override GameScene? StartScene()
    {
        Console.Clear();
        PrintJobs();
        return ChangeClass();
    }

    // 전직 가능한 직업 목록을 화면에 출력
    public void PrintJobs()
    {
        List<string> strings = new();

        var JobsArray = Enum.GetValues(typeof(Player.Jobs))
                            .Cast<Player.Jobs>()
                            .Where(j => j != Player.Jobs.Prestige)
                            .ToList();

        strings.Add("[직업 목록]");

        for (int i = 0; i < JobsArray.Count; i++)
        {
            Player.Jobs job = JobsArray[i];
            string needItemText = "";
            string? requiredItem = Player.JobItemName(job);
            if (requiredItem != null)
            {
                needItemText = $"(필요: {requiredItem})";
            }

            string currentJobMark = (GameManager.player.Job == job) ? "[현재 직업]" : "";

            strings.Add($"{i + 1}. {Player.JobsKorean(job),-10} | {currentJobMark,-10} | {(CanChangeClass(job) ? "전직 가능" : "전직 불가")} {needItemText}");
        }

        UI.DrawTitledBox(SceneName, null);
        UI.DrawLeftAlignedBox(strings);
        Console.Write("전직할 직업을 선택 (0 : 돌아가기) >> ");
    }

    // 플레이어의 직업을 변경하는 처리
    private GameScene? ChangeClass()
    {
        List<string> strings = new();

        var JobsArray = Enum.GetValues(typeof(Player.Jobs))
                            .Cast<Player.Jobs>()
                            .Where(j => j != Player.Jobs.Prestige)
                            .ToList();

        int? input = InputHelper.InputNumber(0, JobsArray.Count);

        if (input == 0) return prevScene; // 0 입력 시 이전 씬으로 돌아가기

        if (input > 0 && input <= JobsArray.Count)
        {
            var selectedJob = JobsArray[(int)input - 1];

            // 이미 해당 직업이라면 전직 불가능
            if (GameManager.player.Job == selectedJob)
            {
                strings.Add($"이미 {Player.JobsKorean(selectedJob)} 직업입니다.");
                UI.DrawBox(strings);
                Console.ReadKey();
                return this;
            }

            // 전직 가능 여부 확인 (아이템 소유 여부 포함)
            if (CanChangeClass(selectedJob))
            {
                // 전직 아이템이 필요하다면 인벤토리에서 제거
                string? needItemName = Player.JobItemName(selectedJob);
                if (needItemName != null)
                {
                    // 해당 아이템을 인벤토리에서 찾아서 제거
                    Item? itemToRemove = GameManager.player.Inventory.FirstOrDefault(x => x.Name == needItemName);
                    if (itemToRemove != null)
                    {
                        GameManager.player.Inventory.Remove(itemToRemove);
                        strings.Add($"'{itemToRemove.Name}'을(를) 사용하여 전직했습니다.");
                    }
                }

                // 플레이어 직업 변경
                GameManager.player.Job = selectedJob;
                strings.Add($"'{Player.JobsKorean(selectedJob)}' 직업으로 전직하였습니다!");
                UI.DrawBox(strings);
                Console.ReadKey();
                return prevScene;
            }
            else // 전직 불가능 시 (아이템 부족 등)
            {
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
            Console.WriteLine("잘못된 입력입니다. 아무 키나 눌러 계속...");
            Console.ReadKey();
            return this;
        }
    }

    // 특정 직업으로 전직 가능한지 여부 확인
    private bool CanChangeClass(Player.Jobs job)
    {
        string? needItemName = Player.JobItemName(job); // 해당 직업에 필요한 아이템 이름

        // 현재 플레이어의 직업과 같으면 전직 불가
        if (GameManager.player.Job == job)
        {
            return false;
        }

        // 2. 전직 아이템이 필요한 직업인 경우
        if (needItemName != null)
        {
            // 플레이어 인벤토리에 해당 아이템이 있는지 확인
            bool hasItem = GameManager.player.Inventory.Any(x => x.Name == needItemName);
            return hasItem;
        }
        else // 3. 전직 아이템이 필요 없는 직업 (Prestige)
        {
            return job == Player.Jobs.Prestige ? false : true;
        }
    }
}