using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq; // Process.GetProcessesByName을 위해 추가

public static class BattleLogger
{
    private static readonly string logFile = Path.Combine(AppContext.BaseDirectory, "battle_log.txt");
    private static Process? logProcess;
    private static int? logProcessId; // 프로세스 ID를 저장할 변수 추가

    /// <summary>
    /// 로그 초기화 및 외부 로그 모니터 창 실행
    /// </summary>
    public static void Init()
    {
        // 파일이 존재하면 초기화 (기존 내용 삭제)
        File.WriteAllText(logFile, "===== 전투 로그 =====\n", Encoding.UTF8);
        StartLogViewer(); // 로그 모니터 시작
    }

    /// <summary>
    /// 로그 파일에 메시지 기록
    /// </summary>
    public static void Log(string message)
    {
        // 파일에 메시지 추가
        File.AppendAllText(logFile, $"{DateTime.Now:HH:mm:ss} | {message}\n", Encoding.UTF8);
    }

    /// <summary>
    /// 외부 로그 뷰어(powershell) 실행
    /// </summary>
    private static void StartLogViewer()
    {
        // PowerShell을 직접 실행하도록 변경
        // -NoExit: PowerShell 창이 종료되지 않도록 함 (명령 실행 후에도 창 유지)
        // -Command: 실행할 명령
        // Get-Content '{logFile}' -Wait: 지정된 파일을 실시간으로 읽음
        string powershellCommand = $"Get-Content '{logFile}' -Wait";

        var psi = new ProcessStartInfo
        {
            FileName = "powershell.exe", // 직접 powershell 실행
            Arguments = $"-NoExit -Command \"{powershellCommand}\"",
            UseShellExecute = true, // 새 창으로 띄우기 위해 true로 설정
            CreateNoWindow = false // 새 창 생성
        };

        try
        {
            logProcess = Process.Start(psi);
            logProcessId = logProcess?.Id; // 시작된 프로세스의 ID를 저장 (종료 시 활용)
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
        // logProcess 객체가 존재하고 아직 종료되지 않았다면
        if (logProcess != null && !logProcess.HasExited)
        {
            try
            {
                int? currentProcessId = logProcess.Id; // 현재 프로세스 ID를 임시 저장
                logProcess.Kill(); // 프로세스 강제 종료
                logProcess.Dispose(); // 리소스 해제
                logProcess = null; // 참조를 null로 설정하여 다시 Kill 시도 방지

                // 혹시 강제 종료 후에도 잔여 프로세스가 남아있을 경우를 대비하여
                // 저장된 ID로 프로세스를 다시 찾아 종료 시도
                if (currentProcessId.HasValue)
                {
                    // GetProcesses()는 모든 실행 중인 프로세스를 가져옵니다.
                    // 해당 ID를 가진 프로세스가 있다면 종료합니다.
                    var processes = Process.GetProcesses();
                    foreach (var p in processes)
                    {
                        try
                        {
                            if (p.Id == currentProcessId.Value && !p.HasExited)
                            {
                                p.Kill();
                                p.Dispose();
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            // 프로세스가 이미 종료되었을 때 발생할 수 있는 예외 (무시)
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
        // logProcess가 null이지만 이전에 ID를 가지고 있었다면 (예: Init 후 바로 Kill 시도 등)
        else if (logProcess == null && logProcessId.HasValue)
        {
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
        // 만약 로그 프로세스가 아예 시작되지 않았거나, 이미 종료된 상태라면 아무 작업도 하지 않습니다.
    }
}