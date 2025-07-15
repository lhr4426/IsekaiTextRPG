using System;
using System.Collections.Generic;

public class StatScene : GameScene
{
    public override string SceneName => "스테이터스 화면";

    public override GameScene? StartScene()
    {
        
        int bonusAtk = GameManager.player.EquippedItems
            .Where(i => i.IsEquip && i.Attack != 0)
            .Sum(i => i.Attack);

        int bonusDef = GameManager.player.EquippedItems
            .Where(i => i.IsEquip && i.Defense != 0)
            .Sum(i => i.Defense);

        int bonusHp = GameManager.player.EquippedItems
            .Where(i => i.IsEquip && i.Type != 0)
            .Sum(i => i.StatValue);

        int bonusMp = GameManager.player.EquippedItems
            .Where(i => i.IsEquip && i.Type != 0)
            .Sum(i => i.StatValue);

        float bonusCR = GameManager.player.EquippedItems
            .Where(i => i.IsEquip && i.CriticalRate != 0)
            .Sum(i => i.CriticalRate);

        float bonusCD = GameManager.player.EquippedItems
            .Where(i => i.IsEquip && i.CriticalDamage != 0)
            .Sum(i => i.CriticalDamage);

        float bonusDR = GameManager.player.EquippedItems
            .Where(i => i.IsEquip && i.DodgeRate != 0)
            .Sum(i => i.DodgeRate);

        string atkStr = bonusAtk > 0 ? $" (+{bonusAtk})" : "";
        string defStr = bonusDef > 0 ? $" (+{bonusDef})" : "";
        string hpStr = bonusHp > 0 ? $" (+{bonusHp})" : "";
        string mpStr = bonusMp > 0 ? $" (+{bonusMp})" : "";
        string crStr = bonusCR > 0 ? $" (+{bonusCR})" : "";
        string cdStr = bonusCD > 0 ? $" (+{bonusCD})" : "";
        string drStr = bonusDR > 0 ? $" (+{bonusDR})" : "";

        Console.Clear();
        Console.WriteLine("상태 보기");
        Console.WriteLine("캐릭터의 정보가 표시됩니다.\n");

        Console.WriteLine($"Lv. {GameManager.player.Level:D2}");
        Console.WriteLine($"{GameManager.player.Name} ( {GameManager.player.Job} )");
        Console.WriteLine($"공격력 : {GameManager.player.BaseAttack + bonusAtk}{atkStr}");
        Console.WriteLine($"방어력 : {GameManager.player.BaseDefense + bonusDef}{defStr}");
        Console.WriteLine($"체 력 : {GameManager.player.CurrentHP + bonusHp}{hpStr}");

        Console.WriteLine($"마 나 : {GameManager.player.CurrentMP + bonusMp}{mpStr}");
        Console.WriteLine($"치명타 확률 : {GameManager.player.CriticalRate + bonusCR}{crStr}");
        Console.WriteLine($"치명타 데미지 : {GameManager.player.CriticalDamage + bonusCD}{cdStr}");
        Console.WriteLine($"회피율 : {GameManager.player.DodgeRate + bonusDR}{drStr}");
        //보유 스킬 추가
        Console.WriteLine($"Gold : {GameManager.player.Gold} G\n");
        Console.WriteLine("0. 나가기\n");
        Console.Write("원하시는 행동을 입력해주세요.\n>> ");

        string? str = Console.ReadLine();
        if(int.TryParse(str, out int index))
        {
            switch (index)
            {
                case 0:
                    return null;                    
                default:
                    break;
            }

        }
        return this;
    }
}