using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
public class ItemSystem
{
    // 아이템 관리 
    private List<Item> inventory = new List<Item>();
    public int Gold { get; private set; }

    // 생성자
    public ItemSystem(int startingGold = 1000)
    {
        Gold = startingGold;
    }

    // 아이템 생성 및 구매
    public bool BuyItem(string name, string description, int attack, int defense,int hp,int mp,
                       int price, bool isEquip, Item.ItemType itemType, float criticalRate = 0,
                       float criticalDamage = 1.6f, float dodgeRate = 0, bool suppressMessage = false) 
    {
        var newItem = new Item(name, description, attack, defense,hp,mp, price, isEquip,
                              itemType, criticalRate, criticalDamage, dodgeRate);

        if (Gold < price)
        {
            Console.WriteLine($"골드가 부족합니다! 현재 골드: {Gold}, 필요한 골드: {price}");
            return false;
        }

        if(newItem.Type != Item.ItemType.Usable
    && inventory.Any(i => i.Name == name))
        {
            Console.WriteLine("이미 소유한 아이템입니다.");
            return false;
        }

        Gold -= price;
        if (!suppressMessage)
            Console.WriteLine($"{name}을(를) 구매했습니다! 남은 골드: {Gold}");
        inventory.Add(newItem);
        
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
    private Item? FindItem(string name)
    {
        foreach (var item in inventory)
        {
            if (item.Name == name)
                return item;
        }
        return null;
    }

    // 아이템 소유 여부 확인
    public bool FineItem(string name)
    {
        return FindItem(name) != null;
    }

    // 모든 아이템 출력
    public bool HasItem(string name)
    {
        return FindItem(name) != null;
    }

    public List<Item> GetUsableItems()
    {
        return inventory
            .Where(i => i.Type == Item.ItemType.Usable)
            .ToList();
    }

    public bool UseConsumable(string name)
    {
        var item = inventory.FirstOrDefault(i => i.Name == name);
        if (item == null) return false;
        inventory.Remove(item);
        return true;
    }

    public void ShowInventory()
    {
        var strings = new List<string>();
        foreach (var item in inventory)
            strings.Add(item.ToString());
        UI.DrawTitledBox("인벤토리", null);
        UI.DrawLeftAlignedBox(strings);
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
