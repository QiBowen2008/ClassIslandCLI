using System.Text.Json;
using System.Text.Json.Nodes;

namespace ClassIslandCLI;

/// <summary>
/// --AddSubject 可选参数：对应 Subjects[].AttachedObjects 中的通知设置
/// </summary>
public record SubjectAttachedSettings(
    bool? IsClassOnNotificationEnabled = null,
    bool? IsClassOnPreparingNotificationEnabled = null,
    bool? IsClassOffNotificationEnabled = null,
    int? ClassPreparingDeltaTime = null,
    string? ClassOnPreparingText = null,
    string? OutdoorClassOnPreparingText = null,
    string? ClassOnPreparingMaskText = null,
    string? OutdoorClassOnPreparingMaskText = null,
    string? ClassOnMaskText = null,
    string? ClassOffMaskText = null,
    string? ClassOffOverlayText = null
);

/// <summary>
/// 科目信息参数，后续可扩展更多字段。
/// </summary>
public record SubjectInfo(
    string Name,
    bool IsOutDoor,
    string Initial,
    string TeacherName = "",
    SubjectAttachedSettings? AttachedSettings = null
);

public static class ProfileManager
{
    static string profilePath = AppContext.BaseDirectory + @"\Default.json";
    static string profileString = File.ReadAllText(profilePath);
    static JsonNode root = JsonNode.Parse(profileString)!;

    static void SaveProfile()
    {
        var options = new JsonSerializerOptions { WriteIndented = false };
        File.WriteAllText(profilePath, root.ToJsonString(options));
    }

    public static class TimeLayoutManager
    {
        public static void GetTimelayouts()
        {
            var timelayouts = root["TimeLayouts"];
            Console.WriteLine(timelayouts.ToString());
        }
    }

    public static class ClassPlanManager
    {
        public static void GetClassplans()
        {
            var subjects = root["Subjects"] as JsonObject;
            var classPlans = root["ClassPlans"] as JsonObject;

            if (subjects == null || classPlans == null) return;

            using var stream = Console.OpenStandardOutput();
            using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false });

            writer.WriteStartObject();

            foreach (var plan in classPlans)
            {
                writer.WriteStartObject(plan.Key);

                var planValue = plan.Value as JsonObject;
                if (planValue == null) continue;

                // 课表名称
                var planName = planValue["Name"];
                if (planName != null)
                    writer.WriteString("Name", planName.GetValue<string>());

                // TimeLayoutId
                var tlId = planValue["TimeLayoutId"];
                if (tlId != null)
                    writer.WriteString("TimeLayoutId", tlId.GetValue<string>());

                // WeekDay
                var timeRule = planValue["TimeRule"] as JsonObject;
                if (timeRule != null)
                {
                    var weekDay = timeRule["WeekDay"];
                    if (weekDay != null)
                        writer.WriteNumber("WeekDay", weekDay.GetValue<int>());
                }

                // IsEnabled
                var enabled = planValue["IsEnabled"];
                if (enabled != null)
                    writer.WriteBoolean("IsEnabled", enabled.GetValue<bool>());

                // Classes 替换 SubjectId
                var classes = planValue["Classes"] as JsonArray;
                if (classes != null)
                {
                    writer.WritePropertyName("Classes");
                    writer.WriteStartArray();

                    foreach (var cls in classes)
                    {
                        var clsObj = cls as JsonObject;
                        if (clsObj == null) continue;

                        writer.WriteStartObject();

                        var sid = clsObj["SubjectId"];
                        if (sid != null)
                        {
                            var sidStr = sid.GetValue<string>();
                            if (sidStr != null && subjects.TryGetPropertyValue(sidStr, out var subjectNode))
                            {
                                var subject = subjectNode as JsonObject;
                                if (subject != null)
                                {
                                    var subjName = subject["Name"];
                                    if (subjName != null)
                                        writer.WriteString("Subject", subjName.GetValue<string>());
                                    var subjInit = subject["Initial"];
                                    if (subjInit != null)
                                        writer.WriteString("Initial", subjInit.GetValue<string>());
                                }
                            }
                            else
                            {
                                writer.WriteString("SubjectId", sidStr ?? "");
                            }
                        }

                        var clsEnabled = clsObj["IsEnabled"];
                        if (clsEnabled != null)
                            writer.WriteBoolean("IsEnabled", clsEnabled.GetValue<bool>());

                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();
                }

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
            writer.Flush();
            Console.WriteLine();
        }
    }

