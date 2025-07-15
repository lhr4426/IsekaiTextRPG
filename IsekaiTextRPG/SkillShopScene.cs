using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


public class SkillShopScene : GameScene
{
    private event Action? OnSkilltreeUpdated;

    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SkilltreeData.json");
    SortedDictionary<int, Skill> skills = SkillManager.Skills;

    public override string SceneName => "스킬 상점";

    public override GameScene? StartScene()
    {
        SetUpdate();
        UI.DrawTitledBox(SceneName, null);
        PrintSkills();
        Console.Write("배울 스킬 입력 (0 : 취소) >> ");

        int? input = InputHelper.InputNumber(0, skills.Count);
        if (input == 0) return prevScene;
        else if (input > 0 && input <= skills.Count)
        {
            LearnSkill((int)input - 1);
        }

        return this;
    }

    private void SetUpdate()
    {
        OnSkilltreeUpdated = null;
        foreach (var skill in skills)
        {
            OnSkilltreeUpdated += skill.Value.UpdateLearnState;
        }
        OnSkilltreeUpdated?.Invoke();
    }

    private void PrintSkills()
    {
        List<string> strings = new List<string>();
        foreach (var skill in skills)
        {
            List<string> itemStrings = skill.Value.ToShopString();
            foreach(var str in itemStrings) {
                strings.Add(str);
            }
            
            strings.Add("");
        }
        UI.DrawLeftAlignedBox(strings);
        
    }

    private void LearnSkill(int idx)
    {
        List<string> strings = new();

        if (skills[idx].learnState == LearnState.Learnable)
        {
            skills[idx].learnState = LearnState.Learned;
            GameManager.player.Skill.Add(skills[idx]);
            strings.Add($"{skills[idx].Name} 스킬을 습득하셨습니다!");
        }
        else if (skills[idx].learnState == LearnState.NotLearnable)
        {
            strings.Add($"스킬을 습득하기 위한 조건을 만족하지 않습니다.");
        }
        else
        {
            strings.Add("이미 해당 스킬을 습득하였습니다.");
        }
        UI.DrawBox(strings);
    }
}

