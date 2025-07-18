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
        if (contents != null)
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
    
    public static List<string> DrawBox(List<string> contents, bool retuner)
    {
        if (retuner)
        {
            int width = GetMaxWidth(contents);

            List<string> strings = new List<string>();

            string topBorder = "╔" + new string('═', width) + "╗";
            string bottomBorder = "╚" + new string('═', width) + "╝";

            strings.Add(topBorder);
            strings.Add($"║{new string(' ', width)}║");

            foreach (string line in contents)
            {
                strings.Add($"║{PadCenter(line, width)}║");
            }

            strings.Add($"║{new string(' ', width)}║");
            strings.Add(bottomBorder);
            return strings;
        }
        else return new List<string>();
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
    public static void DrawLeftAlignedBox(List<string> contents, int fixedWidth)
    {
        int width = Math.Max(fixedWidth, GetMaxWidth(contents));

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


    public static int GetDisplayWidth(string text)
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
            (c >= 0xFFE0 && c <= 0xFFE6) ||
            (c >= 0x2600 && c <= 0x26FF) // 특수기호
        );
    }

    public static void TypeWriter(List<string> texts, int delay = 5)
    {
        bool skip = false;

        foreach (string text in texts)
        {
            foreach (char c in text)
            {
                // 키가 눌렸는지 확인
                if (Console.KeyAvailable)
                {
                    skip = true;
                    Console.ReadKey(true); // 키를 읽어서 버퍼에서 제거
                }

                Console.Write(c);

                // skip이 false일 때만 딜레이
                if (!skip)
                    Thread.Sleep(delay);
            }
            Console.WriteLine();
        }
    }
    public static void DrawTitledBoxAt(string title, List<string> contents, int left, int top)
    {
        List<string> fullStrings = new List<string> { title };
        fullStrings.AddRange(contents);

        int width = GetMaxWidth(fullStrings);
        string topBorder = "╔" + new string('═', width) + "╗";
        string titleLine = $"║{PadCenter(title, width)}║";
        string divider = "╠" + new string('═', width) + "╣";
        string emptyLine = $"║{new string(' ', width)}║";
        string bottomBorder = "╚" + new string('═', width) + "╝";

        Console.SetCursorPosition(left, top++);
        Console.Write(topBorder);

        Console.SetCursorPosition(left, top++);
        Console.Write(titleLine);

        if (contents.Count > 0)
        {
            Console.SetCursorPosition(left, top++);
            Console.Write(divider);

            Console.SetCursorPosition(left, top++);
            Console.Write(emptyLine);

            foreach (var line in contents)
            {
                Console.SetCursorPosition(left, top++);
                Console.Write($"║{PadCenter(line, width)}║");
            }

            Console.SetCursorPosition(left, top++);
            Console.Write(emptyLine);
        }

        Console.SetCursorPosition(left, top);
        Console.Write(bottomBorder);
    }



    public static void DrawBoxAt(List<string> contents, int left, int top)
    {
        int width = GetMaxWidth(contents);
        string topBorder = "╔" + new string('═', width) + "╗";
        string emptyLine = $"║{new string(' ', width)}║";
        string bottomBorder = "╚" + new string('═', width) + "╝";

        Console.SetCursorPosition(left, top++);
        Console.Write(topBorder);

        Console.SetCursorPosition(left, top++);
        Console.Write(emptyLine);

        foreach (string line in contents)
        {
            string centered = $"║{PadCenter(line, width)}║";
            Console.SetCursorPosition(left, top++);
            Console.Write(centered);
        }

        Console.SetCursorPosition(left, top++);
        Console.Write(emptyLine);

        Console.SetCursorPosition(left, top);
        Console.Write(bottomBorder);
    }
}



