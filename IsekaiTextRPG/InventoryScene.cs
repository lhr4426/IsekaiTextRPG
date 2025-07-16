using Microsoft.VisualBasic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class InventoryScene : GameScene
{
    public override string SceneName => "인벤토리";

    public override GameScene? StartScene()
    {
        Console.Clear();
        Player player = GameManager.player;

        List<string> strings = new List<string>()
        {
            "인벤토리",
            "보유 중인 아이템을 관리할 수 있습니다.",
            "",
            "[아이템 목록]"
        };


        var sortedInventory = player.Inventory
            .OrderByDescending(item => player.EquippedItems.Contains(item)) // 장착 여부
            .ThenBy(item => item.Type)                                      // 아이템 타입
            .ThenBy(item => item.Name)                                      // 이름순
            .ToList();

        if (sortedInventory.Count == 0)
        {
            strings.Add(" - 아이템이 없습니다.");
        }
        else
        {
            int index = 1;
            foreach (var item in sortedInventory.Where(i => i.Type != Item.ItemType.Usable))
            {
                string equippedMark = player.EquippedItems.Contains(item) ? "[E]" : "   ";
                string stats = item.Attack > 0 ? $"공격력 +{item.Attack}" : $"방어력 +{item.Defense}";
                strings.Add($"- {index} {equippedMark}{item.Name,-15} | {stats} | {item.Description}");
                index++;
            }

            var consumables = sortedInventory
                .Where(i => i.Type == Item.ItemType.Usable)
                .GroupBy(i => i.Name)
                .OrderBy(g => g.Key);// 소비 아이템은 이름으로 그룹화
            foreach (var g in consumables)
            {
                var sample = g.First();
                string equippedMark = player.EquippedItems.Contains(sample) ? "[E]" : "   ";
                string stats = sample.Attack > 0
                    ? $"공격력 +{sample.Attack}"
                    : $"방어력 +{sample.Defense}";
                strings.Add(
                    $"- {index} {equippedMark}{sample.Name,-15} x{g.Count()} | " +
                    $"{stats} | {sample.Description}"
                );
                index++;
            }
        }

        strings.Add("1. 장착/해제 관리");
        strings.Add("0. 나가기");

        UI.DrawTitledBox(SceneName, strings);

        Console.Write(">> ");

        int? input = InputHelper.InputNumber(0, 1);

        switch (input)
        {
            case 1: 
                HandleEquip(player,
                    sortedInventory
                        .Where(i => i.IsEquip)      
                        .ToList() // 장착 가능한 아이템만 필터링
                );
                return this;
            case 0:
                return prevScene;
            default:
                Console.WriteLine("잘못된 입력입니다. 아무 키나 눌러 계속...");
                return this;

        }
    }

    public void PrintUsableItems(out List<Item> inven)
    {
        Player player = GameManager.player;
        List<string> strings = new();

        var usable = player.Inventory
            .Where(item => item.Type == Item.ItemType.Usable)
            .OrderByDescending(item => player.EquippedItems.Contains(item))
            .ThenBy(item => item.Type)
            .ThenBy(item => item.Name)
            .ToList(); // 사용 가능한 아이템만 필터링

        List<string> _strings = new();

        if (usable.Count > 0)
        {
            int displayIndex = 1;
            for (int i = 0; i < usable.Count; i++)
            {
                var item = usable[i];
                int count = usable.Skip(i).TakeWhile(x => x.Name == item.Name).Count();// 중복된 아이템 수
                string equippedMark = player.EquippedItems.Contains(item) ? "[E]" : "   ";
                string stats = item.Attack > 0 ? $"공격력 +{item.Attack}" : $"방어력 +{item.Defense}";
                // 중복된 아이템은 하나로 묶어서 표시
                strings.Add( $"- {displayIndex} {equippedMark}{item.Name,-15} x{count} | " + $"{stats} | {item.Description}");
                i += count - 1; // 중복된 아이템 수 만큼 건너뜀
                displayIndex++; 
            }
        }
        else
        {
            strings.Add("사용 가능한 아이템이 없습니다.");
        }

        UI.DrawBox(strings);
        inven = usable.GroupBy(x => x.Name).Select(g => g.First()).ToList(); 
    }
    

    private void HandleEquip(Player player, List<Item> inventory)
    {
        while (true)
        {
            Console.Clear();

            List<string> strings = new List<string>()
            {
                "보유 중인 아이템을 관리할 수 있습니다.",
                "[아이템 목록]"
            };

            
            for (int i = 0; i < inventory.Count; i++) 
            {
                var item = inventory[i];
                string equippedMark = player.EquippedItems.Contains(item) ? "[E]" : "   ";
                string stats = item.Attack > 0 ? $"공격력 +{item.Attack}" : $"방어력 +{item.Defense}";
                strings.Add($"- {i + 1} {equippedMark}{item.Name,-15} | {stats} | {item.Description}");
            }

            strings.Add("0. 나가기");

            UI.DrawTitledBox("인벤토리 - 장착 관리", strings);

            Console.Write(">> ");

            int? input = InputHelper.InputNumber(0, inventory.Count);

            if (input == 0) return;
            if (input == null) continue;

            var selectedItem = inventory[(int)input - 1];

            if (!selectedItem.IsEquip)
            {
                Console.WriteLine("장착할 수 없는 아이템입니다.");
                Console.ReadKey();
                continue;
            }

            if (player.EquippedItems.Contains(selectedItem))
            {
                player.EquippedItems.Remove(selectedItem);
                Console.WriteLine($"'{selectedItem.Name}' 장착 해제되었습니다.");
            }
            else
            {
                // 동일 부위 장착 아이템이 있으면 해제
                var sameSlot = player.EquippedItems
                    .FirstOrDefault(e => e.Type == selectedItem.Type);
                if (sameSlot != null)
                    player.EquippedItems.Remove(sameSlot);

                player.EquippedItems.Add(selectedItem);
                Console.WriteLine($"'{selectedItem.Name}' 장착되었습니다.");
            }

            Console.ReadKey();
        }
    }
}