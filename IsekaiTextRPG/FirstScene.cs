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

        List<string> contents = new();
        contents.Add("1. 이어서 하기");
        contents.Add("2. 새로 만들기");
        contents.Add("0. 게임 종료");
        UI.DrawTitledBox("이세계 RPG", contents);

        Console.Write(">> ");


        int? index = InputHelper.InputNumber(0, 2);

         switch (index)
         {
             case 0:
                 Console.WriteLine("게임을 종료합니다.");
                 Console.ReadKey();
                 return null;                    
             case 1:
                GameManager.instance.LoadPlayerData();
                 return EndScene();
             case 2:
                GameManager.instance.NewPlayerData();
                Console.ReadKey();
                return EndScene();
            default:
                 break;
         }

        
        return this;
    }
}

