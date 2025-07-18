using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum LearnState
{
    NotLearnable,  // 조건 미달로 인해 배울 수 없음
    Learnable,     // 조건 충족, 배울 수 있음
    Learned        // 이미 배운 상태
}

public enum SkillId
{
    FireSword = 0,
    EarthQuake = 1,
    IceSpear = 2,
    LightningStrike = 3,
    WindBlade = 4,
    ShadowStep = 5,
    FocusEvasion = 6,
    GuardianLightning = 7,
    PhantomBladeDance = 8,
    ComboDeathFault = 9,
    Doomsday = 10,
    AncientSpear = 11,
    Blizzard = 12,
    BladeArts = 13,
    NovaRemnant = 14,
    QuadrupleThrow = 15,
    ReadytoDie = 16,
    AuraWeapon = 17,
    OverloadedMana = 18

}

public class Skill
{
    public string Name { get; }
    public int Id { get; private set; }
    public int Damage { get; }
    public string Description { get; }
    public int ManaCost { get; }
    public int Cooldown { get; }

    public LearnState learnState { get; set; }
    public int NeedLevel { get; set; }

    // 복수형으로 명확하게 변경
    public List<Player.Jobs> NeedJobs { get; set; }

    public Skill(int id, string name, int damage, int manaCost, string description, int cooldown = 0)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Damage = damage;
        ManaCost = manaCost;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Cooldown = cooldown;
        learnState = LearnState.NotLearnable;
        NeedJobs = new List<Player.Jobs>(); // 리스트 초기화
    }

    public void UpdateLearnState()
    {
        // 이미 배운 상태라면 상태 변경하지 않음
        if (learnState == LearnState.Learned)
            return;

        // 플레이어 직업이 해당 스킬을 배울 수 있는 직업 목록에 있고, 레벨도 충족하면 Learnable
        bool canLearn = NeedJobs.Contains(GameManager.player.Job) && NeedLevel <= GameManager.player.Level;

        learnState = canLearn ? LearnState.Learnable : LearnState.NotLearnable;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"{Name}    |");
        sb.Append($"{Description}    |");
        sb.Append($"쿨타임: {Cooldown}턴    |");
        sb.Append($"공격력: {Damage}    |");
        sb.Append($"소모 마나: {ManaCost}    |");
        return sb.ToString();
    }

    public List<string> ToShopString()
    {
        List<string> strings = new List<string>();
        StringBuilder sb = new StringBuilder();

        // 첫 줄: 스킬 이름과 설명
        sb.Append($"{(int)Id + 1}. ");
        sb.Append($"{Name}    |");
        sb.Append($"{Description}");
        strings.Add(sb.ToString());

        // 두 번째 줄: 기본 정보 (쿨타임, 공격력, 마나)
        sb.Clear();
        sb.Append($"쿨타임: {Cooldown}턴    |");
        sb.Append($"공격력: {Damage}    |");
        sb.Append($"소모 마나: {ManaCost}");
        strings.Add(sb.ToString());

        // 세 번째 줄: 요구 조건 및 상태
        sb.Clear();
        sb.Append($"필요 레벨: {NeedLevel}    |");

        // 직업 이름은 한국어로 변환해서 출력
        var jobNames = NeedJobs.Select(job => Player.JobsKorean(job));
        sb.Append($"필요 직업: {string.Join(", ", jobNames)}    | ");

        switch (learnState)
        {
            case LearnState.NotLearnable:
                sb.Append("배울 수 없음");
                break;
            case LearnState.Learnable:
                sb.Append("배울 수 있음");
                break;
            case LearnState.Learned:
                sb.Append("배움");
                break;
        }

        strings.Add(sb.ToString());

        return strings;
    }
}