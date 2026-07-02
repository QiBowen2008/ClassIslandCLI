using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Runtime.InteropServices;

namespace ClassIslandCLIAddon
{
    [PluginEntrance]
    public class Plugin : PluginBase
    {
        public override void Initialize(HostBuilderContext context, IServiceCollection services)
        {
            string libraryDirectory0 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string libraryDirectory = Path.GetDirectoryName(libraryDirectory0);
            PathManager.AddToUserPath (libraryDirectory);
            string json = File.ReadAllText(libraryDirectory0 + "/Config.json");
            JsonNode? root = JsonNode.Parse (json);
            root["ClassIslandPath"] = libraryDirectory;
            string newjson = root.ToJsonString();
            File.WriteAllText(libraryDirectory0 + "/Config.json", newjson);
            AppBase.Current.AppStarted += async (_, _) =>
                await CommonTaskDialogs.ShowDialog("提示信息", "插件已经初始化完成");
        }
    }
    public static class PathManager
    {
        public static void AddToUserPath(string directoryToAdd)
        {
            // 1. 获取目标平台的正确PATH分隔符
            char pathSeparator = Path.PathSeparator; // Windows: ';'  Linux/macOS: ':'
            string target = directoryToAdd.TrimEnd(Path.DirectorySeparatorChar);

#if WINDOWS
            // --- Windows 实现 ---
            // 读取当前用户的PATH (从注册表)
            string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? "";
            var paths = new List<string>(currentPath.Split(pathSeparator, StringSplitOptions.RemoveEmptyEntries));

            // 去重并添加
            if (!paths.Contains(target, StringComparer.OrdinalIgnoreCase))
            {
                paths.Insert(0, target); // 插入到开头，提高优先级
                string newPath = string.Join(pathSeparator.ToString(), paths);
                Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.User);
                Console.WriteLine($"已将目录添加到用户PATH (Windows): {target}");
            }
#else
        // --- Linux / macOS 实现 ---
        // 需要写入到用户的Shell配置文件 (如 ~/.bashrc, ~/.zshrc, ~/.profile)
        // 这是一个简化的示例，实际应用需更精细地判断用户使用的Shell
        string profilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bashrc");
        
        // 检查文件是否存在，如果不存在则尝试其他常见配置文件
        if (!File.Exists(profilePath))
        {
            profilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".profile");
        }
        if (!File.Exists(profilePath))
        {
            profilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".zshrc");
        }

        if (File.Exists(profilePath))
        {
            string exportLine = $"export PATH=\"{target}:$PATH\"";
            string content = File.ReadAllText(profilePath);
            
            // 简单的去重检查：避免重复添加同一行
            if (!content.Contains(exportLine))
            {
                File.AppendAllText(profilePath, Environment.NewLine + exportLine);
                Console.WriteLine($"已将目录添加到用户PATH (Unix-like): {target}");
                Console.WriteLine("请执行 'source " + profilePath + "' 或重新登录以使更改生效。");
            }
        }
        else
        {
            Console.WriteLine("未找到用户的Shell配置文件，无法自动添加PATH。");
        }
#endif
        }
    }
}