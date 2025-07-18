using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;

public class Player
{
    private static readonly Random _random = new Random();

    public enum Jobs
    {
        Prestige, Warlord, WeaponMaster, Hero, Sorceress,
        Summoner, ArchMage, Blade, Rogue, NightLord,
    }

    public static string JobsKorean(Jobs job)
    {
        return job switch
        {
            Jobs.Prestige => "환생자",
            Jobs.Warlord => "워로드",
            Jobs.WeaponMaster => "웨폰마스터",
            Jobs.Hero => "히어로",
            Jobs.ArchMage => "썬콜",
            Jobs.Sorceress => "소서리스",
            Jobs.Summoner => "소환사",
            Jobs.Blade => "블레이드",
            Jobs.Rogue => "로그",
            Jobs.NightLord => "나이트로드",
            _ => job.ToString()
        };
    }

    public static string? JobItemName(Jobs job)
    {
        return job switch
        {
            Jobs.Hero => "전직의 증표 I",
            Jobs.ArchMage => "전직의 증표 I",
            Jobs.NightLord => "전직의 증표 I",
            Jobs.Warlord => "전직의 증표 II",
            Jobs.Sorceress => "전직의 증표 II",
            Jobs.Blade => "전직의 증표 II",
            Jobs.WeaponMaster => "전직의 증표 III",
            Jobs.Summoner => "전직의 증표 III",
            Jobs.Rogue => "전직의 증표 III",
            _ => null
        };
    }

    public string Name { get; set; }

    private int _level;
    public int Level
    {
        get { return _level; }
        set
        {
            if (_level != value)
            {
                _level = value;
                UpdateMaxExp(); // 레벨이 변경될 때마다 MaxExp 업데이트
                QuestManager.UpdateLevelQuestProgress(_level);
            }
        }
    }

    public int Exp { get; set; } = 0; // 현재 레벨에서 획득한 경험치
    public int MaxExp { get; private set; } // 현재 레벨에서 다음 레벨업에 필요한 총 경험치

    public int RemainingExp
    {
        get
        {
            return Math.Max(0, MaxExp - Exp);
        }
    }

    public int Gold { get; set; } = 1000;
    public Jobs Job { get; set; } = Jobs.Prestige;

    public int MaxHP { get; set; } = 100;
    public int CurrentHP { get; set; } = 100;

    public int MaxMP { get; set; } = 50;
    public int CurrentMP { get; set; } = 50;

    public int BaseAttack { get; set; } = 10;
    public int BaseDefense { get; set; } = 5;

    public float CriticalRate { get; set; } = 0.1f;
    public float CriticalDamage { get; set; } = 1.6f;
    public float DodgeRate { get; set; } = 0.1f;

    public List<Item> Inventory { get; set; } = new List<Item>();
    public List<Item> EquippedItems { get; set; } = new List<Item>();
    public List<Item> ShopItems { get; set; } = new List<Item>();

    public List<Skill> Skills { get; set; } = new List<Skill>();

    private string SkillSavePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Skills_{Name}.json");
    public List<Quest> InProgressQuests { get; set; } = new List<Quest>();
    public List<int> RewardedQuestIds { get; set; } = new List<int>();

    public Player(string name)
    {
        Name = name;
        _level = 1;
        UpdateMaxExp();
    }

    public void EquipItem(Item itemToEquip)
    {
        if (!itemToEquip.IsEquip || !Inventory.Contains(itemToEquip))
        {
            Console.WriteLine($"'{itemToEquip.Name}'은(는) 장착할 수 없거나 소유하지 않은 아이템입니다.");
            return;
        }

        Item? existingEquippedItem = EquippedItems.FirstOrDefault(i => i.Type == itemToEquip.Type);
        if (existingEquippedItem != null)
        {
            UnequipItem(existingEquippedItem);
        }

        EquippedItems.Add(itemToEquip);

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
        QuestManager.UpdateEquipmentQuestProgress();
    }

    public void UnequipItem(Item itemToUnequip)
    {
        if (!EquippedItems.Contains(itemToUnequip))
        {
            Console.WriteLine($"'{itemToUnequip.Name}'은(는) 장착 중인 아이템이 아닙니다.");
            return;
        }

        EquippedItems.Remove(itemToUnequip);

        MaxHP -= itemToUnequip.Hp;
        CurrentHP -= itemToUnequip.Hp;
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
    }

    public int TotalAttack => BaseAttack;
    public int TotalDefense => BaseDefense;
    public int TotalMaxHP => MaxHP;
    public int TotalMaxMP => MaxMP;
    public float TotalCriticalRate => CriticalRate;
    public float TotalCriticalDamage => CriticalDamage;
    public float TotalDodgeRate => DodgeRate;

    public bool IsCriticalHit()
    {
        return _random.NextDouble() < CriticalRate;
    }

    public bool TryDodge()
    {
        return _random.NextDouble() < DodgeRate;
    }

