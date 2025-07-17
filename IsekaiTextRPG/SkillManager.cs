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
                    NeedLevel = 20,
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
            },
            {
                (int)SkillId.ComboDeathFault,
                new Skill(
                    id: (int)SkillId.ComboDeathFault,
                    name: "콤보 데스폴트",
                    damage: 150,
                    manaCost: 150,
                    cooldown: 180,
                    description: "검으로 공간을 가릅니다.")
                {
                    NeedLevel = 20,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.Hero }
                }
            },
            {
                (int)SkillId.Doomsday,
                new Skill(
                    id: (int)SkillId.Doomsday,
                    name: "종말의 날",
                    damage: 200,
                    manaCost: 300,
                    cooldown: 180,
                    description: "매우 거대한 운석을 떨어뜨립니다.")
                {
                    NeedLevel = 20,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.Sorceress }
                }
            },
            {
                (int)SkillId.AncientSpear,
                new Skill(
                    id: (int)SkillId.AncientSpear,
                    name: "고대의 창",
                    damage: 200,
                    manaCost: 300,
                    cooldown: 180,
                    description: "고대의 힘을 담은 창을 소환하여 떨어뜨립니다.")
                {
                    NeedLevel = 20,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.Summoner }
                }
            },
            {
                (int)SkillId.Blizzard,
                new Skill(
                    id: (int)SkillId.Blizzard,
                    name: "블리자드",
                    damage: 200,
                    manaCost: 300,
                    cooldown: 180,
                    description: "하늘로부터 얼음의 창을 떨어뜨립니다.")
                {
                    NeedLevel = 20,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.ArchMage }
                }
            },
            {
                (int)SkillId.BladeArts,
                new Skill(
                    id: (int)SkillId.BladeArts,
                    name: "블레이드아츠",
                    damage: 500,
                    manaCost: 1000,
                    cooldown: 300,
                    description: "여러검술을 합친 단한번의 일격입니다.")
                {
                    NeedLevel = 20,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.Blade }
                }
            },
            {
                (int)SkillId.NovaRemnant,
                new Skill(
                    id: (int)SkillId.NovaRemnant,
                    name: "노바 램넌트",
                    damage: 300,
                    manaCost: 200,
                    cooldown: 140,
                    description: "한줄기의 빛이되어 강력한 내려찍기공격을 한다.")
                {
                    NeedLevel = 20,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.Rogue }
                }
            },
            {
                (int)SkillId.QuadrupleThrow,
                new Skill(
                    id: (int)SkillId.QuadrupleThrow,
                    name: "쿼드러플 스로우",
                    damage: 180,
                    manaCost: 50,
                    cooldown: 60,
                    description: "4개의 표창을 연속으로 던진다.")
                {
                    NeedLevel = 20,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.NightLord }
                }
            },
            {
                (int)SkillId.ReadytoDie,
                new Skill(
                    id: (int)SkillId.ReadytoDie,
                    name: "Ready to Die",
                    damage: 1000,
                    manaCost: 500,
                    cooldown: 600,
                    description: "Ready to Die.....")
                {
                    NeedLevel = 30,
                    NeedJobs = new List<Player.Jobs> { Player.Jobs.NightLord, Player.Jobs.Blade, Player.Jobs.Rogue }
                }
            }
        };
    }

    // ID로 스킬을 가져오는 메서드 (존재 여부 반환)
    public static bool TryGetSkill(int id, out Skill? skill) => _skills.TryGetValue(id, out skill);
}