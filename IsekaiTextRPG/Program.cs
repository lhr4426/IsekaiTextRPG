using System;
using System.Text;

public class Program
{
    static void Main(string[] args)
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        int desiredWidth = 120; // 스크린샷에서 100일 때 오류 났으니 120이나 150으로 다시 시도
        int desiredHeight = 40;

        try
        {
            // 시스템이 허용하는 최대 콘솔 너비와 높이를 가져옵니다.
            int maxWindowWidth = Console.LargestWindowWidth;
            int maxWindowHeight = Console.LargestWindowHeight;

            // 설정하려는 너비와 높이가 시스템 최대치를 넘지 않도록 제한합니다.
            int actualSetWidth = Math.Min(desiredWidth, maxWindowWidth);
            int actualSetHeight = Math.Min(desiredHeight, maxWindowHeight);

            // 먼저 콘솔 버퍼 크기를 시스템이 허용하는 최대로 늘려놓습니다.
            Console.BufferWidth = maxWindowWidth;
            Console.BufferHeight = maxWindowHeight;

            // 원하는 '창 크기'를 설정합니다.
            Console.SetWindowSize(actualSetWidth, actualSetHeight);

            // 최종적으로 콘솔 창 제목을 설정합니다.
            Console.Title = "이세계 RPG - 던전 탐험 시작!";
        }
        catch (System.IO.IOException)
        {
            
        }
        catch (ArgumentOutOfRangeException)
        {
            
        }

        // 게임 매니저 초기화 및 게임 시작 로직
        GameManager gameManager = new GameManager();
        gameManager.GameStart();
    }
}