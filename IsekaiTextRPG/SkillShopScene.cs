using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class SkillShopScene : GameScene
{
    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SkilltreeData.json");

    public override string SceneName => "스킬 상점";

    public override GameScene? StartScene()
    {
        throw new NotImplementedException();
    }
}

