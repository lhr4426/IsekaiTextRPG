using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsekaiTextRPG
{
    public class Shop : GameScene
    {    // 메뉴 선택을 위한 상수 정의
        private const string MenuOptionBuy = "1";   // 아이템 구매
        private const string MenuOptionSell = "2";  // 아이템 판매
        private const string MenuOptionReturn = "3";// 이전 씬으로 돌아가기

        private readonly ItemSystem _itemSystem;// 플레이어의 아이템/골드 관리 시스템
        private readonly List<Item> _shopItems; // 상점에 진열할 아이템 목록

        // enum 값을 사용자에게 보여줄 문자열로 매핑
        private static readonly Dictionary<Item.ItemType, string> _itemTypeNames = new()
        {
            {Item.ItemType.Weapon,"무기"},
            {Item.ItemType.BodyArmor,"방어구"},
            {Item.ItemType.HeadArmor,"투구"},
            {Item.ItemType.LegArmor,"다리보호구"},
            {Item.ItemType.Usable,"소모품"}
        };

        // 생성자: ItemSystem을 주입받고, 상점 아이템 목록을 초기화
        public Shop(ItemSystem itemSystem)
        {
            _itemSystem = itemSystem ?? throw new ArgumentNullException(nameof(itemSystem));

            _shopItems = new List<Item>
            {//이름 설명 공격력 방어력 가격 장착가능여부 아이템 타입 치명타 확률 치명타 배율 회피율
                new Item("칼","기본 검",10, 0, 100, true, Item.ItemType.Weapon, 0.05f, 1.5f, 0f),
                new Item("가죽 갑옷", "초급 가슴방어구", 0, 5,150, true, Item.ItemType.BodyArmor, 0f, 0f, 1f),
                new Item("가죽 헬멧", "초급 머리보호구", 0, 3, 80, true, Item.ItemType.HeadArmor, 0f, 0f, 1f),
                new Item("가죽 바지", "초급 다리보호구", 0, 2, 50, true, Item.ItemType.LegArmor, 0f, 0f, 1f),
                new Item("체력 물약", "HP +50 회복", 0, 0, 50, false, Item.ItemType.Usable, 0f, 0f, 0f),
                new Item("마나 물약", "MP +30 회복", 0, 0, 70, false, Item.ItemType.Usable, 0f, 0f, 0f)
            };
        }

        // 씬 이름: 상단 헤더에 표시
        public override string SceneName => "상점";

        // 씬 시작: 메인 루프에서 화면 갱신, 메뉴 선택, 처리
        public override GameScene? StartScene()
        {
            while (true) // '돌아가기' 선택 전까지 반복
            {
                Console.Clear();
                Console.WriteLine($"\t=== {SceneName} ===    소지 골드: {_itemSystem.Gold}\n");

                DisplayItems(); // 아이템 목록 출력
                DisplayMenu();  // 메뉴 옵션 출력

                var input = Console.ReadLine()?.Trim() ?? string.Empty;
                switch (input)
                {
                    case MenuOptionBuy:
                        HandleBuy();
                        break;
                    case MenuOptionSell:
                        HandleSell();
                        break;
                    case MenuOptionReturn:
                        return EndScene();
                    default:
                        Console.WriteLine("유효한 번호를 입력하세요.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // 상점에 진열된 아이템 목록을 화면에 출력
        private void DisplayItems()
        {
            if (_shopItems.Count == 0)
            {
                Console.WriteLine("판매할 아이템이 없습니다.\n");
                Console.WriteLine("아무 키나 눌러 계속...");
                Console.ReadKey();
                return;
            }

            for (var i = 0; i < _shopItems.Count; i++)
            {
                var item = _shopItems[i];
                var status = _itemSystem.HasItem(item.Name) ? "[구매완료]" : string.Empty;

                // enum을 한글명으로 변환
                var typeName = _itemTypeNames.TryGetValue(item.Type, out var name) ? name : item.Type.ToString();

                // 한 줄로 아이템 정보를 포맷하여 출력
                Console.WriteLine(
                    $"{i + 1} | {item.Name} | {typeName} | {item.Description} | " +
                    $"공격: {item.Attack} | 방어: {item.Defense} | 가격: {item.Price} | " +
                    $"치명타: {item.CriticalRate:P0} | 배율: {item.CriticalDamage} | 회피: {item.DodgeRate:P0} {status}"
                );
            }

            Console.WriteLine();
        }

        // 메뉴 옵션(구매/판매/돌아가기) 출력
        private void DisplayMenu()
        {
            Console.WriteLine("1 | 아이템 구매");
            Console.WriteLine("2 | 아이템 판매");
            Console.WriteLine("3 | 돌아가기");
            Console.Write("\n선택: ");
        }

        // 아이템 구매 처리: 번호 입력 → BuyItem 호출 → 결과 메시지
        private void HandleBuy()
        {
            Console.Write("\n구매할 아이템 번호: ");
            var input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (int.TryParse(input, out var idx)
                && idx >= 1
                && idx <= _shopItems.Count)
            {
                var item = _shopItems[idx - 1];
                if (_itemSystem.HasItem(item.Name))
                {
                    Console.WriteLine("이미 구매한 아이템입니다.");
                }
                else
                {// 구매 시도: 성공하면 골드 차감 및 인벤토리에 추가
                    var success = _itemSystem.BuyItem(
                        item.Name,
                        item.Description,
                        item.Attack,
                        item.Defense,
                        item.Price,
                        item.IsEquip,
                        item.Type.ToString(),
                        item.CriticalRate,
                        item.CriticalDamage,
                        item.DodgeRate
                    );

                    Console.WriteLine(
                        success
                            ? $"{item.Name}을(를) 구매했습니다!"
                            : "구매에 실패했습니다."
                    );
                }
            }
            else
            {
                Console.WriteLine("유효한 번호를 입력하세요.");
            }
            // 구매 후 대기
            Console.WriteLine("\n아무 키나 눌러 계속...");
            Console.ReadKey();
        }

        // 아이템 판매 처리: 이름 입력 → 장착 해제 → SellItem 호출 → 결과 메시지
        private void HandleSell()
        {
            Console.Write("\n판매할 아이템 이름: ");
            var name = Console.ReadLine()?.Trim() ?? string.Empty;

            // 판매 전에 장착 중인 아이템이면 해제
            var player = GameManager.player;
            var equippedItem = player.EquippedItems.FirstOrDefault(i => i.Name == name);
            if (equippedItem != null)
            {
                player.EquippedItems.Remove(equippedItem);
                Console.WriteLine($"{name}의 장착을 해제했습니다.");
            }

            if (!_itemSystem.HasItem(name))
            {
                Console.WriteLine("판매할 아이템이 없습니다.");
            }
            else
            { // 판매 시도: 성공하면 골드 증가 및 인벤토리에서 제거
                var success = _itemSystem.SellItem(name);
                Console.WriteLine(
                    success
                        ? $"{name}을(를) 판매했습니다!"
                        : "판매에 실패했습니다."
                );
            }

            Console.WriteLine("\n아무 키나 눌러 계속...");
            Console.ReadKey();
        }
    }
}