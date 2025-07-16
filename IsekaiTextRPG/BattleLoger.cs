using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;

public static class BattleLogger
{
    private static readonly string logFile = Path.Combine(AppContext.BaseDirectory, "battle_log.txt");

    public static void Init()
    {
        File.WriteAllText(logFile, "===== 전투 로그 =====\n", Encoding.UTF8);
        OpenConsole();
    }

    public static void Log(string message)
    {
        File.AppendAllText(logFile, $"{DateTime.Now:HH:mm:ss} | {message}\n", Encoding.UTF8);
    }

    private static void OpenConsole()
    {
        string cmd = $"/K chcp 65001 && type \"{logFile}\" && powershell -Command \"Get-Content -Path '{logFile}' -Wait\"";
        
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/K chcp 65001 & powershell -Command \"Get-Content 'battle_log.txt' -Wait\"",
            UseShellExecute = true
        };
        Process.Start(psi);
    }
}
