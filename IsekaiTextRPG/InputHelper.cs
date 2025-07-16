using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class InputHelper
{
    /// <summary>
    /// 정수 최소값과 최대값을 매개변수로 넣어주면
    /// 입력을 받아서 알아서 처리해 줍니다.  
    /// 다만, 조건에 맞지 않는 입력일 경우 null 을 반환합니다.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int? InputNumber(int min, int max)
    {
        while (true)
        {
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("⚠ 입력값이 비어 있습니다. 다시 입력해주세요.");
                Console.Write(">> ");
                continue;
            }

            if (int.TryParse(input, out int number))
            {
                if (number >= min && number <= max)
                {
                    return number;
                }
                else
                {
                    Console.WriteLine($"⚠ {min} ~ {max} 사이의 숫자를 입력해주세요.");
                }
            }
            else
            {
                Console.WriteLine("⚠ 유효한 숫자를 입력해주세요.");
            }

            Console.Write(">> ");
        }
    }
}
