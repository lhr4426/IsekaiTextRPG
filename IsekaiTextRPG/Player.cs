using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json; // JSON 직렬화/역직렬화에 사용

public class Player
{
    private static readonly Random _random = new Random(); // 랜덤 값 생성을 위한 객체

    // 플레이어 직업 종류를 나타내는 열거형
    public enum Jobs
    {
        Prestige,       // 환생자
        Warlord,        // 워로드
        WeaponMaster,   // 웨폰마스터
        Hero,           // 히어로
        Sorceress,      // 소서리스
        Summoner,       // 소환사
        ArchMage,       // 썬콜
        Blade,          // 블레이드
        Rogue,          // 로그
        NightLord,      // 나이트로드
    }

    // 직업 Enum 값을 한글 직업명으로 변환
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

    // 직업별 추천/필요 아이템 이름을 반환 (선택적)
    public static string? JobItemName(Jobs job)
    {
        return job switch
        {
            // 1번 아이템: 히어로, 썬콜, 나이트로드
            Jobs.Hero => "전직의 증표 I",
            Jobs.ArchMage => "전직의 증표 I",
            Jobs.NightLord => "전직의 증표 I",

            // 2번 아이템: 워로드, 소서리스, 블레이드
            Jobs.Warlord => "전직의 증표 II",
            Jobs.Sorceress => "전직의 증표 II",
            Jobs.Blade => "전직의 증표 II",

            // 3번 아이템: 워폰마스터, 소환사, 로그
            Jobs.WeaponMaster => "전직의 증표 III",
            Jobs.Summoner => "전직의 증표 III",
            Jobs.Rogue => "전직의 증표 III",

            _ => null // Prestige(환생자) 등 전직 아이템이 필요 없는 직업은 null 반환
        };
    }

    public string Name { get; set; } // 플레이어 이름

    private int _level; // 플레이어 레벨 (내부 변수)
    public int Level
    {
        get { return _level; }
        set
        {
            if (_level != value) // 레벨이 실제로 변경될 때만 동작
            {
                _level = value;
                QuestManager.UpdateLevelQuestProgress(_level);
            }
        }
    }

    public int Experience { get; set; } = 0; // 현재 경험치
    public int Gold { get; set; } = 1000; // 보유 골드
    public Jobs Job { get; set; } = Jobs.Prestige; // 플레이어 직업

    // 기본 체력 및 현재 체력 (아이템 효과가 이 값에 직접 반영됨)
    public int MaxHP { get; set; } = 10;
    public int CurrentHP { get; set; } = 10;

    // 기본 마나 및 현재 마나 (아이템 효과가 이 값에 직접 반영됨)
    public int MaxMP { get; set; } = 50;
    public int CurrentMP { get; set; } = 50;

    // 기본 공격력 (아이템 효과가 이 값에 직접 반영됨)
    public int BaseAttack { get; set; } = 10;
    // 기본 방어력 (아이템 효과가 이 값에 직접 반영됨)
    public int BaseDefense { get; set; } = 5;

    // 기본 치명타 확률 (아이템 효과가 이 값에 직접 반영됨)
    public float CriticalRate { get; set; } = 0.1f;
    // 기본 치명타 데미지 배율 (아이템 효과가 이 값에 직접 반영됨)
    public float CriticalDamage { get; set; } = 1.6f;
    // 기본 회피율 (아이템 효과가 이 값에 직접 반영됨)
    public float DodgeRate { get; set; } = 0.1f;

    // 아이템 장착 여부와 관계없이 플레이어가 소유한 모든 아이템 목록
    public List<Item> Inventory { get; set; } = new List<Item>();
    // 현재 플레이어가 장착 중인 아이템 목록
    public List<Item> EquippedItems { get; set; } = new List<Item>();
    // 상점 아이템 목록 (플레이어 클래스보다는 ShopManager 등에서 관리하는 것이 일반적)
    public List<Item> ShopItems { get; set; } = new List<Item>();

    public List<Skill> Skills { get; set; } = new List<Skill>(); // 보유 스킬 목록

