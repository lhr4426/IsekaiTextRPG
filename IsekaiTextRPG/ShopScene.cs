using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ShopScene : GameScene
{    // 메뉴 선택을 위한 상수 정의
    private const string MenuOptionBuy = "1";   // 아이템 구매
    private const string MenuOptionSell = "2";  // 아이템 판매
    private const string MenuOptionReturn = "0";// 이전 씬으로 돌아가기
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
    public ShopScene(ItemSystem itemSystem)
    {
        _itemSystem = itemSystem ?? throw new ArgumentNullException(nameof(itemSystem));
        _shopItems = new List<Item>
            {//이름 설명 공격력 방어력 가격 장착가능여부 아이템 타입 치명타 확률 치명타 배율 회피율
                new("칼","기본 검",10, 0, 0, 0, 100, true, Item.ItemType.Weapon, 0.05f, 1.5f, 0f),
                new("가죽 갑옷", "초급 가슴방어구", 0, 5, 0, 0, 150, true, Item.ItemType.BodyArmor, 0f, 0f, 0.01f),
                new("가죽 헬멧", "초급 머리보호구", 0, 3, 0, 0, 80, true, Item.ItemType.HeadArmor, 0f, 0f, 0.01f),
                new("가죽 바지", "초급 다리보호구", 0, 2, 0, 0, 50, true, Item.ItemType.LegArmor, 0f, 0f, 0.01f),
                new("체력 물약", "HP +50 회복", 0, 0, 50, 0, 50, false, Item.ItemType.Usable, 0f, 0f, 0f),
                new("마나 물약", "MP +30 회복", 0, 0, 0, 50,70, false, Item.ItemType.Usable, 0f, 0f, 0f)
            };
    }
    // 씬 이름: 상단 헤더에 표시
    public override string SceneName => "상점";
    // 씬 시작: 메인 루프에서 화면 갱신, 메뉴 선택, 처리
    public override GameScene? StartScene()
    {
        Console.Clear();
        UI.DrawTitledBox($"{SceneName} === 소지 골드: {GameManager.player.Gold}", null);
        DisplayItems(); // 아이템 목록 출력
        DisplayMenu();  // 메뉴 옵션 출력
        var input = Console.ReadLine()?.Trim() ?? string.Empty;
        switch (input)
        {
            case MenuOptionBuy:
                HandleBuy();
                return this;
            case MenuOptionSell:
                HandleSell();
                return this;
            case MenuOptionReturn:
                return prevScene;
            default:
                Console.WriteLine("유효한 번호를 입력하세요.");
                Console.ReadKey();
                return this;

        }

    }

    // 상점에 진열된 아이템 목록을 화면에 출력
    private void DisplayItems()
    {
        List<string> strings = new();
        if (_shopItems.Count == 0)
        {
            strings.Add("판매할 아이템이 없습니다.");
            strings.Add("");
            strings.Add("아무 키나 눌러 계속...");
            UI.DrawBox(strings);
            Console.ReadKey();
            return;
        }

        for (var i = 0; i < _shopItems.Count; i++)
        {
            var item = _shopItems[i];
            var status = (item.Type != Item.ItemType.Usable
                          && _itemSystem.HasItem(item.Name))
                         ? "[구매완료]" : string.Empty;

            // 한 줄로 아이템 정보를 포맷하여 출력
            strings.Add($"{i + 1}    |{item.Name}    |{_itemTypeNames[item.Type]}    |" +
                $"공격:{item.Attack}    |방어:{item.Defense}   |" +
                $"치명타:{item.CriticalRate:P0}    |치명타배율:{item.CriticalDamage}    |회피:{item.DodgeRate:P0}  |{item.Description}  |가격:{item.Price}  {status} ");

        }

        UI.DrawLeftAlignedBox(strings);
    }
    // 메뉴 옵션(구매/판매/돌아가기) 출력
    private void DisplayMenu()
    {
        List<string> strings = new();

        strings.Add("1 : 아이템 구매");
        strings.Add("2 : 아이템 판매");
        strings.Add("0 : 돌아가기");
        UI.DrawBox(strings);
        Console.Write(">> "); 

    }
    // 아이템 구매 처리: 번호 입력 → BuyItem 호출 → 결과 메시지
    private void HandleBuy()
    {
        Console.Write("\n구매할 아이템 번호 (0 : 취소) >> ");
        var input = Console.ReadLine()?.Trim() ?? string.Empty;

        if (int.TryParse(input, out var idx))
        {
            if (idx == 0) { return; }
            if (idx >= 1 && idx <= _shopItems.Count)
            {
                var item = _shopItems[idx - 1];
                int quantity = 1;
                if (item.Type == Item.ItemType.Usable)
                {
                    Console.Write("구매할 개수:");
                    if (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 1)
                    {
                        quantity = 1;
                    }
                }
                int purchased = 0;
                // 아이템이 장착 가능한 타입이고 이미 구매한 경우
                if (item.Type != Item.ItemType.Usable && _itemSystem.HasItem(item.Name))
                {
                    Console.WriteLine("이미 구매한 아이템입니다.");
                }
                else
                {       // 아이템 구매 반복
                    for (int i = 0; i < quantity; i++)
                    {
                        if (!_itemSystem.BuyItem(
                          item.Name,
                          item.Description,
                          item.Attack,
                          item.Defense,
                          item.Hp,
                          item.Mp,
                          item.Price,
                          item.IsEquip,
                          item.Type,
                          item.CriticalRate,
                          item.CriticalDamage,
                          item.DodgeRate,
                          suppressMessage: true))
                        {
                            break; // 골드 부족 시 반복 중단
                        }
                        purchased++; // 구매한 개수 증가
                    }
                    if (purchased > 0)
                        Console.WriteLine($"{item.Name} x{purchased} 구매 완료! 남은 골드: {GameManager.player.Gold}");
                    else
                        Console.WriteLine("골드가 부족하여 구매할 수 없습니다.");
                }
            }
            else
            {
                Console.WriteLine("유효한 번호를 입력하세요.");
            }
        }

        // 구매 후 대기
        Console.WriteLine("\n아무 키나 눌러 계속...");
        Console.ReadKey();
    }
    // 아이템 판매 처리: 이름 입력 → 장착 해제 → SellItem 호출 → 결과 메시지
    private void HandleSell()
    {
        Console.Clear();

        // 1) 인벤토리와 골드를 출력
        _itemSystem.ShowInventoryForSell();


        // 판매할 아이템 이름 입력
        Console.Write("\n판매할 아이템 번호 (0 : 취소) >> ");
        
        var input = Console.ReadLine()?.Trim() ?? string.Empty;
        var itemName = input;
        if (int.TryParse(input, out int sel))
        {
            var inv = GameManager.player.Inventory;
            var names = inv
            .Where(i => i.Type != Item.ItemType.Usable)
            .Select(i => i.Name)
            .ToList();

            names.AddRange(inv
            .Where(i => i.Type == Item.ItemType.Usable)
            .GroupBy(i => i.Name)
            .Select(g => g.Key));

            if (sel == 0) return;
            if (sel >= 1 && sel <= names.Count)
            {
                itemName = names[sel - 1];
            }   
            else
            {
                Console.WriteLine("유효한 번호를 입력하세요.");
                Console.ReadKey();
                return;
            }
        }
        else
        {
            Console.WriteLine("번호로만 판매가 가능합니다.");
            Console.ReadKey();
            return;
        }

        // 3) 플레이어가 장착 중인 경우 해제
        var player = GameManager.player;
        var equippedItem = player.EquippedItems.FirstOrDefault(i => i.Name == itemName);
        if (equippedItem != null)
        {
            player.EquippedItems.Remove(equippedItem);
            Console.WriteLine($"{equippedItem.Name}을(를) 장착 해제했습니다.");
        }

        // 4) 실제 판매 호출
        if (!_itemSystem.HasItem(itemName))
        {
            Console.WriteLine("판매할 아이템이 없습니다.");
        }
        else
        {
            var invItems = player.Inventory.Where(i => i.Name == itemName).ToList();
            if (invItems.First().Type == Item.ItemType.Usable)
            {
                Console.Write("판매할 개수: ");
                var qtyInput = Console.ReadLine()?.Trim() ?? string.Empty;
                if (!int.TryParse(qtyInput, out int qty) || qty < 1)
                    qty = 1;
                qty = Math.Min(qty, invItems.Count);

                if (!ConfirmSell(itemName, qty))  
                    return;

                int sold = 0;
                for (int i = 0; i < qty; i++)
                {
                    if (_itemSystem.SellItem(itemName))
                    {
                        sold++;
                    }
                    else break;
                }
                Console.WriteLine($"{itemName} x{sold} 판매 완료! 현재 골드: {GameManager.player.Gold}");
            }
            else
            {
                if (!ConfirmSell(itemName))
                    return;
                bool success = _itemSystem.SellItem(itemName);
            }
        }
        // 5) 판매 후 대기
        Console.WriteLine("\n아무 키나 눌러 계속...");
        Console.ReadKey();
    }

    private bool ConfirmSell(string itemName, int quantity = 1)
    {
        Console.WriteLine($"\n[{itemName} x{quantity}] 정말 판매하시겠습니까? (Y/N)");
        var confirm = Console.ReadLine()?.Trim().ToUpper();
        return confirm == "Y";
    }
}
