using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FirstScene : GameScene
{
    public override string SceneName => "시작 화면";

    public override GameScene? StartScene()
    {
        // TODO : 입력받는 부분 수정 필요
        Console.Clear();
        Console.WriteLine("이세계 RPG");
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("1. 이어서 하기");
        Console.WriteLine("2. 새로 만들기");
        Console.WriteLine("0. 게임 종료");
        Console.WriteLine();
        Console.WriteLine(">> ");
        string? str = Console.ReadLine();
        if(int.TryParse(str, out int index))
        {
            switch (index)
            {
                case 0:
                    Console.WriteLine("게임을 종료합니다.");
                    Console.ReadKey();
                    return null;                    
                case 1:
                    Console.WriteLine("미구현");
                    Console.ReadKey();
                    break;
                case 2:
                    Console.WriteLine("미구현");
                    Console.ReadKey();
                    break;
                default:
                    break;
            }

        }
        return this;
    }
}

