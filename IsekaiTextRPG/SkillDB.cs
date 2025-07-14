using System;
using System.Collections.Generic;

public enum SkillId
{
    FireSword = 0,
    IceSpear = 1,
    LightningStrike = 2,
    WindBlade = 3
}
public class SkillDB
{

    public string Name { get; }
    public int Id { get; private set; }
    public int Damage { get; }
    public string Description { get; }
    public int ManaCost { get; }
    public int Cooldown { get; }
    public SkillDB(int id, string name, int damage, int manaCost, int cooldown, string description)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Damage = damage;
        ManaCost = manaCost;
        Cooldown = cooldown;
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    public static class SkillManager
    {
        private static readonly Dictionary<int, SkillDB> _skills = new Dictionary<int, SkillDB>
            {

                {
                    (int)SkillId.FireSword,
                    new SkillDB(
                        id:(int)SkillId.FireSword,
                        name:"불꽃의 검",
                        damage:20,
                        manaCost:10,
                        cooldown:5,
                        description:"불꽃을 담은 검으로 적에게 큰 피해를 줍니다.")
                },
                {
                   (int)SkillId.IceSpear,
                    new SkillDB(
                    id:          (int)SkillId.IceSpear,
                    name:        "얼음 창",
                    damage:      15,
                    manaCost:     8,
                    cooldown:     4,
                    description: "얼음으로 만들어진 창을 던져 적을 얼립니다.")
                },
                {
                   (int)SkillId.WindBlade,
                    new SkillDB(
                    id:          (int)SkillId.WindBlade,
                    name:        "바람의 칼날",
                    damage:      18,
                    manaCost:     7,
                    cooldown:     3,
                    description: "바람을 날카롭게 만들어 적을 베어냅니다.")
                },
            };

        public static IReadOnlyDictionary<int, SkillDB> Skills => _skills;

        public static bool TryGetSkill(int id, out SkillDB? skill) => _skills.TryGetValue(id, out skill);
    }
}
