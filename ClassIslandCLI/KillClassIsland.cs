using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ClassIslandCLI;

public static class Restarter
{
    public static void Restart()
    {
        string exe = GetConfig.GetConfigs().ClassIslandPath+"/"+(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ClassIsland.Desktop.exe" : "ClassIsland.Desktop");

        // 杀掉所有 test 进程
        foreach (var p in Process.GetProcessesByName("ClassIsland.Desktop"))
        {
            Console.WriteLine(p.ProcessName);
            try { p.Kill(); p.WaitForExit(2000); } catch { Console.WriteLine("未能结束ClassIsland，请手动重启ClassIsland来应用更改"); }
            p.Dispose();
        }

        Thread.Sleep(300); // 等待资源释放

        // 启动 
        Process.Start(exe);
    }
}