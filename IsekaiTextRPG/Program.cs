using System;
using System.Text; // Encoding을 사용하기 위해 필요

public class Program
{
    static void Main(string[] args)
    {
        // --- 1. 콘솔 인코딩 설정 (가장 먼저) ---
        // 한글이 깨지지 않도록 UTF-8로 설정합니다.
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        // --- 2. 콘솔 창 크기 설정 (그 다음) ---
        int desiredWidth = 120; // 원하는 가로 너비 (글자 수 기준)
        int desiredHeight = 40; // 원하는 세로 높이 (줄 수 기준)

        int maxWindowWidth = Console.LargestWindowWidth;
        int maxWindowHeight = Console.LargestWindowHeight;

        // 원하는 너비/높이가 최대값을 초과하지 않도록 조정
        if (desiredWidth > maxWindowWidth)
        {
            desiredWidth = maxWindowWidth;
        }
        if (desiredHeight > maxWindowHeight)
        {
            desiredHeight = maxWindowHeight;
        }

        try
        {
            // 버퍼 크기를 먼저 설정 (창 크기보다 크거나 같아야 함)
            Console.BufferWidth = desiredWidth;
            Console.BufferHeight = desiredHeight;

            // 창 크기 설정
            Console.SetWindowSize(desiredWidth, desiredHeight);

            // 콘솔 창 제목 설정 (선택 사항)
            Console.Title = "나만의 던전 RPG";
        }
        catch (System.IO.IOException e)
        {
            Console.WriteLine($"[경고] 콘솔 창 크기를 조정할 수 없습니다 (IDE 출력 창 등): {e.Message}");
            Console.WriteLine("계속하려면 아무 키나 누르세요...");
            Console.ReadKey();
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.WriteLine($"[경고] 콘솔 창 크기 설정 값이 유효하지 않습니다: {e.Message}");
            Console.WriteLine("기본 크기로 실행합니다. 계속하려면 아무 키나 누르세요...");
            Console.ReadKey();
        }

        // --- 3. 게임 매니저 및 게임 시작 로직 (가장 나중에) ---
        GameManager gameManager = new GameManager();
        gameManager.GameStart();
    }
}