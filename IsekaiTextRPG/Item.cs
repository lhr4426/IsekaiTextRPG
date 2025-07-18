using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class Item
{
    public enum ItemType
    {
        Weapon,
        HeadArmor,
        BodyArmor,
        LegArmor,
        OffHand,
        Usable, // 사용 가능한 아이템 (ex. 포션)
        ClassChange,
    }

    private string TypeKorean(ItemType type)
    {
        return type switch
        {
            ItemType.Weapon => "무기",
            ItemType.HeadArmor => "머리 방어구",
            ItemType.BodyArmor => "몸통 방어구",
            ItemType.LegArmor => "다리 방어구",
            ItemType.OffHand => "보조 무기",
            ItemType.Usable => "소비 아이템",
            ItemType.ClassChange => "전직 아이템",
            _ => type.ToString()
        };
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public int Hp { get; set; }
    public int Mp { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Price { get; set; }
    public bool IsEquip { get; set; }
    public ItemType Type { get; set; }
    public float CriticalRate { get; set; }      // 치명타 확률 
    public float CriticalDamage { get; set; }    // 치명타 데미지 배율 1.6 
    public float DodgeRate { get; set; }       // 회피율 (
    public int ItemCount { get; set; }
    public Item() { }
    public Item(string name, string description, int attack, int defense, int hp, int mp, int price,
               bool isEquip, ItemType type, float criticalRate = 0,
               float criticalDamage = 1.6f, float dodgeRate = 0, int itemCount = 1)
    {
        Name = name;
        Description = description;
        Attack = attack;
        Defense = defense;
        Hp = hp;
        Mp = mp;
        Price = price;
        IsEquip = isEquip;
        Type = type;
        CriticalRate = criticalRate;
        CriticalDamage = criticalDamage;
        DodgeRate = dodgeRate;
        ItemCount = itemCount;
    }

    public override bool Equals(object obj)
    {
        if (obj is Item other)
            return Name == other.Name;
        return false;
    }

    public override int GetHashCode() => Name.GetHashCode();

    public override string ToString()
    {
        return $"{Name} ({TypeKorean(Type)}) - {Description}   |" +
               $"공격력: {Attack}, 방어력: {Defense}    |" +
               $"치명타 확률: {CriticalRate * 100:F1}%, 치명타 데미지: {CriticalDamage * 100:F1}%    |" +
               $"회피율: {DodgeRate * 100:F1}%    |" +
               $"가격: {Price} 골드, 착용 가능: {(IsEquip ? "예" : "아니오")}";
    }
}