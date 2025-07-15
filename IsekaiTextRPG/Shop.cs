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
        private readonly List<Item> _shopItems;


        // 생성자: ItemSystem을 주입받아 필드에 저장하고, 진열용 아이템 리스트 초기화
        public Shop(ItemSystem itemSystem)
        {
            _itemSystem = itemSystem ?? throw new ArgumentNullException(nameof(itemSystem));

            // 상점 진열용 아이템
            _shopItems = new List<Item>
            {  //아이템이름 아이템설명 데미지 방어력 가격 장착 가능여부 아이템종류 치명타확률 치명타데미지배율 회피율
                new Item("칼", "기본 검", 10, 0, 100, true, Item.ItemType.Weapon, 0.05f, 1.5f, 0),
                new Item("가죽 갑옷", "초급 가슴방어구", 0, 5, 150, true, Item.ItemType.BodyArmor, 0f, 0f, 1),
                new Item("가죽 헬멧", "초급 머리보호구", 0, 3, 80, true, Item.ItemType.HeadArmor, 0f, 0f, 1),
                new Item("가죽 바지", "초급 다리보호구", 0, 2, 50, true, Item.ItemType.LegArmor, 0f, 0f, 1),
                new Item("체력 물약", "HP +50 회복", 0, 0, 50, false, Item.ItemType.Usable, 0f, 0f, 0),
                new Item("마나 물약", "MP +30 회복", 0, 0, 70, false, Item.ItemType.Usable, 0f, 0f, 0)
};
        }

        // 씬 이름을 반환, 상단 헤더에 사용
        public override string SceneName => "상점";
        // 씬 시작 시 실행되는 메인 루프
        public override GameScene? StartScene()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\t=== {SceneName} ===    소지 골드: {_itemSystem.Gold}\n");

                // 1) 아이템 목록 표시
                for (var i = 0; i < _shopItems.Count; i++)
                {
                    var item = _shopItems[i];
                    var status = _itemSystem.HasItem(item.Name) ? "[구매완료]" : "";// 이미 구매한 아이템은 [구매완료] 태그 추가

                    // 상점 아이템 목록에서 각 아이템을 번호, 이름, 설명, 공격력, 방어력, 구매 상태를 한 줄로 포맷해 출력
                    $"{i + 1}. {item.Name} / {item.Description} / " +
                    $"공격: {item.Attack}, 방어: {item.Defense}, 가격: {item.Price}, " +
                    $"치명타 확률: {item.CriticalRate:P0}, 치명타 배율: {item.CriticalDamage}, 회피율: {item.DodgeRate:P0} {status}"
                }

                // 2) 메뉴
                Console.WriteLine("\n1. 아이템 구매");
                Console.WriteLine("2. 아이템 판매");
                Console.WriteLine("3. 돌아가기");
                

                var input = Console.ReadLine()?.Trim();
                switch (input)
                {
                    case "1":
                        HandleBuy();
                        break;
                    case "2":
                        HandleSell();
                        break;
                    case "3":
                        return EndScene();
                    default:
                        Console.WriteLine("유효한 번호를 입력하세요.");
                        Console.ReadKey();
                        break;
                }
            }
        }
        // 아이템 구매 처리 메서드
        private void HandleBuy()
        {
            Console.Write("\n구매할 아이템 번호: ");
            if (int.TryParse(Console.ReadLine(), out var idx)
                && idx >= 1
                && idx <= _shopItems.Count)
            {
                var item = _shopItems[idx - 1];

                if (_itemSystem.HasItem(item.Name))
                {
                    Console.WriteLine("이미 구매한 아이템입니다.");
                }
                else
                {
                    // BuyItem 호출: 성공 시 골드 차감 및 인벤토리에 추가
                    bool success = _itemSystem.BuyItem(
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
        // 아이템 판매 처리 메서드
        private void HandleSell()
        {
            Console.Write("\n판매할 아이템 이름: ");
            var name = Console.ReadLine()?.Trim() ?? string.Empty;

            // 판매 전에 플레이어 장착 아이템 리스트에서 해당 아이템이 있으면 해제
            var player = GameManager.player;
            var equipped = player.EquippedItems.FirstOrDefault(i => i.Name == name);
            if (equipped != null)
            {
                player.EquippedItems.Remove(equipped);
                Console.WriteLine($"{name}의 장착을 해제했습니다.");
            }

            if (!_itemSystem.HasItem(name))
            {
                // 인벤토리에 없으면 판매 불가 메시지
                Console.WriteLine("판매할 아이템이 없습니다.");
            }
            else
            {  // SellItem 호출: 성공 시 골드 증가 및 인벤토리에서 제거
                var success = _itemSystem.SellItem(name);
                Console.WriteLine(
                    success
                        ? $"{name}을(를) 판매했습니다!"
                        : "판매에 실패했습니다."
                );
            }
            //판매후 대기
            Console.WriteLine("\n아무 키나 눌러 계속...");
            Console.ReadKey();
        }
    }
}