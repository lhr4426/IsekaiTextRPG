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
        PrintSkills(); // 정렬된 스킬 목록 출력

        Console.Write("배울 스킬 입력 (0 : 취소) >> ");

        int? input = InputHelper.InputNumber(0, SkillManager.Skills.Count);

        if (input == 0) return prevScene;
        else if (input.HasValue && input > 0)
        {
            LearnSkill(input.Value);
        }

        return this;
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

    private void PrintSkills()
    {
        // 스킬들을 원하는 순서대로 정렬합니다.
        var displayedSkills = SkillManager.Skills.Values.OrderBy(s =>
            s.learnState == LearnState.Learnable ? 0 :
            s.learnState == LearnState.NotLearnable ? 1 :
            2
        ).ToList();

        // 정렬된 리스트를 기반으로 화면에 출력합니다.
        List<string> strings = new List<string>();
        foreach (var skill in displayedSkills)
        {
            List<string> itemStrings = skill.ToShopString();
            foreach (var str in itemStrings)
            {
                strings.Add(str);
            }
            strings.Add("");
        }
        UI.DrawLeftAlignedBox(strings);
    }

    private void LearnSkill(int inputNumber)
    {
        int skillId = inputNumber - 1;

        if (!SkillManager.TryGetSkill(skillId, out Skill skill))
        {
            UI.DrawBox(new List<string> { "잘못된 스킬 번호입니다." });
            Console.ReadKey();
            return;
        }

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