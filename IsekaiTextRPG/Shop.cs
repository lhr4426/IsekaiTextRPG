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
        public Shop(ItemSystem itemSystem)
        {
            _itemSystem = itemSystem ?? throw new ArgumentNullException(nameof(itemSystem));

            var allShopItems = new List<Item>
            {              // 데미지, 방어력, 가격, 착용 가능 여부, 아이템 타입, 치명타 확률, 치명타 데미지 배율, 회피율
                new Item("장비 이름", "장비 설명", 20, 0, 100, true, "무기", 0.1f, 1.6f, 0),
                new Item("장비 이름", "장비 설명", 15, 0, 80, true, "무기", 0.05f, 1.5f, 0),
                new Item("장비 이름", "장비 설명", 0, 0, 120, true, "방어구", 0.15f, 1.7f, 0),
                new Item("소모품 이름", "소모품 설명", 0, 0, 50, false, "소모품", 0, 0, 0f),
                new Item("소모품 이름", "소모품 설명", 0, 0, 30, false, "소모품", 0, 0, 0f)
            };             // 나중에 같이 의견 나눠볼것

            _equipmentShopItems = [.. allShopItems.Where(i => i.IsEquip)];
            _consumableShopItems = [.. allShopItems.Where(i => !i.IsEquip)];
        }

        public override string SceneName => "상점";

        public override GameScene? StartScene()
        {
            Console.Clear();
            Console.WriteLine($"\t=== 상점 ===    소지 골드: {_itemSystem.Gold}\n"); 
        }

        private void DisplayAndBuy(List<Item> items, string categoryName)
        {
            Console.Clear();
            Console.WriteLine($"\t=== {categoryName} 상점 ===    소지 골드: {_itemSystem.Gold}\n");
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                bool owned = _itemSystem.HasItem(item.Name);
                string status = owned ? "구매완료" : ""; // 아이템 소유 여부에 따라 상태 표시
                Console.WriteLine($"{i + 1}. {item.Name} / {item.Description} / 공격:{item.Attack}, 방어:{item.Defense}, 가격:{item.Price} / {status}");
            }

            Console.Write($"\n구매할 {categoryName} 번호: ");
            if (int.TryParse(Console.ReadLine(), out var idx) && idx >= 1 && idx <= items.Count)
            {
                var item = items[idx - 1];
                if (item.IsEquip || !item.IsEquip)
                {
                    bool success = _itemSystem.BuyItem(
                        name: item.Name,
                        description: item.Description,
                        attack: item.Attack,
                        defense: item.Defense,
                        price: item.Price,
                        isEquip: item.IsEquip,
                        itemType: item.ItemType,
                        criticalRate: item.CriticalRate,
                        criticalDamage: item.CriticalDamage,
                        dodgeRate: item.DodgeRate
                    );
                    Console.WriteLine(success ? $"{item.Name}을(를) 구매했습니다!" : "구매에 실패했습니다.");
                }
            }
            else
            {
                Console.WriteLine("번호를 입력하세요.");
            }
            Console.WriteLine("\n아무 키나 눌러 계속...");
            Console.ReadKey();

        }

        private void HandleSell()
        {
            Console.Clear();
            Console.Write("판매할 아이템 이름: ");
            var name = Console.ReadLine()?.Trim() ?? "";
            bool success = _itemSystem.SellItem(name);
            Console.WriteLine(success ? "판매 완료!" : "판매 실패 (인벤토리에 없거나 장착 중인 아이템)");
            Console.WriteLine("\n아무 키나 눌러 계속...");
            Console.ReadKey();
        }
    }
}
