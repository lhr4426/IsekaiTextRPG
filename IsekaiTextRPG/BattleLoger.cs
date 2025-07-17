using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;

public static class BattleLogger
{
    // 로그 파일 경로
    private static readonly string logFile = Path.Combine(AppContext.BaseDirectory, "battle_log.txt");

    // 외부 로그 프로세스(cmd) 정보 저장
    private static Process? logProcess;

    // 프로세스 ID를 저장할 변수 추가
    private static int? logProcessId;

    /// <summary>
    /// 로그 초기화 및 외부 로그 모니터 창 실행
    /// </summary>
    public static void Init()
    {
        File.WriteAllText(logFile, "===== 전투 로그 =====\n", Encoding.UTF8); // 초기화
        StartLogViewer();
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
        // PowerShell을 직접 실행하도록 변경
        // -NoExit: PowerShell 창이 종료되지 않도록 함
        // -Command: 실행할 명령
        // Get-Content ... -Wait: 파일 내용 실시간으로 읽기
        string powershellCommand = $"Get-Content '{logFile}' -Wait";

        var psi = new ProcessStartInfo
        {
            FileName = "powershell.exe", // 직접 powershell 실행
            Arguments = $"-NoExit -Command \"{powershellCommand}\"",
            UseShellExecute = true, // 새 창으로 띄우기
            CreateNoWindow = false
        };

        try
        {
            logProcess = Process.Start(psi);
            logProcessId = logProcess?.Id; // 프로세스 ID 저장
        }
        catch (Exception e)
        {
            Console.WriteLine($"[BattleLogger] 로그 뷰어 실행 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 외부 로그 창 종료
    /// </summary>
    public static void CloseLogger()
    {
        if (logProcess != null && !logProcess.HasExited)
        {
            try
            {
                // Save the process ID before attempting to kill
                int? currentProcessId = logProcess.Id; //
                logProcess.Kill(); //
                logProcess.Dispose(); //
                logProcess = null; //

                // 혹시 잔여 powershell 프로세스가 남아있을 경우를 대비하여 ID로 다시 찾아 종료 시도
                if (currentProcessId.HasValue)
                {
                    var processes = Process.GetProcesses(); //
                    foreach (var p in processes) //
                    {
                        try
                        {
                            if (p.Id == currentProcessId.Value && !p.HasExited) //
                            {
                                p.Kill(); //
                                p.Dispose(); //
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            // 프로세스가 이미 종료되었을 때 발생할 수 있는 예외
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[BattleLogger] 잔여 프로세스 종료 실패: {ex.Message}");
                        }
                    }
                }
                Console.WriteLine("[BattleLogger] 로그 뷰어 창이 성공적으로 종료되었습니다.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[BattleLogger] 로그 창 종료 실패: {e.Message}");
            }
        }
        else if (logProcess == null && logProcessId.HasValue)
        {
            // logProcess가 null인데 이전에 ID를 가지고 있었다면, ID로 찾아 종료 시도
            try
            {
                var processes = Process.GetProcesses();
                foreach (var p in processes)
                {
                    try
                    {
                        if (p.Id == logProcessId.Value && !p.HasExited)
                        {
                            p.Kill();
                            p.Dispose();
                        }
                    }
                    catch (InvalidOperationException) { }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[BattleLogger] 잔여 프로세스 (ID로) 종료 실패: {ex.Message}");
                    }
                }
                Console.WriteLine("[BattleLogger] 로그 뷰어 창이 성공적으로 종료되었습니다. (ID 기반)");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[BattleLogger] 로그 창 종료 실패 (ID 기반): {e.Message}");
            }
        }
    }
}