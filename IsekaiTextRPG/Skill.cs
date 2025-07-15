using System;
using System.Collections.Generic;

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
    WindBlade = 3
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

    public Player.Jobs NeedJob { get; set; }
    

    public Skill(int id, string name, int damage, int manaCost, string description)//int cooldown
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Damage = damage;
        ManaCost = manaCost;
        //Cooldown = cooldown;
        learnState = LearnState.NotLearnable;
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    public void UpdateLearnState()
    {
        if (learnState == LearnState.Learned) return;

        if (learnState == LearnState.Learnable &&
            (NeedJob != GameManager.player.Job ||
            NeedLevel > GameManager.player.Level))
        {
            learnState = LearnState.NotLearnable;
        }

        if (learnState == LearnState.NotLearnable &&
            NeedJob == GameManager.player.Job && 
            NeedLevel <= GameManager.player.Level) 
        {
            learnState = LearnState.Learnable;
        }
    }
}