    public static class SubjectManager
    {
        /// <summary>
        /// 添加一个新科目到配置中。
        /// </summary>
        public static void AddSubject(SubjectInfo info)
        {
            var subjects = root["Subjects"] as JsonObject;
            if (subjects == null)
            {
                Console.WriteLine("错误：无法找到 Subjects 节点");
                return;
            }

            string id = Guid.NewGuid().ToString();

            var attachedObj = new JsonObject();

            if (info.AttachedSettings != null)
            {
                var s = info.AttachedSettings;
                var innerId = Guid.NewGuid().ToString();
                var inner = new JsonObject
                {
                    ["IsAttachSettingsEnabled"] = true,
                    ["IsActive"] = false
                };

                if (s.IsClassOnNotificationEnabled.HasValue)
                    inner["IsClassOnNotificationEnabled"] = s.IsClassOnNotificationEnabled.Value;
                if (s.IsClassOnPreparingNotificationEnabled.HasValue)
                    inner["IsClassOnPreparingNotificationEnabled"] = s.IsClassOnPreparingNotificationEnabled.Value;
                if (s.IsClassOffNotificationEnabled.HasValue)
                    inner["IsClassOffNotificationEnabled"] = s.IsClassOffNotificationEnabled.Value;
                if (s.ClassPreparingDeltaTime.HasValue)
                    inner["ClassPreparingDeltaTime"] = s.ClassPreparingDeltaTime.Value;
                if (s.ClassOnPreparingText != null)
                    inner["ClassOnPreparingText"] = s.ClassOnPreparingText;
                if (s.OutdoorClassOnPreparingText != null)
                    inner["OutdoorClassOnPreparingText"] = s.OutdoorClassOnPreparingText;
                if (s.ClassOnPreparingMaskText != null)
                    inner["ClassOnPreparingMaskText"] = s.ClassOnPreparingMaskText;
                if (s.OutdoorClassOnPreparingMaskText != null)
                    inner["OutdoorClassOnPreparingMaskText"] = s.OutdoorClassOnPreparingMaskText;
                if (s.ClassOnMaskText != null)
                    inner["ClassOnMaskText"] = s.ClassOnMaskText;
                if (s.ClassOffMaskText != null)
                    inner["ClassOffMaskText"] = s.ClassOffMaskText;
                if (s.ClassOffOverlayText != null)
                    inner["ClassOffOverlayText"] = s.ClassOffOverlayText;

                attachedObj[innerId] = inner;
            }

            var newSubject = new JsonObject
            {
                ["Name"] = info.Name,
                ["Initial"] = info.Initial,
                ["TeacherName"] = info.TeacherName,
                ["IsOutDoor"] = info.IsOutDoor,
                ["AttachedObjects"] = attachedObj,
                ["IsActive"] = false
            };

            subjects[id] = newSubject;
            SaveProfile();
            Console.WriteLine($"已添加科目：{info.Name}（ID: {id}）");
        }

        public static void GetSubjects()
        {
            var subjects = root["Subjects"];
            Console.WriteLine(subjects.ToString());
        }

        /// <summary>
        /// 根据科目名称删除一个科目。
        /// </summary>
        public static void DeleteSubject(string name)
        {
            var subjects = root["Subjects"] as JsonObject;
            if (subjects == null)
            {
                Console.WriteLine("错误：无法找到 Subjects 节点");
                return;
            }

            string? targetKey = null;
            foreach (var subject in subjects)
            {
                var subjectObj = subject.Value as JsonObject;
                if (subjectObj != null && subjectObj["Name"]?.GetValue<string>() == name)
                {
                    targetKey = subject.Key;
                    break;
                }
            }

            if (targetKey == null)
            {
                Console.WriteLine($"错误：未找到名称为 \"{name}\" 的科目");
                return;
            }

            subjects.Remove(targetKey);
            SaveProfile();
            Console.WriteLine($"已删除科目：{name}（ID: {targetKey}）");
        }
    }
}
