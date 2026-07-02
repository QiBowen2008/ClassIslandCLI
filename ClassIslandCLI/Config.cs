using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClassIslandCLI
{
        public class Config
        {
                /// <summary>
                /// 课表配置目录
                /// </summary>
                public string ProfilePath { get; set; }

               /// <summary>
               /// 设置配置目录
               /// </summary>
               public string ClassIslandPath { get; set; }

        }

        /// <summary>
        /// AOT 兼容的 JSON 源生成器上下文。
        /// </summary>
        [JsonSerializable(typeof(Config))]
        internal partial class ConfigJsonContext : JsonSerializerContext
        {
        }

    public static class GetConfig
    {
        /// <summary>
        /// Config.json 的完整路径（始终位于程序所在目录）。
        /// </summary>
        public static string ConfigFilePath => Path.Combine(AppContext.BaseDirectory, "Config.json");

        /// <summary>
        /// 读取 Config.json。如果文件不存在则自动创建一个空配置。
        /// </summary>
        public static Config GetConfigs()
        {
            EnsureConfigExists();
            string jsonString = File.ReadAllText(ConfigFilePath);
            Config config = JsonSerializer.Deserialize(jsonString, ConfigJsonContext.Default.Config);
            return config;
        }

        /// <summary>
        /// 若 Config.json 不存在，则自动创建一个包含空路径的默认配置文件。
        /// </summary>
        private static void EnsureConfigExists()
        {
            if (File.Exists(ConfigFilePath)) return;
            var defaultConfig = new Config
            {
                ProfilePath = "",
                ClassIslandPath = ""
            };
            string json = JsonSerializer.Serialize(defaultConfig, ConfigJsonContext.Default.Config);
            File.WriteAllText(ConfigFilePath, json);
            Console.WriteLine($"[配置] 未找到 Config.json，已在以下位置创建默认配置：{ConfigFilePath}");
            Console.WriteLine("       请使用 --SetProfilePath 和 --SetClassIslandPath 设置路径。");
        }
    }
    public static class SetConfig
    {
        public static void SetProfilePath(string ProfilePath)
        {
            if (File.Exists(ProfilePath))
            {
                string jsonString = File.ReadAllText(GetConfig.ConfigFilePath);
                Config config = JsonSerializer.Deserialize(jsonString, ConfigJsonContext.Default.Config);
                config.ProfilePath = ProfilePath;
                jsonString = JsonSerializer.Serialize(config, ConfigJsonContext.Default.Config);
                File.WriteAllText(GetConfig.ConfigFilePath, jsonString);
            }
            else
            {
                Console.WriteLine("你所指定的文件不存在");
            }
        }

           public static void SetClassIslandPath(string ClassIslandPath)
            {
                   if (Directory.Exists(ClassIslandPath))
                    {
                        string jsonString = File.ReadAllText(GetConfig.ConfigFilePath);
                        Config config = JsonSerializer.Deserialize(jsonString, ConfigJsonContext.Default.Config);
                       config.ClassIslandPath = ClassIslandPath;
                        jsonString = JsonSerializer.Serialize(config, ConfigJsonContext.Default.Config);
                        File.WriteAllText(GetConfig.ConfigFilePath, jsonString);
                    }
                    else
                    {
                        Console.WriteLine("你所指定的目录不存在");
                    }
            }
            
    }
}
