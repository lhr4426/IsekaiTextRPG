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
        skills = GameManager.player.Skills;
        PrintSkills();
        return EndScene();
    }

    public void PrintSkills()
    {
        List<string> strings = new List<string>();
        skills = GameManager.player.Skills;
        if (skills != null && skills.Count > 0)
        {
            for(int i = 0; i < skills.Count; i++)
            {
                strings.Add($"{i+1} : "+skills[i].ToString());

            }
        }

        else { strings.Add("사용 가능한 스킬이 없습니다."); }


        UI.DrawTitledBox(SceneName, strings);
    }
}