    // 스킬 저장 파일 경로 (플레이어 이름에 따라 고유하게 생성)
    private string SkillSavePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Skills_{Name}.json");
    // 현재 진행 중인 퀘스트 목록
    public List<Quest> InProgressQuests { get; set; } = new List<Quest>();
    // 보상을 이미 받은 퀘스트의 ID 목록 (중복 보상 방지용)
    public List<int> RewardedQuestIds { get; set; } = new List<int>();

    // Player 클래스 생성자: 플레이어 이름을 받아 초기 설정
    public Player(string name)
    {
        Name = name;
        _level = 1; // Level 속성의 setter를 통해 레벨 업 메시지 및 퀘스트 업데이트
        // LoadSkillsFromJson(); // 스킬 로딩은 GameManager에서 플레이어 로드 시 호출하는 것이 권장됨
    }

    // 아이템 장착 메서드: 플레이어의 스탯을 업데이트하고 퀘스트 진행도를 알림
    public void EquipItem(Item itemToEquip)
    {
        // 장착 불가능한 아이템이거나, 인벤토리에 없는 아이템이면 장착 시도 중단
        if (!itemToEquip.IsEquip || !Inventory.Contains(itemToEquip))
        {
            Console.WriteLine($"'{itemToEquip.Name}'은(는) 장착할 수 없거나 소유하지 않은 아이템입니다.");
            return;
        }

        // 1. 동일 부위 아이템이 이미 장착되어 있다면 기존 아이템 해제
        Item? existingEquippedItem = EquippedItems.FirstOrDefault(i => i.Type == itemToEquip.Type);
        if (existingEquippedItem != null)
        {
            UnequipItem(existingEquippedItem); // 기존 아이템을 해제하고 스탯 되돌림
        }

        // 2. 인벤토리에서 장착할 아이템을 제거
        Inventory.Remove(itemToEquip);

        // 3. EquippedItems 리스트에 아이템 추가
        EquippedItems.Add(itemToEquip);

        // 4. 플레이어 스탯 업데이트 (아이템 스탯을 플레이어의 실제 스탯 속성에 더함)
        MaxHP += itemToEquip.Hp;
        CurrentHP += itemToEquip.Hp;
        MaxMP += itemToEquip.Mp;
        CurrentMP += itemToEquip.Mp;

        BaseAttack += itemToEquip.Attack;
        BaseDefense += itemToEquip.Defense;

        CriticalRate += itemToEquip.CriticalRate;
        CriticalDamage += itemToEquip.CriticalDamage;
        DodgeRate += itemToEquip.DodgeRate;

        Console.WriteLine($"'{itemToEquip.Name}'을(를) 장착했습니다!");

        // 5. 퀘스트 매니저에 장비 장착 이벤트 알림
        QuestManager.UpdateEquipmentQuestProgress();
    }

    // 아이템 해제 메서드: 플레이어의 스탯을 되돌림
    public void UnequipItem(Item itemToUnequip)
    {
        // 장착 중인 아이템이 아니라면 해제 시도 중단
        if (!EquippedItems.Contains(itemToUnequip))
        {
            Console.WriteLine($"'{itemToUnequip.Name}'은(는) 장착 중인 아이템이 아닙니다.");
            return;
        }

        // 1. EquippedItems 리스트에서 아이템 제거
        EquippedItems.Remove(itemToUnequip);

        // 2. 인벤토리에 다시 추가
        Inventory.Add(itemToUnequip);

        // 3. 플레이어 스탯 되돌리기 (아이템 스탯을 플레이어의 실제 스탯 속성에서 뺌)
        MaxHP -= itemToUnequip.Hp;
        CurrentHP -= itemToUnequip.Hp;
        // 현재 HP/MP가 최대 HP/MP를 초과하거나 0보다 작아지지 않도록 보정
        if (CurrentHP > MaxHP) CurrentHP = MaxHP;
        if (CurrentHP < 0) CurrentHP = 0;

        MaxMP -= itemToUnequip.Mp;
        CurrentMP -= itemToUnequip.Mp;
        if (CurrentMP > MaxMP) CurrentMP = MaxMP;
        if (CurrentMP < 0) CurrentMP = 0;

        BaseAttack -= itemToUnequip.Attack;
        BaseDefense -= itemToUnequip.Defense;

        CriticalRate -= itemToUnequip.CriticalRate;
        CriticalDamage -= itemToUnequip.CriticalDamage;
        DodgeRate -= itemToUnequip.DodgeRate;

        Console.WriteLine($"'{itemToUnequip.Name}'을(를) 장착 해제했습니다.");

        // 아이템 해제 시에는 퀘스트 업데이트가 필요하지 않음
    }

