using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
public class ItemSystem
{
    public static Item CreateJobChangeItem(string jobChangeItemName)
    {
        // 전직 아이템의 능력치는 0이고, 소모품 타입이며, 가격은 0으로 설정
        // Item(name, description, attack, defense, hp, mp, price, isEquip, type, criticalRate, criticalDamage, dodgeRate)
        return jobChangeItemName switch
        {
            "전직의 증표 I" => new Item("전직의 증표 I", "메이플 세계의 직업군으로 전직할 수 있는 증표입니다.", 0, 0, 0, 0, 0, false, Item.ItemType.ClassChange, 0f, 0f, 0f),
            "전직의 증표 II" => new Item("전직의 증표 II", "라크라시아 대륙의 직업군으로 전직할 수 있는 증표입니다.", 0, 0, 0, 0, 0, false, Item.ItemType.ClassChange, 0f, 0f, 0f),
            "전직의 증표 III" => new Item("전직의 증표 III", "아라드 세계의 직업군으로 전직할 수 있는 증표입니다.", 0, 0, 0, 0, 0, false, Item.ItemType.ClassChange, 0f, 0f, 0f),
            "플레이 해 주셔서 감사합니다" => new Item("플레이 해 주셔서 감사합니다", "모든 직업군으로 전직할 수 있는 증표입니다.", 0, 0, 0, 0, 0, false, Item.ItemType.ClassChange, 0f, 0f, 0f),
            _ => throw new ArgumentException($"알 수 없는 전직 아이템 이름: {jobChangeItemName}") // 정의되지 않은 아이템 요청 시 예외 발생
        };
    }

    // 아이템 생성 및 구매
    public bool BuyItem(string name, string description, int attack, int defense, int hp, int mp,
                       int price, bool isEquip, Item.ItemType itemType, float criticalRate = 0,
                       float criticalDamage = 1.6f, float dodgeRate = 0, bool suppressMessage = false)
    {
        var newItem = new Item(name, description, attack, defense, hp, mp, price, isEquip,
                              itemType, criticalRate, criticalDamage, dodgeRate);

        if (GameManager.player.Gold < price)
        {
            Console.WriteLine($"골드가 부족합니다! 현재 골드: {GameManager.player.Gold}, 필요한 골드: {price}");
            return false;
        }

        if (newItem.Type != Item.ItemType.Usable
    && GameManager.player.Inventory.Any(i => i.Name == name))
        {
            Console.WriteLine("이미 소유한 아이템입니다.");
            return false;
        }

        GameManager.player.Gold -= price;
        if (!suppressMessage)
            Console.WriteLine($"{name}을(를) 구매했습니다! 남은 골드: {GameManager.player.Gold}");
        GameManager.player.Inventory.Add(newItem);

        return true;
    }

    // 아이템 판매 
    public bool SellItem(string itemName, int countToSell = 1)
    {
        var itemToSell = GameManager.player.Inventory.FirstOrDefault(i => i.Name == itemName && i.Type != Item.ItemType.ClassChange);
        if (itemToSell == null)
        {
            Console.WriteLine("판매할 아이템이 없습니다.");
            return false;
        }

        if (itemToSell.Type == Item.ItemType.ClassChange)
        {
            Console.WriteLine("이 아이템은 판매할 수 없습니다!");
            return false;
        }

        if (itemToSell.ItemCount < countToSell)
        {
            Console.WriteLine("판매 수량이 보유 수량보다 많습니다.");
            return false;
        }

        int sellPrice = (int)(itemToSell.Price * 0.85) * countToSell;
        itemToSell.ItemCount -= countToSell;

        if (itemToSell.ItemCount <= 0)
        {
            GameManager.player.Inventory.Remove(itemToSell);
        }

        GameManager.player.Gold += sellPrice;
        Console.WriteLine($"{itemToSell.Name} x{countToSell}개를 판매했습니다! 골드 +{sellPrice}, 현재 골드: {GameManager.player.Gold}");
        return true;
    }

    // 아이템 찾기
    private Item? FindItem(string name)
    {
        foreach (var item in GameManager.player.Inventory)
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
        return GameManager.player.Inventory
            .Where(i => i.Type == Item.ItemType.Usable)
            .ToList();
    }

    public bool UseConsumable(string name)
    {
        var item = GameManager.player.Inventory.FirstOrDefault(i => i.Name == name);
        if (item == null) return false;
        GameManager.player.Inventory.Remove(item);
        return true;
    }

    public void ShowInventoryForSell()
    {
        var lines = new List<string>
    {
        $"보유 골드: {GameManager.player.Gold}",
        "",
        "[아이템 목록]"
    };

        int index = 1;
        List<string> itemsToDisplay = new();
        Dictionary<int, string> indexToItemName = new(); // 선택 인덱스 매핑

        // 장비 아이템
        var equipmentItems = GameManager.player.Inventory
            .Where(i => i.Type != Item.ItemType.Usable && i.Type != Item.ItemType.ClassChange)
            .ToList();

        foreach (var eq in equipmentItems)
        {
            var statParts = new List<string>();
            if (eq.Attack > 0) statParts.Add($"공격력 +{eq.Attack}");
            if (eq.Defense > 0) statParts.Add($"방어력 +{eq.Defense}");
            if (eq.CriticalRate > 0) statParts.Add($"치명타율 {eq.CriticalRate:P0}");
            if (eq.CriticalDamage > 1) statParts.Add($"치명타배율 {eq.CriticalDamage}");
            if (eq.DodgeRate > 0) statParts.Add($"회피율 {eq.DodgeRate:P0}");

            string stats = statParts.Count > 0 ? " | " + string.Join(" | ", statParts) : "";
            string line = $"- {index} {eq.Name}{stats} | {eq.Description} | 판매가 {(int)(eq.Price * 0.85)}";
            itemsToDisplay.Add(line);
            indexToItemName[index] = eq.Name;
            index++;
        }

        // 소비 아이템
        var usableGroups = GameManager.player.Inventory
            .Where(i => i.Type == Item.ItemType.Usable)
            .GroupBy(i => i.Name);

        foreach (var grp in usableGroups)
        {
            var sample = grp.First();
            int totalCount = grp.Sum(i => i.ItemCount);
            if (totalCount <= 0) continue;

            string line = $"- {index} {sample.Name} x{totalCount} | {sample.Description} | 판매가 {(int)(sample.Price * 0.85)}";
            itemsToDisplay.Add(line);
            indexToItemName[index] = sample.Name;
            index++;
        }

        int maxDisplayWidth = itemsToDisplay.Select(UI.GetDisplayWidth).DefaultIfEmpty(0).Max();
        for (int i = 0; i < itemsToDisplay.Count; i++)
        {
            lines.Add(itemsToDisplay[i]);
            if (i < itemsToDisplay.Count - 1)
            {
                lines.Add(new string('─', maxDisplayWidth + 2));
            }
        }

        UI.DrawTitledBox("판매 인벤토리", null);
        UI.DrawLeftAlignedBox(lines);

        Console.Write("판매할 아이템 번호 (0: 나가기): ");
        int? selected = InputHelper.InputNumber(0, indexToItemName.Count);

        if (selected == null || selected == 0)
            return;

        string selectedName = indexToItemName[(int)selected];

        Console.Write("판매할 수량을 입력하세요: ");
        int? count = InputHelper.InputNumber(1, 999); // 예외처리는 내부에서 처리

        if (count != null)
            SellItem(selectedName, (int)count);
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
