using System;
using System.Collections.Generic;
using System.Linq;

namespace IsekaiTextRPG
{
    public class Player
    {
        public string Name { get; private set; }
        public int Level { get; private set; } = 1;
        public int Experience { get; private set; } = 0;
        public int Gold { get; set; } = 1000;

        public int MaxHP { get; private set; } = 100;
        public int CurrentHP { get; set; } = 100;
        public int MaxMP { get; private set; } = 50;
        public int CurrentMP { get; set; } = 50;

        public int BaseAttack { get; private set; } = 10;
        public int BaseDefense { get; private set; } = 5;

        public float CritRate { get; private set; } = 0.1f;       // 10%
        public float CritMultiplier { get; private set; } = 1.6f; // 150% 데미지
        public float DodgeRate { get; private set; } = 0.1f;      // 10%

        public List<Item> Inventory { get; private set; } = new List<Item>();
        public List<Item> EquippedItems { get; private set; } = new List<Item>();
        public List<Item> ShopItems { get; set; } = new List<Item>();

        public Player(string name)
        {
            Name = name;
        }

        // 총 공격력 계산 (기본 + 장착 아이템)
        public int GetTotalAttack()
        {
            int itemBonus = EquippedItems.Sum(item => item.AttackBonus);
            return BaseAttack + itemBonus;
        }

        // 총 방어력 계산 (기본 + 장착 아이템)
        public int GetTotalDefense()
        {
            int itemBonus = EquippedItems.Sum(item => item.DefenseBonus);
            return BaseDefense + itemBonus;
        }

        // 치명타 여부 판단
        public bool IsCriticalHit()
        {
            return new Random().NextDouble() < CritRate;
        }

        // 회피 여부 판단
        public bool TryDodge()
        {
            return new Random().NextDouble() < DodgeRate;
        }

        // 경험치 획득 및 레벨업
        public void GainExp(int amount)
        {
            Experience += amount;
            while (Experience >= Level * 100)
            {
                Experience -= Level * 100;
                LevelUp();
            }
        }

        // 레벨업 시 능력치 증가
        private void LevelUp()
        {
            Level++;
            MaxHP += 20;
            MaxMP += 10;
            BaseAttack += 2;
            BaseDefense += 1;
            CritRate += 0.01f;
            DodgeRate += 0.01f;
            CritMultiplier += 0.1f;

            Console.WriteLine($"\n[레벨 업!] {Level} 레벨이 되었습니다!");
        }

        // 장착 여부 확인
        public bool HasEquippedItems()
        {
            return EquippedItems.Count > 0;
        }

        // 장착 아이템 표시 (없으면 메시지 출력)
        public void ShowEquippedItems()
        {
            if (!HasEquippedItems())
            {
                Console.WriteLine("장착 중인 아이템이 없습니다.");
                return;
            }

            Console.WriteLine("장착 중인 아이템:");
            foreach (var item in EquippedItems)
            {
                Console.WriteLine($"- {item.Name} (공격력 +{item.AttackBonus}, 방어력 +{item.DefenseBonus})");
            }
        }

        // 상태 출력
        public void ShowStatus()
        {
            int bonusAtk = GetTotalAttack() - BaseAttack;
            int bonusDef = GetTotalDefense() - BaseDefense;

            Console.WriteLine($"Lv. {Level}");
            Console.WriteLine($"{Name}");
            Console.WriteLine($"공격력 : {GetTotalAttack()} {(bonusAtk > 0 ? $"(+{bonusAtk})" : "")}");
            Console.WriteLine($"방어력 : {GetTotalDefense()} {(bonusDef > 0 ? $"(+{bonusDef})" : "")}");
            Console.WriteLine($"체 력 : {CurrentHP} / {MaxHP}");
            Console.WriteLine($"MP : {CurrentMP} / {MaxMP}");
            Console.WriteLine($"Gold : {Gold} G");
        }
    }
}