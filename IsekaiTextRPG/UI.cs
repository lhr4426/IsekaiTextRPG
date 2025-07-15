using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class UI
{
    /// <summary>
    /// 타이틀에 씬 이름 넣고, 
    /// contents에다가는 원하는 문자열 리스트 넣으시면 알아서 정렬됨
    /// </summary>
    /// <param name="title"></param>
    /// <param name="contents"></param>
    public static void DrawTitledBox(string title, List<string>? contents)
    {
        List<string> fullStrings = new List<string>();
        fullStrings.Add(title);
        if(contents != null)
        {
            foreach (var str in contents)
            {
                fullStrings.Add(str);
            }
        }
        
        int width = GetMaxWidth(fullStrings);

        string topBorder = "╔" + new string('═', width) + "╗";
        string titleLine = $"║{PadCenter(title, width)}║";
        string divider = "╠" + new string('═', width) + "╣";
        string bottomBorder = "╚" + new string('═', width) + "╝";

        Console.WriteLine(topBorder);
        Console.WriteLine(titleLine);
        if (contents != null && contents.Count > 0)
        {
            Console.WriteLine(divider);
            Console.WriteLine($"║{new string(' ', width)}║");

            foreach (string line in contents)
            {
                Console.WriteLine($"║{PadCenter(line, width)}║");
            }

            Console.WriteLine($"║{new string(' ', width)}║");
            Console.WriteLine(bottomBorder);
        }
        else
        {
            Console.WriteLine(bottomBorder);
        }

    }

    public static void DrawBox(List<string> contents)
    {
        int width = GetMaxWidth(contents);

        string topBorder = "╔" + new string('═', width) + "╗";
        string bottomBorder = "╚" + new string('═', width) + "╝";

        Console.WriteLine(topBorder);
        Console.WriteLine($"║{new string(' ', width)}║");

        foreach (string line in contents)
        {
            Console.WriteLine($"║{PadCenter(line, width)}║");
        }

        Console.WriteLine($"║{new string(' ', width)}║");
        Console.WriteLine(bottomBorder);
    }

    public static void DrawLeftAlignedBox(List<string> contents)
    {
        int width = GetMaxWidth(contents);

        string topBorder = "╔" + new string('═', width) + "╗";
        string bottomBorder = "╚" + new string('═', width) + "╝";

        Console.WriteLine(topBorder);
        Console.WriteLine($"║{new string(' ', width)}║");

        foreach (string line in contents)
        {
            Console.WriteLine($"║{PadLeftAlign(line, width)}║");
        }

        Console.WriteLine($"║{new string(' ', width)}║");
        Console.WriteLine(bottomBorder);
    }

    private static int GetMaxWidth(List<string> contents)
    {
        int maxWidth = 0;
        foreach (string line in contents)
        {
            int displayWidth = GetDisplayWidth(line);
            if (displayWidth > maxWidth)
                maxWidth = displayWidth;
        }
        return maxWidth + 4; // 좌우 여유 여백
    }

    private static string PadCenter(string text, int width)
    {
        int textWidth = GetDisplayWidth(text); 
        int padding = Math.Max(0, width - textWidth); // 남는 부분 (패딩)
        int padLeft = padding / 2;
        int padRight = padding - padLeft;
        return new string(' ', padLeft) + text + new string(' ', padRight);
    }

    private static string PadLeftAlign(string text, int width)
    {
        int displayWidth = GetDisplayWidth(text);
        int padding = Math.Max(0, width - displayWidth);
        return text + new string(' ', padding);
    }


    private static int GetDisplayWidth(string text)
    {
        int width = 0;
        foreach (char c in text)
        {
            width += IsFullWidth(c) ? 2 : 1; // 한글은 두 글자로 취급
        }
        return width;
    }

    /// <summary>
    /// 입력받은 글자가 한글인지 확인함
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private static bool IsFullWidth(char c)
    {
        return c >= 0x1100 && (
            c <= 0x115F || // 한글 자모
            c == 0x2329 || c == 0x232A ||
            (c >= 0x2E80 && c <= 0xA4CF) ||
            (c >= 0xAC00 && c <= 0xD7A3) || // 한글 완성형
            (c >= 0xF900 && c <= 0xFAFF) ||
            (c >= 0xFE10 && c <= 0xFE19) ||
            (c >= 0xFF01 && c <= 0xFF60) ||
            (c >= 0xFFE0 && c <= 0xFFE6)
        );
    }
}



