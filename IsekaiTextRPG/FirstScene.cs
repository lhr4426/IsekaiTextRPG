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
        contents.Add("1. 게임 시작");
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
                 ChoosePlayer();
                Console.Clear();
                 return EndScene();
            default:
                 break;
         }

        
        return this;
    }

    public void ChoosePlayer()
    {
        while (true)
        {
            Console.Clear();
            GameManager.instance.ShowSaveSlots();
            Console.Write("플레이 할 슬롯 선택 (0: 게임 종료) : ");
            int? slotSelect = InputHelper.InputNumber(0, 4);

            if (slotSelect == 0) GameManager.instance.GameExit();
            if (slotSelect == null) continue;
            else 
            {
                List<string> strings = new();
                strings.Add($" {slotSelect} 번째 슬롯 선택됨");
                strings.Add(" 1. 선택한 데이터로 플레이");
                strings.Add(" 2. 선택한 데이터 삭제");
                strings.Add(" 0. 뒤로가기");
                UI.DrawLeftAlignedBox(strings);
                Console.Write(">> ");
                int? input = InputHelper.InputNumber(0, 2);
                if (input == null || input == 0) continue;
                if (input == 1)
                {
                    GameManager.instance.LoadPlayerData((int)slotSelect);
                    break;
                }
                else if (input == 2)
                {
                    GameManager.instance.DeleteSlot((int)slotSelect);
                }
            }
        }
        
    }
}

