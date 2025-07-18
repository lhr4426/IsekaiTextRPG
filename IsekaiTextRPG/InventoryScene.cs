using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryScene : GameScene
{
    public override string SceneName => "인벤토리"; // 씬 이름

    public override GameScene? StartScene()
    {
        Console.Clear(); // 화면 초기화
        Player player = GameManager.player;

        List<string> lines = new(); // 최종 출력줄
        List<string> itemLines = new(); // 아이템 줄만 모음

        // 정렬: 장착 여부 → 타입 → 이름
        var sortedInventory = player.Inventory
            .OrderByDescending(item => player.EquippedItems.Contains(item))
            .ThenBy(item => item.Type)
            .ThenBy(item => item.Name)
            .ToList();

        if (sortedInventory.Count == 0)
        {
            lines.Add(" - 아이템이 없습니다.");
        }
        else
        {
            int index = 1;

            // 장비 아이템
            foreach (var item in sortedInventory.Where(i => i.Type != Item.ItemType.Usable))
            {
                string equippedMark = player.EquippedItems.Contains(item) ? "[E]" : "   ";

                List<string> statParts = new();
                if (item.Attack > 0) statParts.Add($"공격력 +{item.Attack}");
                if (item.Defense > 0) statParts.Add($"방어력 +{item.Defense}");
                if (item.CriticalRate > 0) statParts.Add($"치명타율 {item.CriticalRate:P0}");
                if (item.CriticalDamage > 1f) statParts.Add($"치명타배율 {item.CriticalDamage:F1}");
                if (item.DodgeRate > 0) statParts.Add($"회피율 {item.DodgeRate:P0}");

                string statText = statParts.Count > 0 ? string.Join(" | ", statParts) + " | " : "";

                string line = $" - {index} {equippedMark}{item.Name,-15} | {statText}{item.Description}";
                itemLines.Add(line);
                index++;
            }

            // 소비 아이템 (이름+설명 기준으로 그룹화)
            foreach (var group in sortedInventory
                        .Where(i => i.Type == Item.ItemType.Usable)
                        .GroupBy(i => new { i.Name, i.Description }))
            {
                int totalCount = group.Sum(i => i.ItemCount);
                if (totalCount <= 0) continue;

                string line = $"- {index}    {group.Key.Name,-15} x{totalCount} | {group.Key.Description}";
                itemLines.Add(line);
                index++;
            }

            // 공통 너비 계산
            int maxDisplayWidth = itemLines.Select(UI.GetDisplayWidth).Max();

            // 줄 삽입 + 구분선 통일
            for (int i = 0; i < itemLines.Count; i++)
            {
                lines.Add(itemLines[i]);
                if (i < itemLines.Count - 1)
                {
                    lines.Add(new string('─', maxDisplayWidth + 2)); // 통일된 길이
                }
            }
        }

        // 메뉴 출력도 동일한 폭 기준
        List<string> menu = new()
        {
            " 1. 장착/해제 관리",
            " 0. 나가기"
        };

        UI.DrawTitledBox(SceneName, null);
        UI.DrawLeftAlignedBox(lines);
        UI.DrawLeftAlignedBox(menu, lines.Any() ? UI.GetMaxWidth(lines) : UI.GetMaxWidth(menu));

        Console.Write(">> ");
        int? input = InputHelper.InputNumber(0, 1);

        switch (input)
        {
            case 1:
                HandleEquip(player, sortedInventory.Where(i => i.IsEquip).ToList());
                return this;
            case 0:
                return prevScene;
            default:
                Console.WriteLine("잘못된 입력입니다. 아무 키나 눌러 계속...");
                Console.ReadKey();
                return this;
        }
    }


    // 사용 가능한 아이템 목록을 출력하는 메서드 (현재 사용되지 않을 수 있으나, 확장성을 위해 유지)
    public void PrintUsableItems(out List<Item> inven)
    {
        Player player = GameManager.player;
        List<string> strings = new();

        var grouped = player.Inventory
            .Where(item => item.Type == Item.ItemType.Usable)
            .GroupBy(item => item.Name)
            .Select(g =>
            {
                var baseItem = g.First(); // 첫 번째 아이템 복사
                baseItem.ItemCount = g.Sum(x => x.ItemCount); // 총 개수 반영
                return baseItem;
            })
            .OrderBy(item => item.Name)
            .ToList();

        if (grouped.Count > 0)
        {
            int displayIndex = 1;
            foreach (var item in grouped)
            {
                List<string> statParts = new(); // 스탯 설명 구성
                if (item.Attack > 0) statParts.Add($"공격력 +{item.Attack}");
                if (item.Defense > 0) statParts.Add($"방어력 +{item.Defense}");
                if (item.Hp > 0) statParts.Add($"HP +{item.Hp}");
                if (item.Mp > 0) statParts.Add($"MP +{item.Mp}");

                string stats = statParts.Count > 0 ? string.Join(" | ", statParts) : "";
                strings.Add($" - {displayIndex} {item.Name,-15} x{item.ItemCount} | " +
                            $"{(stats != "" ? stats + " | " : "")}{item.Description}");
                displayIndex++;
            }
        }
        else
        {
            strings.Add("사용 가능한 아이템이 없습니다.");
        }

        UI.DrawLeftAlignedBox(strings);

        // 정확한 수량 포함한 usable 아이템 반환
        inven = grouped;
    }


    // 아이템 장착/해제를 처리하는 메서드
    private void HandleEquip(Player player, List<Item> equippableItems)
    {
        while (true)
        {
            Console.Clear();

            List<string> lines = new()
            {
                " 장착/해제할 아이템을 선택하세요.",
                "",
                " [아이템 목록]",
                ""
            };

            List<string> itemLines = new();

            if (equippableItems.Count == 0)
            {
                itemLines.Add(" - 장착 가능한 아이템이 없습니다.");
            }
            else
            {
                for (int i = 0; i < equippableItems.Count; i++)
                {
                    var item = equippableItems[i];
                    string equippedMark = player.EquippedItems.Contains(item) ? "[E]" : "   ";

                    List<string> statParts = new();
                    if (item.Attack > 0) statParts.Add($"공격력 +{item.Attack}");
                    if (item.Defense > 0) statParts.Add($"방어력 +{item.Defense}");
                    if (item.Hp > 0) statParts.Add($"HP +{item.Hp}");
                    if (item.Mp > 0) statParts.Add($"MP +{item.Mp}");
                    if (item.CriticalRate > 0) statParts.Add($"치명타율 {item.CriticalRate:P0}");
                    if (item.CriticalDamage > 1f) statParts.Add($"치명타배율 {item.CriticalDamage:F1}");
                    if (item.DodgeRate > 0) statParts.Add($"회피율 {item.DodgeRate:P0}");

                    string stats = statParts.Count > 0 ? string.Join(" | ", statParts) : "";
                    string descriptionPart = item.Description;
                    string line = $"- {i + 1} {equippedMark}{item.Name,-15}";

                    if (!string.IsNullOrEmpty(stats))
                        line += $" | {stats}";

                    if (!string.IsNullOrEmpty(descriptionPart))
                        line += $" | {descriptionPart}";

                    itemLines.Add(line);
                }
            }

            // 🟨 통일된 DisplayWidth 기반 구분선
            int maxWidth = itemLines.Any() ? itemLines.Select(UI.GetDisplayWidth).Max() : 0;

            for (int i = 0; i < itemLines.Count; i++)
            {
                lines.Add(itemLines[i]);

                if (i < itemLines.Count - 1)
                    lines.Add(new string('─', maxWidth + 2));
            }

            List<string> menu = new()
            {
                "0. 나가기"
            };

            UI.DrawTitledBox("인벤토리 - 장착 관리", null);
            UI.DrawLeftAlignedBox(lines);
            UI.DrawLeftAlignedBox(menu, Math.Max(maxWidth + 4, UI.GetMaxWidth(menu)));

            Console.Write(">> ");
            int? input = InputHelper.InputNumber(0, equippableItems.Count);

            if (input == 0) return;
            if (input == null) continue;

            var selectedItem = equippableItems[(int)input - 1];

            if (player.EquippedItems.Contains(selectedItem))
            {
                player.UnequipItem(selectedItem);
            }
            else
            {
                player.EquipItem(selectedItem);
            }

            Console.ReadKey();
        }
    }

}