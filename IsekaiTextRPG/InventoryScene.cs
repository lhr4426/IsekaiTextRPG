using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryScene : GameScene
{
    public override string SceneName => "인벤토리"; // 씬 이름

    public override GameScene? StartScene()
    {
        Console.Clear(); // 화면 초기화
        Player player = GameManager.player; // 현재 플레이어 객체 가져오기

        List<string> strings = new List<string>();

        // 인벤토리 아이템을 정렬합니다. (장착 여부 -> 타입 -> 이름 순)
        var sortedInventory = player.Inventory
            .OrderByDescending(item => player.EquippedItems.Contains(item)) // 장착 여부 (장착된 것이 먼저 오도록)
            .ThenBy(item => item.Type)                                      // 아이템 타입 (무기, 방어구, 소모품 등)
            .ThenBy(item => item.Name)                                      // 이름순
            .ToList();

        if (sortedInventory.Count == 0) // 인벤토리가 비어있을 경우
        {
            strings.Add(" - 아이템이 없습니다.");
        }
        else
        {
            int index = 1;
            // 장착 가능한 아이템(무기, 방어구)을 먼저 표시
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

                strings.Add(
                    $" - {index} {equippedMark}{item.Name,-15} | {statText}{item.Description}"
                );
                index++;
            }


            // 소비 아이템을 이름으로 그룹화하여 표시
            foreach (var item in sortedInventory.Where(i => i.Type == Item.ItemType.Usable))
            {
                string equippedMark = "   "; // 소비 아이템은 항상 빈칸
                strings.Add(
                    $"- {index} {equippedMark}{item.Name,-15} x{item.ItemCount} | {item.Description}"
                );
                index++;
            }
        }

        strings.Add(" 1. 장착/해제 관리"); // 장착/해제 선택 메뉴
        strings.Add(" 0. 나가기"); // 나가기 메뉴

        UI.DrawTitledBox(SceneName, null); // UI 박스 그리기
        UI.DrawLeftAlignedBox(strings); // UI 박스 그리기

        Console.Write(">> ");

        // 사용자 입력 받기 (0 또는 1)
        int? input = InputHelper.InputNumber(0, 1);

        switch (input)
        {
            case 1:
                // HandleEquip 메서드 호출 시, 장착 가능한 아이템만 넘겨줍니다.
                HandleEquip(player, sortedInventory.Where(i => i.IsEquip).ToList());
                return this; // 현재 씬 유지
            case 0:
                return prevScene; // 이전 씬으로 돌아가기
            default:
                Console.WriteLine("잘못된 입력입니다. 아무 키나 눌러 계속...");
                Console.ReadKey();
                return this; // 현재 씬 유지
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
    private void HandleEquip(Player player, List<Item> equippableItems) // 'inventory' 매개변수 이름을 'equippableItems'로 변경
    {
        while (true)
        {
            Console.Clear(); // 화면 초기화

            List<string> strings = new List<string>()
            {
                " 장착/해제할 아이템을 선택하세요.", // 안내 메시지 수정
                "",
                " [아이템 목록]",
                ""
            };

            // 장착 가능한 아이템 목록을 표시
            if (equippableItems.Count == 0)
            {
                strings.Add(" - 장착 가능한 아이템이 없습니다.");
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
                    strings.Add($"- {i + 1} {equippedMark}{item.Name,-15} | {stats} {(stats != "" ? "| " : "")}{item.Description}");
                }

            }

            strings.Add("");
            strings.Add("0. 나가기"); // 나가기 메뉴

            UI.DrawTitledBox("인벤토리 - 장착 관리", null);// UI 박스 그리기
            UI.DrawLeftAlignedBox(strings);


            Console.Write(">> ");

            // 사용자 입력 받기 (0부터 장착 가능한 아이템 개수까지)
            int? input = InputHelper.InputNumber(0, equippableItems.Count);

            if (input == 0) return; // 0 입력 시 메서드 종료
            if (input == null) continue; // 유효하지 않은 입력 시 다시 반복

            // 선택된 아이템 가져오기
            var selectedItem = equippableItems[(int)input - 1];

            // ⚡️ 중요한 변경: Player 클래스의 EquipItem 또는 UnequipItem 메서드 호출 ⚡️
            if (player.EquippedItems.Contains(selectedItem))
            {
                // 이미 장착된 아이템이면 해제
                player.UnequipItem(selectedItem); // Player.UnequipItem 호출
            }
            else
            {
                // 장착되지 않은 아이템이면 장착
                player.EquipItem(selectedItem); // Player.EquipItem 호출
            }

            Console.ReadKey(); // 사용자 입력 대기
        }
    }
}