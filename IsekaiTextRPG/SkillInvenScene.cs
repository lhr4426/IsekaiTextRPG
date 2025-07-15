using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SkillInvenScene : GameScene
{
    List<Skill> skills;
    public override string SceneName => "스킬트리";

    public override GameScene? StartScene()
    {
        skills = GameManager.player.Skill;
        PrintSkills();
        return EndScene();
    }

    public void PrintSkills()
    {
        List<string> strings = new List<string>();

        foreach(var skill in skills)
        {
            strings.Add(skill.ToString());
        }

        UI.DrawTitledBox(SceneName, strings);
    }
}