    // 총 공격력을 반환하는 계산 속성 (BaseAttack에 아이템 효과가 이미 반영되므로 BaseAttack 반환)
    public int TotalAttack => BaseAttack;
    // 총 방어력을 반환하는 계산 속성
    public int TotalDefense => BaseDefense;
    // 총 최대 HP를 반환하는 계산 속성
    public int TotalMaxHP => MaxHP;
    // 총 최대 MP를 반환하는 계산 속성
    public int TotalMaxMP => MaxMP;
    // 총 치명타 확률을 반환하는 계산 속성
    public float TotalCriticalRate => CriticalRate;
    // 총 치명타 데미지 배율을 반환하는 계산 속성
    public float TotalCriticalDamage => CriticalDamage;
    // 총 회피율을 반환하는 계산 속성
    public float TotalDodgeRate => DodgeRate;


    // 치명타 발생 여부를 랜덤으로 판단
    public bool IsCriticalHit()
    {
        return _random.NextDouble() < CriticalRate;
    }

    // 회피 성공 여부를 랜덤으로 판단
    public bool TryDodge()
    {
        return _random.NextDouble() < DodgeRate;
    }

    // 경험치 획득 처리 및 레벨업 체크
    public void GainExp(int amount)
    {
        Experience += amount;
        Console.WriteLine($"경험치 {amount}를 획득했습니다. 현재 경험치: {Experience}");
        while (Experience >= Level * 100) // 현재 레벨업 필요 경험치보다 많으면 레벨업
        {
            Experience -= Level * 100; // 남은 경험치 계산
            Level++; // 레벨업 (Level 속성의 setter가 호출되어 퀘스트 업데이트)

            // 레벨업 시 기본 스탯 증가
            MaxHP += 20;
            CurrentHP = MaxHP; // 현재 HP 최대치로 회복
            MaxMP += 10;
            CurrentMP = MaxMP; // 현재 MP 최대치로 회복
            BaseAttack += 2;
            BaseDefense += 1;
            CriticalRate += 0.01f;
            DodgeRate += 0.01f;
            CriticalDamage += 0.1f;

            Console.WriteLine($"\n[레벨 업!] {Level} 레벨이 되었습니다!");
        }
    }

    // 장착 아이템이 있는지 여부 반환
    public bool HasEquippedItems()
    {
        return EquippedItems.Count > 0;
    }

