using System;
using System.Collections.Generic;
using System.Text;

public enum LearnState
{
    NotLearnable,
    Learnable,
    Learned
}

public enum SkillId
{
    FireSword = 0,
    IceSpear = 1,
    LightningStrike = 2,
    WindBlade = 3,
    EarthQuake = 4,
    ShadowStep = 5
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

    public List<Player.Jobs> NeedJob { get; set; }

    public Skill(int id, string name, int damage, int manaCost, string description)//int cooldown
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Damage = damage;
        ManaCost = manaCost;
        //Cooldown = cooldown;
        learnState = LearnState.NotLearnable;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        // 여러 직업을 담을 수 있도록 초기화
        NeedJob = new List<Player.Jobs>();
    }

    public void UpdateLearnState()
    {
        if (learnState == LearnState.Learned) return;

        // 직업 조건을 '리스트에 포함되어 있는지'로 확인
        bool canLearn = NeedJob.Contains(GameManager.player.Job) && NeedLevel <= GameManager.player.Level;

        if (canLearn)
        {
            learnState = LearnState.Learnable;
        }
        else
        {
            learnState = LearnState.NotLearnable;
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"{(int)Id + 1}. ");
        sb.Append($"{Name}    |");
        sb.Append($"{Description}    |");
        sb.Append($"쿨타임: {Cooldown}턴    |");
        sb.Append($"공격력: {Damage}    |");
        sb.Append($"소모 마나 : {ManaCost}    |");
        return sb.ToString();
    }

    public List<string> ToShopString()
    {
        List<string> strings = new List<string>();
        StringBuilder sb = new StringBuilder();
        sb.Append($"{(int)Id + 1}. ");
        sb.Append($"{Name}    |");
        sb.Append($"{Description}");
        strings.Add(sb.ToString());

        sb.Clear();
        sb.Append($"쿨타임: {Cooldown}턴    |");
        sb.Append($"공격력: {Damage}    |");
        sb.Append($"소모 마나 : {ManaCost}");
        strings.Add(sb.ToString());
        
        sb.Clear();
        sb.Append($"필요 레벨 : {NeedLevel}    |");

        // 여러 직업을 예쁘게 출력하도록 변경 (예: 워로드, 웨폰마스터, 히어로)
        var jobNames = NeedJob.Select(job => GameManager.player.JobsKorean(job));
        sb.Append($"필요 직업 : {string.Join(", ", jobNames)}     | ");

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