using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

public static class SkillManager
{
    // 파일 경로를 SkillManager로 가져옵니다.
    private static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SkilltreeData.json");
    private static SortedDictionary<int, Skill> _skills;

    public static SortedDictionary<int, Skill> Skills => _skills;

    // 게임 시작 시 단 한번 호출될 초기화 메서드
    public static void Initialize()
    {
        LoadSkills();
    }

    private static void LoadSkills()
    {
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                // JSON에서 불러온 데이터로 _skills를 채웁니다.
                _skills = JsonSerializer.Deserialize<SortedDictionary<int, Skill>>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] 스킬 데이터 불러오기 실패: {e.Message}");
                // 로드 실패 시 기본값으로 설정
                InitializeDefaultSkills();
            }
        }
        else
        {
            // 파일이 없으면 기본 스킬들을 설정하고 파일로 저장합니다.
            InitializeDefaultSkills();
            SaveSkills();
        }
    }

    // 현재 스킬 목록을 JSON 파일로 저장하는 메서드
    public static void SaveSkills()
    {
        try
        {
            // Json 직렬화 시 한글이 깨지지 않도록 인코더 옵션을 추가합니다.
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
            };
            // JSON으로 직렬화하여 파일에 저장합니다.
            string json = JsonSerializer.Serialize(_skills, options);
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] 스킬 데이터 저장 실패: {e.Message}");
        }
    }

    // 스킬 목록을 생성하는 메서드
    // 스킬 생성 시 직업을 List로 할당
    private static void InitializeDefaultSkills()
    {
        _skills = new SortedDictionary<int, Skill>
        {
            {
                (int)SkillId.FireSword,
                new Skill(
                    id:(int)SkillId.FireSword,
                    name:"불꽃의 검",
                    damage:20,
                    manaCost:10,
                    description:"불꽃을 담은 검으로 적에게 큰 피해를 줍니다.")
                {
                    NeedLevel = 5, // 스킬을 배우기 위한 최소 레벨
                    // new List<Player.Jobs> { ... } 형태로 할당합니다.
                    NeedJob = new List<Player.Jobs> { Player.Jobs.Warlord, Player.Jobs.WeaponMaster, Player.Jobs.Hero } // 필요한 직업 목록
                }
            },
            {
                (int)SkillId.EarthQuake,
                new Skill(
                    id:         (int)SkillId.EarthQuake,
                    name:       "대지의 분노",
                    damage:     30,
                    manaCost:   15,
                    description: "대지를 흔들어 적에게 큰 피해를 줍니다.")
                {
                    NeedLevel = 10,
                    NeedJob = new List<Player.Jobs> { Player.Jobs.Warlord, Player.Jobs.WeaponMaster, Player.Jobs.Hero }
                }
            },
            {
               (int)SkillId.IceSpear,
                new Skill(
                    id:         (int)SkillId.IceSpear,
                    name:       "얼음 창",
                    damage:     15,
                    manaCost:   8,
                    description: "얼음으로 만들어진 창을 던져 적을 얼립니다.")
                {
                    NeedLevel = 5,
                    NeedJob = new List<Player.Jobs> { Player.Jobs.Sorceress, Player.Jobs.Summoner, Player.Jobs.ArchMage }
                }
            },
            {
                (int)SkillId.LightningStrike,
                new Skill(
                    id:          (int)SkillId.LightningStrike,
                    name:        "번개의 일격",
                    damage:      25,
                    manaCost:    12,
                    description:  "하늘에서 번개를 내려쳐 적에게 큰 피해를 줍니다.")
                {
                    NeedLevel = 10,
                    NeedJob = new List<Player.Jobs> { Player.Jobs.Sorceress, Player.Jobs.Summoner, Player.Jobs.ArchMage }
                }
            },
            {
                (int)SkillId.WindBlade,
                new Skill(
                    id:         (int)SkillId.WindBlade,
                    name:       "바람의 칼날",
                    damage:     18,
                    manaCost:   7,
                    description: "바람을 날카롭게 만들어 적을 베어냅니다.")
                {
                    NeedLevel = 5,
                    NeedJob = new List<Player.Jobs> { Player.Jobs.Blade, Player.Jobs.Rogue, Player.Jobs.NightLord }
                }
            },
            {
                (int)SkillId.ShadowStep,
                new Skill(
                    id:         (int)SkillId.ShadowStep,
                    name:       "그림자 걸음",
                    damage:     5,
                    manaCost:   5,
                    description: "그림자의 힘을 빌려 빠르게 적 뒤로 이동합니다.")
                {
                    NeedLevel = 3,
                    NeedJob = new List<Player.Jobs> { Player.Jobs.Blade, Player.Jobs.Rogue, Player.Jobs.NightLord }
                }
            }
        };
    }

    public static bool TryGetSkill(int id, out Skill? skill) => _skills.TryGetValue(id, out skill);
}