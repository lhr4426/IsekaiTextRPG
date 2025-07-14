using System;
using System.Collections.Generic;

namespace Game.Items
{
    public class ItemSystem
    {
        // 내부 아이템 클래스 정의
        private class Item
        {
            public string Name { get; }
            public string Description { get; }
            public int Attack { get; }
            public int Defense { get; }
            public int Price { get; }
            public bool IsEquip { get; }
            public string ItemType { get; }
            public double CriticalRate { get; }      // 치명타 확률 
            public double CriticalDamage { get; }    // 치명타 데미지 배율 1.6 
            public double EvasionRate { get; }       // 회피율 (

            public Item(string name, string description, int attack, int defense, int price,
                       bool isEquip, string itemType, double criticalRate = 0,
                       double criticalDamage = 1.6, double evasionRate = 0)
            {
                Name = name;
                Description = description;
                Attack = attack;
                Defense = defense;
                Price = price;
                IsEquip = isEquip;
                ItemType = itemType;
                CriticalRate = criticalRate;
                CriticalDamage = criticalDamage;  
                EvasionRate = evasionRate;
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
                return $"{Name} ({ItemType}) - {Description}\n" +
                       $"공격력: {Attack}, 방어력: {Defense}\n" +
                       $"치명타 확률: {CriticalRate * 100:F1}%, 치명타 데미지: {CriticalDamage * 100:F1}%\n" +
                       $"회피율: {EvasionRate * 100:F1}%\n" +
                       $"가격: {Price} 골드, 착용 가능: {(IsEquip ? "예" : "아니오")}";
            }
        }

        // 아이템 관리 
        private HashSet<Item> inventory = new HashSet<Item>();
        public int Gold { get; private set; }

        // 생성자
        public ItemSystem(int startingGold = 1000)
        {
            Gold = startingGold;
        }

        // 아이템 생성 및 구매
        public bool BuyItem(string name, string description, int attack, int defense,
                           int price, bool isEquip, string itemType, double criticalRate = 0,
                           double criticalDamage = 1.6, double evasionRate = 0)
        {
            var newItem = new Item(name, description, attack, defense, price, isEquip,
                                  itemType, criticalRate, criticalDamage, evasionRate);

            if (Gold < price)
            {
                Console.WriteLine($"골드가 부족합니다! 현재 골드: {Gold}, 필요한 골드: {price}");
                return false;
            }

            if (inventory.Contains(newItem))
            {
                Console.WriteLine("이미 소유한 아이템입니다.");
                return false;
            }

            Gold -= price;
            inventory.Add(newItem);
            Console.WriteLine($"{name}을(를) 구매했습니다! 남은 골드: {Gold}");
            return true;
        }

        // 아이템 판매 
        public bool SellItem(string itemName)
        {
            var itemToSell = FindItem(itemName);
            if (itemToSell == null)
            {
                Console.WriteLine("판매할 아이템이 없습니다.");
                return false;
            }

            inventory.Remove(itemToSell);
            int sellPrice = (int)(itemToSell.Price * 0.85);  // 판매가 85%
            Gold += sellPrice;
            Console.WriteLine($"{itemToSell.Name}을(를) 판매했습니다! 골드 +{sellPrice}, 현재 골드: {Gold}");
            return true;
        }

        // 아이템 찾기
        private Item FindItem(string name)
        {
            foreach (var item in inventory)
            {
                if (item.Name == name)
                    return item;
            }
            return null;
        }

        // 아이템 소유 여부 확인
        public bool HasItem(string name)
        {
            return FindItem(name) != null;
        }

        // 모든 아이템 출력
        public void PrintAllItems()
        {
            Console.WriteLine("===== 인벤토리 =====");
            Console.WriteLine($"현재 골드: {Gold}");

            if (inventory.Count == 0)
            {
                Console.WriteLine("아이템이 없습니다.");
                return;
            }

            foreach (var item in inventory)
            {
                Console.WriteLine("\n" + item);
                Console.WriteLine("-------------------");
            }
        }

        // 아이템 정보 가져오기
        public string GetItemInfo(string name)
        {
            var item = FindItem(name);
            if (item == null)
                return "해당 아이템이 없습니다.";

            return item.ToString();
        }
    }
}