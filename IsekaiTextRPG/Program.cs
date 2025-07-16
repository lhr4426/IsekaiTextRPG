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
            Console.Title = "이세계 RPG - 크기 조절 시도 중...";

            // SetWindowSize도 MaxValue를 넘으면 안되므로 안전하게 설정합니다.
            int actualDesiredWidth = Math.Min(desiredWidth, Console.LargestWindowWidth);
            int actualDesiredHeight = Math.Min(desiredHeight, Console.LargestWindowHeight);

            // 중요: 창 크기를 먼저 설정합니다.
            Console.SetWindowSize(actualDesiredWidth, actualDesiredHeight);

            Console.BufferWidth = actualDesiredWidth;
            Console.BufferHeight = actualDesiredHeight;

            Console.Title = "이세계 RPG - 창 크기 설정 완료!";
        }
        catch (System.IO.IOException e)
        {
            Console.WriteLine($"[경고] 콘솔 창 크기를 조정할 수 없습니다 (권한 또는 환경 문제): {e.Message}");
            Console.WriteLine("계속하려면 아무 키나 누르세요...");
            Console.ReadKey();
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.WriteLine($"[경고] 콘솔 창 크기 설정 값이 유효하지 않습니다: {e.Message}");
            Console.WriteLine($"현재 시스템에서 허용하는 최대 크기: Width={Console.LargestWindowWidth}, Height={Console.LargestWindowHeight}");
            Console.WriteLine("설정값을 조정하거나 폰트 크기를 변경해 보세요.");
            Console.WriteLine("기본 크기로 실행합니다. 계속하려면 아무 키나 누르세요...");
            Console.ReadKey();
        }

        // 게임 매니저 초기화 및 게임 시작 로직
        GameManager gameManager = new GameManager();
        gameManager.GameStart();
    }
}