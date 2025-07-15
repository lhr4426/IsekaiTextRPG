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
        Usable, // 사용 가능한 아이템 (ex. 포션)
    }

    private string TypeKorean(ItemType type)
    {
        return type switch
        {
            ItemType.Weapon => "무기",
            ItemType.HeadArmor => "머리 방어구",
            ItemType.BodyArmor => "몸통 방어구",
            ItemType.LegArmor => "다리 방어구",
            ItemType.Usable => "소비 아이템",
            _ => type.ToString()
        };
    }

    public string Name { get; }
    public string Description { get; }
    public int Hp { get; }
    public int Mp { get; }
    public int Attack { get; }
    public int Defense { get; }
    public int Price { get; }
    public bool IsEquip { get; }
    public ItemType Type { get; }
    public float CriticalRate { get; }      // 치명타 확률 
    public float CriticalDamage { get; }    // 치명타 데미지 배율 1.6 
    public float DodgeRate { get; }       // 회피율 (

    public Item(string name, string description, int attack, int defense, int price,
               bool isEquip, ItemType itemType, float criticalRate = 0,
               float criticalDamage = 1.6f, float dodgeRate = 0)
    {
        Name = name;
        Description = description;
        Attack = attack;
        Defense = defense;
        Price = price;
        IsEquip = isEquip;
        Type = itemType;
        CriticalRate = criticalRate;
        CriticalDamage = criticalDamage;
        DodgeRate = dodgeRate;
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
        return $"{Name} ({TypeKorean(Type)}) - {Description}\n" +
               $"공격력: {Attack}, 방어력: {Defense}\n" +
               $"치명타 확률: {CriticalRate * 100:F1}%, 치명타 데미지: {CriticalDamage * 100:F1}%\n" +
               $"회피율: {DodgeRate * 100:F1}%\n" +
               $"가격: {Price} 골드, 착용 가능: {(IsEquip ? "예" : "아니오")}";
    }

    
}

