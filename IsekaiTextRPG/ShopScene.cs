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
            {Item.ItemType.LegArmor,"각반"},
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
                new("가죽 바지", "초급 각반", 0, 2, 0, 0, 50, true, Item.ItemType.LegArmor, 0f, 0f, 0.01f),
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

        int maxWidth = 0;
        List<string> displayLines = new();

        for (var i = 0; i < _shopItems.Count; i++)
        {
            var item = _shopItems[i];
            var status = (item.Type != Item.ItemType.Usable && _itemSystem.HasItem(item.Name)) ? "[구매완료]" : "";

            string line = string.Format(
                "{0,2} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} | {9}",
                i + 1,
                PadToWidth(item.Name, 10),
                PadToWidth(_itemTypeNames[item.Type], 6),
                PadToWidth(item.Attack > 0 ? $"공격:{item.Attack}" : "", 8),
                PadToWidth(item.Defense > 0 ? $"방어:{item.Defense}" : "", 8),
                PadToWidth(item.CriticalRate > 0 ? $"치명타:{item.CriticalRate:P0}" : "", 10),
                PadToWidth(item.CriticalDamage > 1 ? $"배율:{item.CriticalDamage:F1}" : "", 10),
                PadToWidth(item.DodgeRate > 0 ? $"회피:{item.DodgeRate:P0}" : "", 10),
                PadToWidth(item.Description, 20),
                PadToWidth($"가격:{item.Price}", 10) + status
            );

            displayLines.Add(line);
            maxWidth = Math.Max(maxWidth, line.Length); // 가장 긴 줄 길이 기록
        }

        // 이후 줄과 줄 사이에 고정 너비 구분선 추가
        for (int i = 0; i < displayLines.Count; i++)
        {
            strings.Add(displayLines[i]);

            if (i < displayLines.Count - 1)
                strings.Add(new string('─', maxWidth+20)); // 고정 너비의 선 추가
        }


        UI.DrawLeftAlignedBox(strings);
    }
    // 출력될 실제 너비를 계산 (한글은 2칸)
    public static int GetDisplayWidth(string s)
    {
        int width = 0;
        foreach (char c in s)
        {
            width += (c >= 0xAC00 && c <= 0xD7A3) ? 2 : 1;
        }
        return width;
    }

    // 지정된 출력 너비까지 패딩 (오른쪽 정렬 가능)
    public static string PadToWidth(string s, int totalWidth)
    {
        int displayWidth = GetDisplayWidth(s);
        int padding = Math.Max(0, totalWidth - displayWidth);
        return s + new string(' ', padding);
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
            if (idx == 0) return;
            if (idx >= 1 && idx <= _shopItems.Count)
            {
                var item = _shopItems[idx - 1];
                int quantity = 1;

                // 소모품일 경우 수량 입력 받기
                if (item.Type == Item.ItemType.Usable)
                {
                    Console.Write("구매할 개수: ");
                    if (!int.TryParse(Console.ReadLine(), out quantity) || quantity < 1)
                        quantity = 1;
                }

                int totalPrice = item.Price * quantity;
                if (GameManager.player.Gold < totalPrice)
                {
                    Console.WriteLine("골드가 부족합니다.");
                    return;
                }

                var inventory = GameManager.player.Inventory;

                if (item.Type == Item.ItemType.Usable)
                {
                    var existing = inventory.FirstOrDefault(i => i.Name == item.Name);
                    if (existing != null)
                    {
                        existing.ItemCount += quantity;
                    }
                    else
                    {
                        var newItem = new Item(item.Name, item.Description, item.Attack, item.Defense, item.Hp, item.Mp,
                                            item.Price, item.IsEquip, item.Type, item.CriticalRate, item.CriticalDamage, item.DodgeRate);
                        newItem.ItemCount = quantity;
                        inventory.Add(newItem);
                    }
                }
                else
                {
                    if (_itemSystem.HasItem(item.Name))
                    {
                        Console.WriteLine("이미 구매한 아이템입니다.");
                        Console.ReadKey();
                        return;
                    }

                    var newItem = new Item(item.Name, item.Description, item.Attack, item.Defense, item.Hp, item.Mp,
                                        item.Price, item.IsEquip, item.Type, item.CriticalRate, item.CriticalDamage, item.DodgeRate);
                    inventory.Add(newItem);
                }

                GameManager.player.Gold -= totalPrice;
                Console.WriteLine($"{item.Name} x{quantity} 구매 완료! 남은 골드: {GameManager.player.Gold}");
            }
            else
            {
                Console.WriteLine("유효한 번호를 입력하세요.");
            }
        }

        Console.WriteLine("\n아무 키나 눌러 계속...");
        Console.ReadKey();
    }
    // 아이템 판매 처리: 이름 입력 → 장착 해제 → SellItem 호출 → 결과 메시지
    private void HandleSell()
    {
        Console.Clear();
        _itemSystem.ShowInventoryForSell();

        Console.Write("\n판매할 아이템 번호 (0 : 취소) >> ");
        var input = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!int.TryParse(input, out int sel))
        {
            Console.WriteLine("번호로만 판매가 가능합니다.");
            Console.ReadKey();
            return;
        }

        var inv = GameManager.player.Inventory;
        var names = inv.Select(i => i.Name).Distinct().ToList();

        if (sel == 0) return;
        if (sel < 1 || sel > names.Count)
        {
            Console.WriteLine("유효한 번호를 입력하세요.");
            Console.ReadKey();
            return;
        }

        var itemName = names[sel - 1];
        var target = inv.First(i => i.Name == itemName);

        if (target.Type == Item.ItemType.Usable)
        {
            Console.Write("판매할 개수: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty < 1)
                qty = 1;

            qty = Math.Min(qty, target.ItemCount);
            if (!ConfirmSell(itemName, qty)) return;

            target.ItemCount -= qty;
            int sellPricePer = (int)Math.Round(target.Price * 0.85);
            GameManager.player.Gold += sellPricePer;

            if (target.ItemCount <= 0)
                inv.Remove(target);

            Console.WriteLine($"{itemName} x{qty} 판매 완료! 현재 골드: {GameManager.player.Gold}");
        }
        else
        {
            if (!ConfirmSell(itemName)) return;

            GameManager.player.EquippedItems.RemoveAll(i => i.Name == itemName);
            inv.Remove(target);
            int sellPricePer = (int)Math.Round(target.Price * 0.85);
            GameManager.player.Gold += sellPricePer;

            Console.WriteLine($"{itemName} 판매 완료! 현재 골드: {GameManager.player.Gold}");
        }

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
