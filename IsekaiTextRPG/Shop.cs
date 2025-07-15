using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsekaiTextRPG
{
    public class Shop : GameScene
    {
        private readonly ItemSystem _itemSystem;
        private readonly List<Item> _equipmentShopItems;
        private readonly List<Item> _consumableShopItems;

    }
      public Shop(ItemSystem itemSystem)
        {
            _itemSystem = itemSystem ?? throw new ArgumentNullException(nameof(itemSystem));

            var allShopItems = new List<Item>
            {
                new Item("칼", "기본 검",        10, 0,  100, true,  "무기",    0.05f, 1.5f, 0),
                new Item("가죽 갑옷", "초급 방어구",   0,  5,  150, true,  "방어구", 0.00f, 1.0f, 0),
                new Item("체력 물약", "HP +50 회복",  0,  0,   50, false, "소모품", 0.00f, 1.0f, 0),
                new Item("마나 물약", "MP +30 회복",   0,  0,   70, false, "소모품", 0.00f, 1.0f, 0)
            };

            _equipmentShopItems = allShopItems.Where(i => i.IsEquip).ToList();
            _consumableShopItems = allShopItems.Where(i => !i.IsEquip).ToList();
        }

        public override string SceneName => "상점";

        public override GameScene? StartScene()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\t=== {SceneName} ===    소지 골드: {_itemSystem.Gold}\n");
                Console.WriteLine("1. 장비 구매");
                Console.WriteLine("2. 소모품 구매");
                Console.WriteLine("3. 아이템 판매");
                Console.WriteLine("4. 돌아가기");
                Console.Write("\n선택: ");

                var choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1":
                        DisplayAndBuy(_equipmentShopItems, "장비");
                        break;
                    case "2":
                        DisplayAndBuy(_consumableShopItems, "소모품");
                        break;
                    case "3":
                        HandleSell();
                        break;
                    case "0":
                        EndScene();
                        break;
                    default:
                        Console.WriteLine("유효한 번호를 입력하세요.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void DisplayAndBuy(List<Item> items, string categoryName)
        {
            Console.Clear();
            Console.WriteLine($"\t=== {categoryName} 상점 ===    소지 골드: {_itemSystem.Gold}\n");
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                bool owned = _itemSystem.HasItem(item.Name);
                string status = owned ? "구매완료" : "";
                Console.WriteLine($"{i + 1}. {item.Name} / {item.Description} / 공격:{item.Attack}, 방어:{item.Defense}, 가격:{item.Price} / {status}");
            }

            Console.Write($"\n구매할 {categoryName} 번호: ");
            if (int.TryParse(Console.ReadLine(), out int idx)
                && idx >= 1
                && idx <= items.Count)
            {
                var selected = items[idx - 1];
                bool success = _itemSystem.BuyItem(
                    selected.Name,
                    selected.Description,
                    selected.Attack,
                    selected.Defense,
                    selected.Price,
                    selected.IsEquip,
                    selected.ItemType,
                    selected.CriticalRate,
                    selected.CriticalDamage,
                    selected.DodgeRate
                );

                Console.WriteLine(success
                    ? $"{selected.Name}을(를) 구매했습니다!"
                    : "구매에 실패했습니다.");
            }
            else
            {
                Console.WriteLine("유효한 번호를 입력하세요.");
            }

            Console.WriteLine("\n아무 키나 눌러 계속...");
            Console.ReadKey();
        }

        private void HandleSell()
        {
            Console.Clear();
            Console.Write("판매할 아이템 이름: ");
            var name = Console.ReadLine()?.Trim() ?? "";

            // 장착 해제 (ItemSystem에 구현된 경우 호출)
            if (_itemSystem.IsEquipped(name))
            {
                _itemSystem.UnequipItem(name);
                Console.WriteLine($"{name}의 장착을 해제했습니다.");
            }

            bool success = _itemSystem.SellItem(name);
            Console.WriteLine(success
                ? $"{name}을(를) 판매했습니다!"
                : "판매에 실패했습니다.");

            Console.WriteLine("\n아무 키나 눌러 계속...");
            Console.ReadKey();
        }
    }
}