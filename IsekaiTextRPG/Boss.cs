using System;
using System.Collections.Generic;
using System.Linq;
namespace IsekaiTextRPG
{
    public static class BossClass
    {
        public class Skill
        {
            public string Name { get; }
            public float MultiplicativeFactor { get; }
            public int AdditiveBonus { get; }
            public double Chance { get; }
            public int CooldownTurns { get; }

            public Skill(string name, float multiplicativeFactor, int additiveBonus, double chance, int cooldownTurns)
            {
                Name = name;
                MultiplicativeFactor = multiplicativeFactor;
                AdditiveBonus = additiveBonus;
                Chance = chance;
                CooldownTurns = cooldownTurns;
            }
        }

        // 2) 보스 클래스: 회피 → 스킬/기본 공격 → 크리티컬 → 방어력 차감 순으로 계산
        public class Boss : Enemy
        {
            private static readonly Random rng = new Random();

            public float DodgeRate { get; }
            public float CriticalRate { get; }
            public float CriticalMultiplier { get; }
            public List<Skill> Skills { get; }
            private readonly Dictionary<Skill, int> skillCooldowns;

            public Boss(
                int level, string name, int hp, int attack, int defense,
                int rewardGold, int rewardExp,
                float dodgeRate, float criticalRate, float criticalMultiplier,
                IEnumerable<Skill> skills
            ) : base(level, name, hp, attack, defense, rewardGold, rewardExp)
            {
                DodgeRate = dodgeRate;
                CriticalRate = criticalRate;
                CriticalMultiplier = criticalMultiplier;
                Skills = skills.ToList();
                skillCooldowns = Skills.ToDictionary(s => s, _ => 0);
            }

            // 매 턴 쿨다운 감소
            private void TickCooldowns()
            {
                foreach (var sk in Skills)
                    if (skillCooldowns[sk] > 0)
                        skillCooldowns[sk]--;
            }
            //공격연산: 회피 → 스킬/기본 공격 → 크리티컬 → 최종공격력 - 상대방어력 = 최종피해량
            public int PerformAttack(Enemy target)
            {
                TickCooldowns();
                if (rng.NextDouble() < DodgeRate)
                {
                    return 0; // 공격 회피
                }

                float calculatedAttack = Attack;
                var available = Skills
                    .Where(s => skillCooldowns[s] == 0 && rng.NextDouble() < s.Chance)
                    .ToList();

                if (available.Any())
                {
                    var skill = available[rng.Next(available.Count)];
                    calculatedAttack = Attack * skill.MultiplicativeFactor + skill.AdditiveBonus;
                    skillCooldowns[skill] = skill.CooldownTurns; // 스킬 쿨다운 설정

                }

                if (rng.NextDouble() < CriticalRate)
                {
                    calculatedAttack *= CriticalMultiplier; // 크리티컬 공격
                }

                int damage = Math.Max(0, (int)(calculatedAttack - target.Defense));
                target.CurrentHP -= damage;
                return damage;
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
                    criticalMultiplier: 1.50f,      // 150% 크리티컬 데미지 
                    skills: PinkBeanSkills
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
                    criticalMultiplier: 1.40f,
                    skills: KuxseitanSkills
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
                    criticalMultiplier: 1.60f,
                    skills: AntonSkills
                )
            };
           
            private static readonly List<Skill> PinkBeanSkills = new List<Skill>
            {
                new Skill(
                    name: "스킬이름",
                    multiplicativeFactor: 1.2f, // 공격력 20% 증가
                    additiveBonus: 0, // 공격연산이 끝난뒤 피해량에 추가    
                    chance: 0.25, // 25% 확률로 발동
                    cooldownTurns: 3 // 3턴 쿨다운
                ),
                new Skill(
                    name: "스킬이름2",
                    multiplicativeFactor: 1.5f, // 공격력 50% 증가
                    additiveBonus: 0, // 공격연산이 끝난뒤 피해량에 추가
                    chance: 0.15, // 15% 확률로 발동
                    cooldownTurns: 5 // 5턴 쿨다운
                )
            };
            private static readonly List<Skill> KuxseitanSkills = new List<Skill>
            {
                new Skill(
                    name: "스킬이름3",
                    multiplicativeFactor: 1.3f, // 공격력 30% 증가
                    additiveBonus: 0, // 공격연산이 끝난뒤 피해량에 추가
                    chance: 0.20, // 20% 확률로 발동
                    cooldownTurns: 4 // 4턴 쿨다운
                ),
                new Skill(
                    name: "스킬이름4",
                    multiplicativeFactor: 1.6f, // 공격력 60% 증가
                    additiveBonus: 0, // 공격연산이 끝난뒤 피해량에 추가
                    chance: 0.10, // 10% 확률로 발동
                    cooldownTurns: 6 // 6턴 쿨다운
                )
            };
            private static readonly List<Skill> AntonSkills = new List<Skill>
            {
                new Skill(
                    name: "스킬이름5",
                    multiplicativeFactor: 1.4f, // 공격력 40% 증가
                    additiveBonus: 0, // 공격연산이 끝난뒤 피해량에 추가
                    chance: 0.30, // 30% 확률로 발동
                    cooldownTurns: 2 // 2턴 쿨다운
                ),
                new Skill(
                    name: "스킬이름6",
                    multiplicativeFactor: 1.7f, // 공격력 70% 증가
                    additiveBonus: 0, // 공격연산이 끝난뒤 피해량에 추가
                    chance: 0.05, // 5% 확률로 발동
                    cooldownTurns: 7 // 7턴 쿨다운
                )
            };

        }
    } 
}

