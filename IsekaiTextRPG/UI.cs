using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class UI
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="contents"></param>
    public static void DrawTitledBox(string title, List<string> contents)
    {
        int width = GetMaxWidth(contents);

        string topBorder = "╔" + new string('═', width) + "╗";
        string titleLine = $"║{PadCenter(title, width)}║";
        string divider = "╠" + new string('═', width) + "╣";
        string bottomBorder = "╚" + new string('═', width) + "╝";

        Console.WriteLine(topBorder);
        Console.WriteLine(titleLine);
        Console.WriteLine(divider);
        Console.WriteLine($"║{new string(' ', width)}║");

        foreach (string line in contents)
        {
            Console.WriteLine($"║{PadCenter(line, width)}║");
        }

        Console.WriteLine($"║{new string(' ', width)}║");
        Console.WriteLine(bottomBorder);
    }

    private static int GetMaxWidth(List<string> contents)
    {
        int maxWidth = 0;
        foreach (string line in contents)
        {
            if(line.Length > maxWidth) maxWidth = line.Length;
        }
        return maxWidth + 10;
    }

    private static string PadCenter(string text, int width)
    {
        int textWidth = GetDisplayWidth(text); // 한글은 2칸으로 계산
        int padding = Math.Max(0, width - textWidth);
        int padLeft = padding / 2;
        int padRight = padding - padLeft;
        return new string(' ', padLeft) + text + new string(' ', padRight);
    }

    private static int GetDisplayWidth(string text)
    {
        int width = 0;
        foreach (char c in text)
        {
            width += IsFullWidth(c) ? 2 : 1;
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