    // 현재 장착 중인 아이템 목록을 콘솔에 출력
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
            Console.WriteLine($"- {item.Name} (공격력 +{item.Attack}, 방어력 +{item.Defense}, HP +{item.Hp}, MP +{item.Mp})");
        }
    }

    // 플레이어의 현재 상태(스탯)를 콘솔에 출력 (장착 아이템 효과 포함)
    public void ShowStatus()
    {
        // 이제 BaseAttack, BaseDefense 등은 이미 아이템 효과가 반영된 최종 값입니다.
        // 따라서 "보너스" 계산을 다시 할 필요 없이, BaseAttack - 초기 BaseAttack 값을 표시합니다.
        int initialBaseAttack = 10; // 초기 BaseAttack 값 (게임 시작 시 플레이어의 기본 스탯)
        int initialBaseDefense = 5; // 초기 BaseDefense 값
        int initialMaxHP = 10; // 초기 MaxHP 값
        int initialMaxMP = 50; // 초기 MaxMP 값
        float initialCriticalRate = 0.1f; // 초기 CriticalRate 값
        float initialCriticalDamage = 1.6f; // 초기 CriticalDamage 값
        float initialDodgeRate = 0.1f; // 초기 DodgeRate 값

        // 아이템으로 인한 추가 스탯 계산 (표시용)
        string atkStr = (TotalAttack - initialBaseAttack) > 0 ? $" (+{TotalAttack - initialBaseAttack})" : "";
        string defStr = (TotalDefense - initialBaseDefense) > 0 ? $" (+{TotalDefense - initialBaseDefense})" : "";
        string hpStr = (TotalMaxHP - initialMaxHP) > 0 ? $" (+{TotalMaxHP - initialMaxHP})" : "";
        string mpStr = (TotalMaxMP - initialMaxMP) > 0 ? $" (+{TotalMaxMP - initialMaxMP})" : "";
        string crStr = (TotalCriticalRate - initialCriticalRate) > 0 ? $" (+{(TotalCriticalRate - initialCriticalRate) * 100:F1}%)" : "";
        string cdStr = (TotalCriticalDamage - initialCriticalDamage) > 0 ? $" (+{(TotalCriticalDamage - initialCriticalDamage) * 100:F1}%)" : "";
        string drStr = (TotalDodgeRate - initialDodgeRate) > 0 ? $" (+{(TotalDodgeRate - initialDodgeRate) * 100:F1}%)" : "";

        List<string> strings = new List<string>()
        {
            $"Lv. {Level:D2}",
            $"{Name} ( {JobsKorean(Job)} )",
            $"공격력 : {TotalAttack}{atkStr}",
            $"방어력 : {TotalDefense}{defStr}",
            $"체 력   : {CurrentHP} / {TotalMaxHP}{hpStr}",
            $"마 나   : {CurrentMP} / {TotalMaxMP}{mpStr}",
            $"치명타 확률   : {TotalCriticalRate * 100:F1}%{crStr}",
            $"치명타 데미지 : {TotalCriticalDamage * 100:F1}%{cdStr}",
            $"회피율        : {TotalDodgeRate * 100:F1}%{drStr}",
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
                WriteIndented = true, // 가독성을 위해 들여쓰기 적용
                // 한글 깨짐 방지를 위한 인코더 설정
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
            };

            // 스킬 객체 대신, 스킬 ID 리스트만 저장하여 데이터 크기 줄임
            var learnedSkillIds = Skills.Select(s => s.Id).ToList();

            string json = JsonSerializer.Serialize(learnedSkillIds, options);
            string path = GetSkillSavePath(); // 저장 경로 가져오기

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
        string path = GetSkillSavePath(); // 불러올 파일 경로

        if (!File.Exists(path))
        {
            return; // 저장된 파일이 없으면 함수 종료
        }

        try
        {
            string json = File.ReadAllText(path);
            var skillIds = JsonSerializer.Deserialize<List<int>>(json); // 스킬 ID 목록 역직렬화

            if (skillIds == null) return; // 불러온 데이터가 없으면 종료

            Skills.Clear(); // 현재 스킬 목록 초기화

            foreach (int id in skillIds)
            {
                // SkillManager에서 ID에 해당하는 스킬을 찾아 플레이어 스킬 목록에 추가
                if (SkillManager.TryGetSkill(id, out Skill skill))
                {
                    skill.learnState = LearnState.Learned; // 스킬을 배운 상태로 표시
                    Skills.Add(skill);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] 플레이어 스킬 불러오기 실패: {e.Message}");
        }
    }

    // JSON 저장/불러오기 경로 반환 (게임 매니저의 저장 디렉토리 및 선택된 슬롯 사용)
    private string GetSkillSavePath()
    {
        // GameManager.instance.saveDir와 GameManager.instance.selectedSlot이 올바르게 설정되어 있다고 가정
        return Path.Combine(GameManager.instance.saveDir, $"Skills_{GameManager.instance.selectedSlot}.json");
    }

    // 현재 진행 중인 퀘스트 로그를 콘솔에 출력
    public void ShowQuestLog()
    {
        List<string> questLogContents = new List<string>();
        if (InProgressQuests.Count == 0)
        {
            questLogContents.Add("진행 중인 퀘스트가 없습니다.");
        }
        else
        {
            foreach (var quest in InProgressQuests)
            {
                string status = "";
                if (quest.State == QuestState.InProgress)
                {
                    status = $"(진행 중: {Math.Min(quest.CurrentCount, quest.RequiredCount)}/{quest.RequiredCount})";
                }
                else if (quest.State == QuestState.Completed)
                {
                    status = "(완료!) 보상 대기 중";
                }
                questLogContents.Add($"- {quest.Title} {status}");
            }
        }
        UI.DrawTitledBox("퀘스트 로그", questLogContents);
    }
}