using System.Runtime.InteropServices;

namespace ClassIslandCLI;

/// <summary>
/// Config.json 路径核验器——跨平台，启动时自动检测目录或文件是否存在。
/// </summary>
public static class ConfigValidator
{
    /// <summary>
    /// 读取 Config.json 并逐一核验其中的文件/目录路径是否存在，
    /// 若不存在则以黄色警告输出；无论如何不阻止程序继续运行。
    /// </summary>
    public static void Validate()
    {
        Config? config;
        try
        {
            config = GetConfig.GetConfigs();
        }
        catch (Exception ex)
        {
            Warn($"无法读取 Config.json：{ex.Message}");
            return;
        }

        bool anyIssue = false;

        // 1) ProfilePath —— 应当指向一个 JSON 文件
        if (!string.IsNullOrWhiteSpace(config.ProfilePath))
        {
            var normalized = NormalizePath(config.ProfilePath);
            if (!File.Exists(normalized))
            {
                Warn($"ProfilePath 文件不存在：{normalized}");
                anyIssue = true;
            }
        }
        else
        {
            Warn("Config.json 中 ProfilePath 为空，请使用 --SetProfilePath 设置");
            anyIssue = true;
        }

        // 2) ClassIslandPath —— 应当指向一个目录
        if (!string.IsNullOrWhiteSpace(config.ClassIslandPath))
        {
            var normalized = NormalizePath(config.ClassIslandPath);
            if (!Directory.Exists(normalized))
            {
                Warn($"ClassIslandPath 目录不存在：{normalized}");
                anyIssue = true;
            }
        }
        else
        {
            Warn("Config.json 中 ClassIslandPath 为空，请使用 --SetClassIslandPath 设置");
            anyIssue = true;
        }

        if (!anyIssue)
            Console.Out.WriteLine("[配置核验] Config.json 路径检查通过");
    }

    /// <summary>
    /// 统一路径格式：交换反斜杠为斜杠（跨平台友好），处理 ~/ 家目录展开（Unix）等。
    /// </summary>
    private static string NormalizePath(string path)
    {
        // 统一为正斜杠，避免 Windows 风格转义问题
        path = path.Replace('\\', '/');

        // Unix 上的 ~/ 展开为用户主目录
        if (path.StartsWith("~/") && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            if (home != null)
                path = home + path[1..]; // ~/ → /home/xxx/
        }

        // Windows 上确保正斜杠能被 .NET 正确识别（File.Exists 接受正斜杠）
        return path;
    }

    private static void Warn(string message)
    {
        var original = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Error.WriteLine($"[配置核验] 警告：{message}");
        Console.ForegroundColor = original;
    }
}
