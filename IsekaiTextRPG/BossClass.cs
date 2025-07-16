using System;
using System.Collections.Generic;
using System.Linq;
namespace IsekaiTextRPG
{
    public static class BossClass
    {
        public class Skill //보스 스킬 정보 클래스
        {
            public string Name { get; }
            public float MultiplicativeFactor { get; } // 공격력 배수
            public int AdditiveBonus { get; } // 추가 피해량
            public double Chance { get; } // 발동 확률
            public int CooldownTurns { get; }  // 쿨다운 턴수
            public Skill(string name, float multiplicativeFactor, int additiveBonus, double chance, int cooldownTurns)
            {
                Name = name;
                MultiplicativeFactor = multiplicativeFactor;
                AdditiveBonus = additiveBonus;
                Chance = chance;
                CooldownTurns = cooldownTurns;
            }
        }
        public class Boss : Enemy
        {
            private static readonly Random _rng = new Random();
            public float DodgeRate { get; } // 회피 확률
            public float CriticalRate { get; } // 치명타 확률
            public float CriticalMultiplier { get; }  // 치명타 배율
            public List<Skill> Skills { get; }  // 보유 스킬 리스트
            private readonly Dictionary<Skill, int> _skillCooldowns; // 스킬 쿨타임 관리 딕셔너리
            public Boss(
                int level, string name, int hp, int attack, int defense,
                int rewardGold, int rewardExp,
                float dodgeRate, float criticalRate, float criticalMultiplier,
                IEnumerable<Skill> skills

            ) : base(level, name, hp, attack, defense, dodgeRate, rewardGold, rewardExp)

            {
                DodgeRate = dodgeRate;
                CriticalRate = criticalRate;
                CriticalMultiplier = criticalMultiplier;
                Skills = skills.ToList();
                _skillCooldowns = Skills.ToDictionary(s => s, _ => 0); // 모든 스킬 쿨타임 초기화
            }
            private void TickCooldowns() // 턴 시작 시 모든 스킬 쿨타임 1씩 감소
            {
                foreach (var skill in Skills)
                {
                    if (_skillCooldowns[skill] > 0)
                    {
                        _skillCooldowns[skill]--;
                    }
                }
            }

            // 공격 실행: 회피 확인 → 공격력 계산 → 크리티컬 확인 → 피해량 방어력비례차감 → HP 감소
            public int PerformAttack(Enemy target)
            {
                TickCooldowns();

                if (IsAttackDodged())
                    return 0;// 회피 시 공격 무효

                float attackPower = CalculateAttackPower();

                if (IsCriticalHit())
                    attackPower *= CriticalMultiplier; // 치명타 성공 시 배율 적용

                int damage = ApplyDefenseReduction(attackPower, target);
                target.CurrentHP -= damage; // 대상 HP 감소

                return damage;// 최종 데미지 반환
            }
            private bool IsAttackDodged() => _rng.NextDouble() < DodgeRate;  // 회피 여부 판정
            private float CalculateAttackPower() // 공격력 계산 (스킬이 발동하면 스킬, 아니면 기본 공격)
            {
                var availableSkills = Skills
                    .Where(s => _skillCooldowns[s] == 0 && _rng.NextDouble() < s.Chance)
                    .ToList();

                if (availableSkills.Any())
                {
                    var skill = availableSkills[_rng.Next(availableSkills.Count)];
                    _skillCooldowns[skill] = skill.CooldownTurns; // 쿨타임 설정
                    return Attack * skill.MultiplicativeFactor + skill.AdditiveBonus;
                }

                return Attack; // 스킬 발동 실패 시 기본 공격
            }
            private bool IsCriticalHit() => _rng.NextDouble() < CriticalRate;  // 크리티컬 여부 판정
            private int ApplyDefenseReduction(float attackPower, Enemy target) 
                => Math.Max(0, (int)(attackPower - target.Defense)); // 방어력 차감 후 피해량 계산
        }

        public static IReadOnlyList<Enemy> GetBossList() => new List<Enemy> 
        {
                new Boss(
                    level: 9999999,                 // 레벨
                    name: "핑크빈",                 // 보스 이름
                    hp: 9999999,                    // 최대 HP
                    attack: 9999999,                // 공격력
                    defense: 9999999,               // 방어력
                    rewardGold: 9999999,            // 보상 골드
                    rewardExp: 9999999,             // 보상 경험치
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

        private static readonly List<Skill> PinkBeanSkills = new List<Skill> //핑크빈 보스 스킬 리스트
        {
                new Skill(
                    name: "스킬이름",
                    multiplicativeFactor: 1.2f, // 공격력 20% 증가
                    additiveBonus: 0, // 공격연산이 끝난뒤 피해량에 추가    
                    chance: 0.25f, // 25% 확률로 발동
                    cooldownTurns: 3 // 3턴 쿨다운
                ),
                new Skill(
                    name: "스킬이름2",
                    multiplicativeFactor: 1.5f, // 공격력 50% 증가
                    additiveBonus: 0, // 공격연산이 끝난뒤 피해량에 추가
                    chance: 0.15f, // 15% 확률로 발동
                    cooldownTurns: 5 // 5턴 쿨다운
                )
        };
        private static readonly List<Skill> KuxseitanSkills = new List<Skill> //쿠크세이튼 보스 스킬 리스트
        {
                new Skill(
                    name: "스킬이름3",
                    multiplicativeFactor: 1.3f, // 공격력 30% 증가
                    additiveBonus: 0, // 공격연산이 끝난뒤 피해량에 추가
                    chance: 0.20f, // 20% 확률로 발동
                    cooldownTurns: 4 // 4턴 쿨다운
                ),
                new Skill(
                    name: "스킬이름4",
                    multiplicativeFactor: 1.6f, // 공격력 60% 증가
                    additiveBonus: 0, // 공격연산이 끝난뒤 피해량에 추가
                    chance: 0.10f, // 10% 확률로 발동
                    cooldownTurns: 6 // 6턴 쿨다운
                )
        };
        private static readonly List<Skill> AntonSkills = new List<Skill> // 안톤 보스 스킬 리스트
        {
                new Skill(
                    name: "스킬이름5",
                    multiplicativeFactor: 1.4f, // 공격력 40% 증가
                    additiveBonus: 0, // 공격연산이 끝난뒤 피해량에 추가
                    chance: 0.30f, // 30% 확률로 발동
                    cooldownTurns: 2 // 2턴 쿨다운
                ),
                new Skill(
                    name: "스킬이름6",
                    multiplicativeFactor: 1.7f, // 공격력 70% 증가
                    additiveBonus: 0, // 공격연산이 끝난뒤 피해량에 추가
                    chance: 0.05f, // 5% 확률로 발동
                    cooldownTurns: 7 // 7턴 쿨다운
                )
        };
    }
};
