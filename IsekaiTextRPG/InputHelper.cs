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
        string? str = Console.ReadLine();
        if (int.TryParse(str, out int index))
        {
            if (index >= min && index <= max) return index;
            else return null;
        }
        else return null;
    }
}
