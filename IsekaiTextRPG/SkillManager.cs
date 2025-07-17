using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

public static class SkillManager
{
    private static SortedDictionary<int, Skill> _skills;
    public static SortedDictionary<int, Skill> Skills => _skills;

    // 게임 시작 시 단 한 번 호출될 초기화 메서드
    public static void Initialize()
    {
        InitializeDefaultSkills();
    }

    // 기본 스킬 목록을 생성하는 메서드
    private static void InitializeDefaultSkills()
    {
        _skills = new SortedDictionary<int, Skill>
        {
            {(int)SkillId.FocusEvasion,
                new Skill(
                    id: (int)SkillId.FocusEvasion,
                    name: "돌던지기",
                    damage: 10,
                    manaCost: 0,
                    cooldown: 5,
                    description: "매우큰 짱돌을 던집니다.")
                {
                    NeedLevel = 0,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.Prestige} 
                }
            },
            {
                (int)SkillId.FireSword,
                new Skill(
                    id: (int)SkillId.FireSword,
                    name: "불꽃의 검",
                    damage: 20,
                    manaCost: 10,
                    cooldown: 20,
                    description: "불꽃을 담은 검으로 적에게 큰 피해를 줍니다.")
                {
                    NeedLevel = 0,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.Warlord, Player.Jobs.WeaponMaster, Player.Jobs.Hero }
                }
            },
            {
                (int)SkillId.EarthQuake,
                new Skill(
                    id: (int)SkillId.EarthQuake,
                    name: "대지의 분노",
                    damage: 50,
                    manaCost: 40,
                    cooldown: 60,
                    description: "대지를 흔들어 적에게 큰 피해를 줍니다.")
                {
                    NeedLevel = 10,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.Warlord, Player.Jobs.WeaponMaster, Player.Jobs.Hero }
                }
            },
            {
                (int)SkillId.IceSpear,
                new Skill(
                    id: (int)SkillId.IceSpear,
                    name: "얼음 창",
                    damage: 20,
                    manaCost: 10,
                    cooldown: 20,
                    description: "얼음으로 만들어진 창을 던져 적을 얼립니다.")
                {
                    NeedLevel = 0,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.Sorceress, Player.Jobs.Summoner, Player.Jobs.ArchMage }
                }
            },
            {
                (int)SkillId.LightningStrike,
                new Skill(
                    id: (int)SkillId.LightningStrike,
                    name: "번개의 일격",
                    damage: 55,
                    manaCost: 40,
                    cooldown: 60,
                    description: "하늘에서 번개를 내려쳐 적에게 큰 피해를 줍니다.")
                {
                    NeedLevel = 10,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.Sorceress, Player.Jobs.Summoner, Player.Jobs.ArchMage }
                }
            },
            {
                (int)SkillId.WindBlade,
                new Skill(
                    id: (int)SkillId.WindBlade,
                    name: "바람의 칼날",
                    damage: 20,
                    manaCost: 10,
                    cooldown: 20,
                    description: "바람을 날카롭게 만들어 적을 베어냅니다.")
                {
                    NeedLevel = 0,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.Blade, Player.Jobs.Rogue, Player.Jobs.NightLord }
                }
            },
            {
                (int)SkillId.ShadowStep,
                new Skill(
                    id: (int)SkillId.ShadowStep,
                    name: "그림자 도약",
                    damage: 55,
                    manaCost: 40,
                    cooldown: 60,
                    description: "그림자의 힘을 빌려 빠르게 적 뒤로 이동하고 적에게 큰 피해를 입힙니다.")
                {
                    NeedLevel = 3,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.Blade, Player.Jobs.Rogue, Player.Jobs.NightLord }
                }
            },
            {
                (int)SkillId.GuardianLightning,
                new Skill(
                    id: (int)SkillId.GuardianLightning,
                    name: "가디언의 낙뢰",
                    damage: 40,
                    manaCost: 40,
                    cooldown: 20,
                    description: "하늘에서 낙뢰를 떨어뜨립니다")
                {
                    NeedLevel = 15,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.Warlord }
                }
            },
            {
                (int)SkillId.PhantomBladeDance,
                new Skill(
                    id: (int)SkillId.PhantomBladeDance,
                    name: "환영검무",
                    damage: 100,
                    manaCost: 100,
                    cooldown: 120,
                    description: "환영의 칼날로 적을 마구베어버립니다.")
                { 
                    NeedLevel = 20,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.WeaponMaster }
                }
            }


        };
    }

    // ID로 스킬을 가져오는 메서드 (존재 여부 반환)
    public static bool TryGetSkill(int id, out Skill? skill) => _skills.TryGetValue(id, out skill);
}