    private void UpdateMaxExp()
    {
        if (Level == 1)
        {
            MaxExp = 100;
        }
        else
        {
            MaxExp = (int)(Math.Pow(Level, 2) * 50 + Level * 50 + 100);
            if (MaxExp < 100) MaxExp = 100;
        }
    }

    public void GainExp(int amount)
    {
        int initialLevel = Level;
        Exp += amount;

        while (Exp >= MaxExp)
        {
            Exp -= MaxExp;
            Level++;

            MaxHP += 20;
            CurrentHP = MaxHP;
            MaxMP += 10;
            CurrentMP = MaxMP;
            BaseAttack += 2;
            BaseDefense += 1;
            CriticalRate += 0.01f;
            DodgeRate += 0.01f;
            CriticalDamage += 0.1f;
        }

        Console.WriteLine($"경험치 {amount}를 획득했습니다. 현재 경험치: {Exp} / {MaxExp} (남은 경험치: {RemainingExp})");

        if (Level > initialLevel)
        {
            Console.WriteLine($"\n[레벨 업!] {Level} 레벨이 되었습니다!");
        }
    }

    public bool HasEquippedItems()
    {
        return EquippedItems.Count > 0;
    }

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

    public void ShowStatus()
    {
        int initialBaseAttack = 10;
        int initialBaseDefense = 5;
        int initialMaxHP = 100;
        int initialMaxMP = 50;
        float initialCriticalRate = 0.1f;
        float initialCriticalDamage = 1.6f;
        float initialDodgeRate = 0.1f;

        string atkStr = (TotalAttack - initialBaseAttack) > 0 ? $" (+{TotalAttack - initialBaseAttack})" : "";
        string defStr = (TotalDefense - initialBaseDefense) > 0 ? $" (+{TotalDefense - initialBaseDefense})" : "";
        string hpStr = (TotalMaxHP - initialMaxHP) > 0 ? $" (+{TotalMaxHP - initialMaxHP})" : "";
        string mpStr = (TotalMaxMP - initialMaxMP) > 0 ? $" (+{TotalMaxMP - initialMaxMP})" : "";
        string crStr = (TotalCriticalRate - initialCriticalRate) > 0 ? $" (+{(TotalCriticalRate - initialCriticalRate) * 100:F1}%)" : "";
        string cdStr = (TotalCriticalDamage - initialCriticalDamage) > 0 ? $" (+{(TotalCriticalDamage - initialCriticalRate) * 100:F1}%)" : "";
        string drStr = (TotalDodgeRate - initialDodgeRate) > 0 ? $" (+{(TotalDodgeRate - initialDodgeRate) * 100:F1}%)" : "";

        List<string> strings = new List<string>()
        {
            $"Lv. {Level:D2}",
            $"{Name} ( {JobsKorean(Job)} )",
            $"경험치 : {Exp} / {MaxExp} (남은 경험치: {RemainingExp})",
            $"공격력 : {TotalAttack}{atkStr}",
            $"방어력 : {TotalDefense}{defStr}",
            $"체 력   : {CurrentHP} / {TotalMaxHP}{hpStr}",
            $"마 나   : {CurrentMP} / {TotalMaxMP}{mpStr}",
            $"치명타 확률   : {TotalCriticalRate * 100:F1}%{crStr}",
            $"치명타 데미지 : {TotalCriticalDamage * 100:F1}%{cdStr}",
            $"회피율        : {TotalDodgeRate * 100:F1}%{drStr}",
            $"Gold : {Gold} G"
        };

        UI.DrawTitledBox("스테이터스", strings);
    }

    public void SaveSkillsToJson()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            var learnedSkillIds = Skills.Select(s => s.Id).ToList();

            string json = JsonSerializer.Serialize(learnedSkillIds, options);
            string path = GetSkillSavePath();

            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] 플레이어 스킬 저장 실패: {e.Message}");
        }
    }

    public void LoadSkillsFromJson()
    {
        string path = GetSkillSavePath();

        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            var skillIds = JsonSerializer.Deserialize<List<int>>(json);

            if (skillIds == null) return;

            Skills.Clear();

            foreach (int id in skillIds)
            {
                if (SkillManager.TryGetSkill(id, out Skill skill))
                {
                    skill.learnState = LearnState.Learned;
                    Skills.Add(skill);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] 플레이어 스킬 불러오기 실패: {e.Message}");
        }
    }

    private string GetSkillSavePath()
    {
        return Path.Combine(GameManager.instance.saveDir, $"Skills_{GameManager.instance.selectedSlot}.json");
    }

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

    public string ToHeroString()
    {
        int initialBaseAttack = 10;
        string atkStr = (TotalAttack - initialBaseAttack) > 0 ? $" (+{TotalAttack - initialBaseAttack})" : "";
        string strings =
            $"Lv. {Level:D2} |" +
            $"{Name} ( {JobsKorean(Job)} ) |" +
            $"공격력 : {TotalAttack}{atkStr} |";

        return strings;
    }
}