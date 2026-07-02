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
                public static Config GetConfigs()
                {
                        string jsonString = File.ReadAllText(AppContext.BaseDirectory + "Config.json");
                        Config config = JsonSerializer.Deserialize(jsonString, ConfigJsonContext.Default.Config);
                        return config;
                }
        }
        public static class SetConfig
        {
                public static void SetProfilePath(string ProfilePath)
                {
                        if (File.Exists(ProfilePath)) 
                        {
                                string jsonString = File.ReadAllText("Config.json");
                                Config config = JsonSerializer.Deserialize(jsonString, ConfigJsonContext.Default.Config);
                                config.ProfilePath = ProfilePath;
                                jsonString = JsonSerializer.Serialize(config, ConfigJsonContext.Default.Config);
                                File.WriteAllText("Config.json", jsonString);
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

                                string jsonString = File.ReadAllText(AppContext.BaseDirectory + "Config.json");
                                Config config = JsonSerializer.Deserialize(jsonString, ConfigJsonContext.Default.Config);
                               config.ClassIslandPath = ClassIslandPath;
                                jsonString = JsonSerializer.Serialize(config, ConfigJsonContext.Default.Config);
                                File.WriteAllText("Config.json", jsonString);
                        }
                        else
                        {
                                Console.WriteLine("你所指定的目录不存在");
                        }
                }
                
        }
}
