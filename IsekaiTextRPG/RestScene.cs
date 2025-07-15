using System;

public class RestScene : GameScene
{
    public override string SceneName => "휴식하기";
    private const int RestCost = 500;
    private const int HealAmount = 100;

    public override GameScene? StartScene()
    {
        Console.Clear();
        Player player = GameManager.player;

        List<string> strings = new List<string>()
        {
            $"{RestCost} G를 내면 체력을 회복할 수 있습니다. (보유 골드 : {player.Gold} G)",
            "",
            "1. 휴식하기",
            "0. 나가기"
        };

        UI.DrawTitledBox(SceneName, strings);

        Console.Write(">> ");

        int? input = InputHelper.InputNumber(0, 1);
        
        switch (input)
        {
            case 1:
                if (player.Gold >= RestCost)
                {
                    player.Gold -= RestCost;
                    player.CurrentHP = Math.Min(player.MaxHP, player.CurrentHP + HealAmount);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\n→ 휴식을 완료했습니다.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n→ Gold가 부족합니다.");
                    Console.ResetColor();
                }
                Console.WriteLine("\n아무 키나 누르면 계속...");
                Console.ReadKey();
                return this;

            case 0:
                return prevScene;

            default:
                return this;
        }
    }
}

