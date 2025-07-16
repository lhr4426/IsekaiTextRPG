using System;
using System.Collections.Generic;
using System.Linq;
namespace IsekaiTextRPG
{
    public static class BossClass
    {
        public class Boss : Enemy
        {
            private static readonly Random rng = new Random();
            public float DodgeRate { get; }
            public float CriticalRate { get; }
            public float CriticalDamageMultiplier { get; }
            public List<BossSkill> Skills { get; }
            private readonly Dictionary<Skill, int> skillCooldowns;

            public Boss(
                int level,
                string name,
                int hp,
                int attack,
                int defense,
                int rewardGold,
                int rewardExp,
                float dodgeRate,
                float criticalRate,
                float criticalDamageMultiplier,
                IEnumerable<BossSkill> skills
            ) : base(level, name, hp, attack, defense, rewardGold, rewardExp)
            {
                DodgeRate = dodgeRate;
                CriticalRate = criticalRate;
                CriticalDamageMultiplier = criticalDamageMultiplier;
                Skills = skills.ToList();
                skillCooldowns = Skills.ToDictionary(sk => sk, sk => 0);

            }

            private void TickCooldowns()
            {
                foreach (var sk in Skills.ToList())
                {
                    if (skillCooldowns[sk] > 0)
                    {
                        skillCooldowns[sk]--;
                    }
                }
            }

            public int PerformAction(Player target, out string description)
            {
                TickCooldowns();
                if (target.TryDodge())
                {
                    description = $"{Name} → {target.Name}: 회피!";
                    return 0; // 공격이 피함
                }

                var available = Skills
                    .Where(s => skillCooldowns[s] <= 0 && rng.NextDouble() < s.Chance)
                    .ToList();
                if (available.Any)
                {
                    var skill = available[rng.Next(available.Count)];
                    int dmg = skill.Execute(this, target);
                    skillCooldowns[skill] = (int)skill.Cooldown; // 쿨다운 설정
                    if (dmg > 0)
                        desription = $"{Name}의 {skill.Name} 이 {target.Name}에게 {dmg}의 피해를 입힘!";
                    else
                        desription = $"{Name}의 {skill.Name} 이 실패!";
                    return dmg;
                }

                bool isCrit = rng.NextDouble() < CriticalRate;
                int baseDmg = Attack;
                int finalDmg = isCrit ? (int)(baseDmg * CriticalDamageMultiplier) : baseDmg;
            }
            public static IReadOnlyList<Enemy> GetBossList() => new List<Enemy>
            {
                new Boss(
                    level: 9999999,
                    name: "핑크빈",
                    hp: 9999999,
                    attack: 9999999,
                    defense: 9999999,
                    rewardGold: 9999999,
                    rewardExp: 9999999,
                    dodgeRate: 0.10f,               // 10% 회피율
                    criticalRate: 0.20f,            // 20% 크리티컬 확률
                    criticalDamageMultiplier: 1.50f// 150% 크리티컬 데미지 
                ),
                new Boss(
                    level: 9999999,
                    name: "쿠크세이튼",
                    hp: 9999999,
                    attack: 9999999,
                    defense: 9999999,
                    rewardGold: 9999999,
                    rewardExp: 9999999,
                    dodgeRate: 0.05f,
                    criticalRate: 0.15f,
                    criticalDamageMultiplier: 1.40f
                ),
                new Boss(
                    level: 9999999,
                    name: "안톤",
                    hp: 9999999,
                    attack: 9999999,
                    defense: 9999999,
                    rewardGold: 9999999,
                    rewardExp: 9999999,
                    dodgeRate: 0.07f,
                    criticalRate: 0.18f,
                    criticalDamageMultiplier: 1.60f
                )
            };
            public class BossSkill
            {
                public string Name { get; }
                public int Damage { get; }
                public double Chance { get; }
                public float Cooldown { get; }
                public string Description { get; }
                public Func<Boss, Player, int> Execute { get; }

                public BossSkill(
                    string name,
                    int damage,
                    double chance,
                    float cooldown,
                    string description,
                    Func<Boss, Player, int> execute
                )
                {
                    Name = name;
                    Damage = damage;
                    Chance = chance;
                    Cooldown = cooldown;
                    Description = description;
                    Execute = execute;
                }
            }
        }
    } 
}

