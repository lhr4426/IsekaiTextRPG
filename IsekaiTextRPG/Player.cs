using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class Player
{
    private static readonly Random _random = new Random();

    // 플레이어 직업 종류를 열거형으로 정의
    public enum Jobs
    {
        // 0 : 환생자

        // 1 ~ 3 : 전사
        // 4 ~ 6 : 법사
        // 7 ~ 9 : 도적

        // % 3 == 1 : 로아
        // % 3 == 2 : 던파
        // % 3 == 0 : 메이플 (0 : 환생자 제외)

        Prestige,
        Warlord,
        WeaponMaster,
        Hero,
        Sorceress,
        Summoner,
        ArchMage,
        Blade,
        Rogue,
        NightLord,
    }

    // 직업 enum을 한글 직업명으로 변환하는 함수 (static)
    public static string JobsKorean(Jobs job)
    {
        return job switch
        {
            Jobs.Prestige => "환생자",
            Jobs.Warlord => "워로드",
            Jobs.WeaponMaster => "웨폰마스터",
            Jobs.Hero => "히어로",
            Jobs.Sorceress => "소서리스",
            Jobs.Summoner => "소환사",
            Jobs.ArchMage => "썬콜",
            Jobs.Blade => "블레이드",
            Jobs.Rogue => "로그",
            Jobs.NightLord => "나이트로드",
            _ => job.ToString()
        };
    }

    // 플레이어 이름
    public string Name { get; private set; }
    // 플레이어 레벨
    public int Level { get; private set; } = 1;
    // 현재 경험치
    public int Experience { get; private set; } = 0;
    // 소지 골드
    public int Gold { get; set; } = 1000;

    // 현재 직업
    public Jobs Job { get; set; } = Jobs.Prestige;

    // 최대 및 현재 체력
    public int MaxHP { get; private set; } = 100;
    public int CurrentHP { get; set; } = 100;
    // 최대 및 현재 마나
    public int MaxMP { get; private set; } = 50;
    public int CurrentMP { get; set; } = 50;

    // 기본 공격력 및 방어력
    public int BaseAttack { get; private set; } = 10;
    public int BaseDefense { get; private set; } = 5;

    // 치명타 확률, 치명타 피해 배율, 회피 확률
    public float CriticalRate { get; private set; } = 0.1f;       // 10%
    public float CriticalDamage { get; private set; } = 1.6f;    // 160%
    public float DodgeRate { get; private set; } = 0.1f;         // 10%

    // 인벤토리, 장착 아이템, 상점 판매 아이템 리스트
    public List<Item> Inventory { get; private set; } = new List<Item>();
    public List<Item> EquippedItems { get; private set; } = new List<Item>();
    public List<Item> ShopItems { get; set; } = new List<Item>();

    // 플레이어가 보유한 스킬 리스트
    public List<Skill> Skills { get; set; } = new List<Skill>();

    // 스킬 저장 파일 경로 (플레이어 이름을 포함)
    private string SkillSavePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Skills_{Name}.json");

    // 생성자 - 플레이어 이름을 받아 세팅하고 스킬을 JSON에서 불러옴
    public Player(string name)
    {
        Name = name;
        LoadSkillsFromJson();
    }

    // // 총 공격력 계산 (기본 + 장착 아이템)
    // public int GetTotalAttack()
    // {
    //     int itemBonus = EquippedItems.Sum(item => item.Attack);
    //     return BaseAttack + itemBonus;
    // }

    // // 총 방어력 계산 (기본 + 장착 아이템)
    // public int GetTotalDefense()
    // {
    //     int itemBonus = EquippedItems.Sum(item => item.Defense);
    //     return BaseDefense + itemBonus;
    // }

    // 치명타 발생 여부 판단 (랜덤)
    public bool IsCriticalHit()
    {
        return _random.NextDouble() < CriticalRate;
    }

    // 회피 성공 여부 판단 (랜덤)
    public bool TryDodge()
    {
        return _random.NextDouble() < DodgeRate;
    }

    // 경험치 획득 처리 및 레벨업 체크
    public void GainExp(int amount)
    {
        Experience += amount;
        while (Experience >= Level * 100)
        {
            Experience -= Level * 100;
            LevelUp();
        }
    }

    // 레벨업 시 능력치 상승 처리
    private void LevelUp()
    {
        Level++;
        MaxHP += 20;
        MaxMP += 10;
        BaseAttack += 2;
        BaseDefense += 1;
        CriticalRate += 0.01f;      // 1% 증가
        DodgeRate += 0.01f;         // 1% 증가
        CriticalDamage += 0.1f;     // 10% 증가

        Console.WriteLine($"\n[레벨 업!] {Level} 레벨이 되었습니다!");
    }

    // 장착 아이템이 있는지 여부 반환
    public bool HasEquippedItems()
    {
        return EquippedItems.Count > 0;
    }

    // 장착 아이템 리스트 출력, 없으면 메시지 출력
    public void ShowEquippedItems()
    {
        if (!HasEquippedItems())
        {
            Console.WriteLine("장착 중인 아이템이 없습니다.");
            return;
        }

        Console.WriteLine("장착 중인 아이템:");
        foreach (var item in EquippedItems)
        {
            Console.WriteLine($"- {item.Name} (공격력 +{item.Attack}, 방어력 +{item.Defense})");
        }
    }

    // 플레이어 상태창 출력, 장착 아이템 효과 포함
    public void ShowStatus()
    {
        int bonusAtk = EquippedItems.Where(i => i.IsEquip && i.Attack != 0).Sum(i => i.Attack);
        int bonusDef = EquippedItems.Where(i => i.IsEquip && i.Defense != 0).Sum(i => i.Defense);
        int bonusHp = EquippedItems.Where(i => i.IsEquip && i.Hp != 0).Sum(i => i.Hp);
        int bonusMp = EquippedItems.Where(i => i.IsEquip && i.Mp != 0).Sum(i => i.Mp);

        float bonusCR = EquippedItems.Where(i => i.IsEquip && i.CriticalRate != 0).Sum(i => i.CriticalRate);
        float bonusCD = EquippedItems.Where(i => i.IsEquip && i.CriticalDamage != 0).Sum(i => i.CriticalDamage);
        float bonusDR = EquippedItems.Where(i => i.IsEquip && i.DodgeRate != 0).Sum(i => i.DodgeRate);

        string atkStr = bonusAtk > 0 ? $" (+{bonusAtk})" : "";
        string defStr = bonusDef > 0 ? $" (+{bonusDef})" : "";
        string hpStr = bonusHp > 0 ? $" (+{bonusHp})" : "";
        string mpStr = bonusMp > 0 ? $" (+{bonusMp})" : "";

        string crStr = bonusCR > 0 ? $" (+{bonusCR * 100:F1}%)" : "";
        string cdStr = bonusCD > 0 ? $" (+{bonusCD * 100:F1}%)" : "";
        string drStr = bonusDR > 0 ? $" (+{bonusDR * 100:F1}%)" : "";

        List<string> strings = new List<string>()
        {
            $"Lv. {Level:D2}",
            $"{Name} ( {JobsKorean(Job)} )",
            $"공격력 : {BaseAttack + bonusAtk}{atkStr}",
            $"방어력 : {BaseDefense + bonusDef}{defStr}",
            $"체 력   : {CurrentHP + bonusHp} / {MaxHP + bonusHp}{hpStr}",
            $"마 나   : {CurrentMP + bonusMp} / {MaxMP + bonusMp}{mpStr}",
            $"치명타 확률   : {(CriticalRate + bonusCR) * 100:F1}%{crStr}",
            $"치명타 데미지 : {(CriticalDamage + bonusCD) * 100:F1}%{cdStr}",
            $"회피율        : {(DodgeRate + bonusDR) * 100:F1}%{drStr}",
            $"Gold : {Gold} G"
        };

        UI.DrawTitledBox("스테이터스", strings);
    }

    // 현재 보유한 스킬 리스트를 JSON 파일로 저장
    public void SaveSkillsToJson()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
            };

            // 스킬 객체가 아닌, 스킬 ID 리스트만 저장
            var learnedSkillIds = Skill.Select(s => s.Id).ToList();

            string json = JsonSerializer.Serialize(learnedSkillIds, options);
            string path = GetSkillSavePath();

            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] 플레이어 스킬 저장 실패: {e.Message}");
        }
    }

    // JSON 파일에서 스킬 리스트를 불러와 세팅 (초기화 포함)
    public void LoadSkillsFromJson()
    {
        string path = GetSkillSavePath();

        if (!File.Exists(path))
        {
            return; // 저장된 파일 없으면 종료
        }

        try
        {
            string json = File.ReadAllText(path);
            var skillIds = JsonSerializer.Deserialize<List<int>>(json);

            if (skillIds == null) return;

            Skill.Clear();

            foreach (int id in skillIds)
            {
                if (SkillManager.TryGetSkill(id, out Skill skill))
                {
                    skill.learnState = LearnState.Learned; // 배운 상태로 표시
                    Skill.Add(skill);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] 플레이어 스킬 불러오기 실패: {e.Message}");
        }
    }

    // JSON 저장 경로 반환 (플레이어 이름별 파일 구분용)
    private string GetSkillSavePath()
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Skills_{Name}.json");
    }
}