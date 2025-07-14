namespace IsekaiTextRPG
{
    public enum skillId
    {
        FireSword = 0,
        IceSpear = 1,
        LightningStrike = 2,
        EarthShield = 3,
        WindBlade = 4
    }
    public class SkillDB
    {
        
        public string Name { get; }
        public int Id { get; private set; }
        public int Damage { get; }
        public string Description { get; }
        public int ManaCost { get; }
        public int Cooldown { get; }

        public SkillDB()
        {
            Id = default;
            Name = string.Empty;
            Damage = 0;
            ManaCost = 0;
            Cooldown = 0;
            Description = string.Empty;
        }
        public SkillDB(string name, int damage, int manaCost, string desc)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = desc ?? throw new ArgumentNullException(nameof(desc));
            ManaCost = manaCost;
            Damage = damage;
        }
        public IReadOnlyList<SkillDB> Skills { get; } = new List<SkillDB>
        {
            new SkillDB("불꽃의 검", 20, 10,"불꽃을 담은 검으로 적에게 큰 피해를 줍니다."),
            new SkillDB("얼음 창", 20, 10, "얼음으로 만들어진 창을 던져 적을 얼립니다."),
            new SkillDB("번개 충격", 20, 10, "번개를 소환하여 적에게 강력한 충격을 가합니다."),
            new SkillDB("대지의 방패", 20, 10, "대지의 힘으로 자신을 보호하는 방패를 생성합니다." ),
            new SkillDB("바람의 칼날", 20, 10, "바람을 날카롭게 만들어 적을 베어냅니다." )
        };
    }


}
