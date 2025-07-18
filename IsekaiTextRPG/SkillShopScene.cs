using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

public class SkillShopScene : GameScene
{
    private event Action? OnSkilltreeUpdated;

    public override string SceneName => "스킬 상점";

    public override GameScene? StartScene()
    {
        Console.Clear();
        CheckDidYouLearn();
        SetUpdate();

        UI.DrawTitledBox(SceneName, null);
        RunSkillShopUI(); // 병합된 UI 함수 실행

        return prevScene;
    }

    private void CheckDidYouLearn()
    {
        var learnedSkillIds = new HashSet<int>(GameManager.player.Skills.Select(s => s.Id));
        foreach (var skill in SkillManager.Skills.Values)
        {
            if (learnedSkillIds.Contains(skill.Id))
            {
                skill.learnState = LearnState.Learned;
            }
        }
    }

    private void SetUpdate()
    {
        OnSkilltreeUpdated = null;
        foreach (var skill in SkillManager.Skills.Values)
        {
            OnSkilltreeUpdated += skill.UpdateLearnState;
        }
        OnSkilltreeUpdated?.Invoke();
    }

    private void RunSkillShopUI()
    {
        const int pageSize = 7;
        var displayedSkills = SkillManager.Skills.Values.OrderBy(s =>
          s.learnState == LearnState.Learnable ? 0 :
          s.learnState == LearnState.NotLearnable ? 1 : 2
        ).ToList();

        int totalPages = (int)Math.Ceiling(displayedSkills.Count / (double)pageSize);
        int currentPage = 0;

        while (true)
        {
            Console.Clear();
            List<string> strings = new List<string>();

            int startIndex = currentPage * pageSize;
            int endIndex = Math.Min(startIndex + pageSize, displayedSkills.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                Skill skill = displayedSkills[i];
                var itemStrings = skill.ToShopString();
                string firstLine = itemStrings[0];
                int dot = firstLine.IndexOf('.');
                string rest = dot >= 0 ? firstLine.Substring(dot + 1) : firstLine;
                itemStrings[0] = $"{i + 1}.{rest}";
                strings.AddRange(itemStrings);
                strings.Add("");
            }

            strings.Add($"[Page {currentPage + 1} / {totalPages}]");
            strings.Add("N: 다음 | P: 이전 | 번호 입력: 배우기 | Q: 종료");
            UI.DrawLeftAlignedBox(strings);

            Console.Write("입력 ▶ ");
            string input = Console.ReadLine()?.Trim().ToUpper() ?? "";

            if (input == "N")
            {
                if (currentPage < totalPages - 1) currentPage++;
                else { UI.DrawBox(new List<string> { "이미 마지막 페이지입니다." }); Console.ReadKey(true); }
                continue;
            }
            if (input == "P")
            {
                if (currentPage > 0) currentPage--;
                else { UI.DrawBox(new List<string> { "이미 첫 페이지입니다." }); Console.ReadKey(true); }
                continue;
            }
            if (input == "Q")
            {
                break;
            }

            if (int.TryParse(input, out int num))
            {
                int selected = num - 1;
                if (selected >= startIndex && selected < endIndex)
                {
                    LearnSkill(displayedSkills[selected]);
                }
                else
                {
                    UI.DrawBox(new List<string> { $"유효한 번호는 {startIndex + 1}~{endIndex}입니다." });
                    Console.ReadKey(true);
                }
                continue;
            }

            UI.DrawBox(new List<string> { "잘못된 입력입니다. 다시 시도하세요." });
            Console.ReadKey(true);
        }
    }

    private void LearnSkill(Skill skill)
    {
        List<string> strings = new();

        if (skill.learnState == LearnState.Learnable)
        {
            skill.learnState = LearnState.Learned;
            GameManager.player.Skills.Add(skill);
            strings.Add($"{skill.Name} 스킬을 습득하셨습니다!");
            GameManager.player.SaveSkillsToJson();
        }
        else if (skill.learnState == LearnState.NotLearnable)
        {
            strings.Add("스킬을 습득하기 위한 조건을 만족하지 않습니다.");
        }
        else
        {
            strings.Add("이미 해당 스킬을 습득하였습니다.");
        }

        UI.DrawBox(strings);
        Console.ReadKey();
    }
}
