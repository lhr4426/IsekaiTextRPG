using System;
using System.Diagnostics;
using System.IO;
using System.Text;

public static class BattleLogger
{
    // 로그 파일 경로
    private static readonly string logFile = Path.Combine(AppContext.BaseDirectory, "battle_log.txt");

    // 외부 로그 프로세스(cmd) 정보 저장
    private static Process? logProcess;

    /// <summary>
    /// 로그 초기화 및 외부 로그 모니터 창 실행
    /// </summary>
    public static void Init()
    {
        File.WriteAllText(logFile, "===== 전투 로그 =====\n", Encoding.UTF8); // 초기화
        StartLogViewer(); // 로그 모니터 시작
    }

    /// <summary>
    /// 로그 파일에 메시지 기록
    /// </summary>
    public static void Log(string message)
    {
        File.AppendAllText(logFile, $"{DateTime.Now:HH:mm:ss} | {message}\n", Encoding.UTF8);
    }

    /// <summary>
    /// 외부 로그 뷰어(cmd + powershell) 실행
    /// </summary>
    private static void StartLogViewer()
    {
        string powershellCmd = "powershell -Command \"Get-Content 'battle_log.txt' -Wait\"";

        var psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/K chcp 65001 & {powershellCmd}", // UTF-8 출력
            UseShellExecute = true,
            CreateNoWindow = false
        };

        try
        {
            logProcess = Process.Start(psi);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[BattleLogger] 로그 뷰어 실행 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 외부 로그 창 종료 (사용자가 Q 키를 눌렀을 때 호출)
    /// </summary>
    public static void CloseLogger()
    {
        if (logProcess != null && !logProcess.HasExited)
        {
            try
            {
                logProcess.Kill(); // 프로세스 강제 종료
                logProcess.Dispose();
                logProcess = null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[BattleLogger] 로그 창 종료 실패: {e.Message}");
            }
        }
    }
}