using System;

public class RestScene : GameScene
{
    public override string SceneName => "휴식하기";
    private const int RestCost = 500;
    private const int HealAmount = 100;

    public override GameScene? StartScene()
    {
        Console.Clear();
        Player player = GameManager.Instance.Player;

        Console.WriteLine("휴식하기");
        Console.WriteLine($"{RestCost} G를 내면 체력을 회복할 수 있습니다. (보유 골드 : {player.Gold} G)");
        Console.WriteLine();
        Console.WriteLine("1. 휴식하기");
        Console.WriteLine("0. 나가기");
        Console.WriteLine();
        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
        string? input = Console.ReadLine();

        switch (input)
        {
            case "1":
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

            case "0":
                return SceneManager.Instance.scenes[SceneManager.SceneType.TownScene];

            default:
                return this;
        }
    }
}

