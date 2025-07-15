using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryScene : GameScene
{
    public override string SceneName => "인벤토리";

    public override GameScene? StartScene()
    {
        Player player = GameManager.Instance.Player;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("인벤토리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");

            var sortedInventory = player.Inventory
                .OrderByDescending(item => player.EquippedItems.Contains(item)) // 장착 여부
                .ThenBy(item => item.Type)                                      // 아이템 타입
                .ThenBy(item => item.Name)                                      // 이름순
                .ToList();

            Console.WriteLine("[아이템 목록]");
            if (sortedInventory.Count == 0)
            {
                Console.WriteLine(" - 아이템이 없습니다.");
            }
            else
            {
                int index = 1;
                foreach (var item in sortedInventory)
                {
                    string equippedMark = player.EquippedItems.Contains(item) ? "[E]" : "   ";
                    string stats = item.Attack > 0 ? $"공격력 +{item.Attack}" : $"방어력 +{item.Defense}";
                    Console.WriteLine($"- {index} {equippedMark}{item.Name,-15} | {stats} | {item.Description}");
                    index++;
                }
            }

            Console.WriteLine("\n1. 장착/해제 관리");
            Console.WriteLine("0. 나가기");
            Console.Write("\n원하시는 행동을 입력해주세요.\n>> ");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    HandleEquip(player, sortedInventory);
                    break;
                case "0":
                    return GameManager.Instance.GetScene(SceneManager.SceneType.TownScene);
                default:
                    Console.WriteLine("잘못된 입력입니다. 아무 키나 눌러 계속...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void HandleEquip(Player player, List<Item> inventory)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("인벤토리 - 장착 관리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");

            Console.WriteLine("[아이템 목록]");
            for (int i = 0; i < inventory.Count; i++)
            {
                var item = inventory[i];
                string equippedMark = player.EquippedItems.Contains(item) ? "[E]" : "   ";
                string stats = item.Attack > 0 ? $"공격력 +{item.Attack}" : $"방어력 +{item.Defense}";
                Console.WriteLine($"- {i + 1} {equippedMark}{item.Name,-15} | {stats} | {item.Description}");
            }

            Console.WriteLine("\n0. 나가기");
            Console.Write("\n원하시는 행동을 입력해주세요.\n>> ");
            string? input = Console.ReadLine();

            if (input == "0") return;

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= inventory.Count)
            {
                var selectedItem = inventory[choice - 1];

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
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
            }

            Console.WriteLine("\n계속하려면 아무 키나 누르세요...");
            Console.ReadKey();
        }
    }
}