using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class SkillManager
{
    // TODO : 스킬마다 직업이랑 레벨 설정 부탁드려요
    private static readonly Dictionary<int, Skill> _skills = new Dictionary<int, Skill>
            {

                {
                    (int)SkillId.FireSword,
                    new Skill(
                        id:(int)SkillId.FireSword,
                        name:"불꽃의 검",
                        damage:20,
                        manaCost:10,
                        //cooldown:5,
                        description:"불꽃을 담은 검으로 적에게 큰 피해를 줍니다.")
                },
                {
                   (int)SkillId.IceSpear,
                    new Skill(
                    id:          (int)SkillId.IceSpear,
                    name:        "얼음 창",
                    damage:      15,
                    manaCost:     8,
                    //cooldown:     4,
                    description: "얼음으로 만들어진 창을 던져 적을 얼립니다.")
                },
                {
                    (int)SkillId.LightningStrike,
                     new Skill(
                     id:           (int)SkillId.LightningStrike,
                     name:         "번개의 일격",
                     damage:       25,
                     manaCost:     12,
                     //cooldown:   6,
                     description:  "하늘에서 번개를 내려쳐 적에게 큰 피해를 줍니다.")
                },
                {
                   (int)SkillId.WindBlade,
                    new Skill(
                    id:          (int)SkillId.WindBlade,
                    name:        "바람의 칼날",
                    damage:      18,
                    manaCost:     7,
                    //cooldown:     3,
                    description: "바람을 날카롭게 만들어 적을 베어냅니다.")
                },
            };

    public static IReadOnlyDictionary<int, Skill> Skills => _skills;

    public static bool TryGetSkill(int id, out Skill? skill) => _skills.TryGetValue(id, out skill);

}
