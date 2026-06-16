using System.Text.Json;

namespace ClassIslandCLI
{
        public class Config
        {
                /// <summary>
                /// 
                /// </summary>
                public string ProfilePath { get; set; }

                /// <summary>
                /// 
                /// </summary>
                public string SettingsfilePath { get; set; }

        }

        public static class GetConfig
        {
                public static Config GetConfigs()
                {
                        string jsonString = File.ReadAllText(AppContext.BaseDirectory + "Config.json");
                        Config config = JsonSerializer.Deserialize<Config>(jsonString);
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
                                Config config = JsonSerializer.Deserialize<Config>(jsonString);
                                config.ProfilePath = ProfilePath;
                                jsonString = JsonSerializer.Serialize(config);
                                File.WriteAllText("Config.json", jsonString);
                        }
                        else
                        {
                                Console.WriteLine("你所指定的文件不存在");
                        }
                }

                public static void SetSettingsfilePath(string SettingsfilePath)
                {
                        if (File.Exists(SettingsfilePath))
                        {

                                string jsonString = File.ReadAllText(AppContext.BaseDirectory + "Config.json");
                                Config config = JsonSerializer.Deserialize<Config>(jsonString);
                                config.SettingsfilePath = SettingsfilePath;
                                jsonString = JsonSerializer.Serialize(config);
                                File.WriteAllText("Config.json", jsonString);
                        }
                        else
                        {
                                Console.WriteLine("你所指定的文件不存在");
                        }
                }
                
        }
}