using System.Text.Json;
using System.Text.Json.Nodes;

namespace ClassIslandCLI;

/// <summary>
/// --SetSubject 可选参数：对应 Subjects[].AttachedObjects 中的通知设置
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
    static string profilePath = GetConfig.GetConfigs().ProfilePath;
    static JsonNode root = LoadProfile(profilePath);

    /// <summary>
    /// 加载课表 JSON。如果路径为空或文件不存在，返回一个空的 JsonObject，
    /// 避免 ProfileManager 静态构造抛异常导致整个程序无法启动。
    /// </summary>
    static JsonNode LoadProfile(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            return new JsonObject();
        return JsonNode.Parse(File.ReadAllText(path)) ?? new JsonObject();
    }

    static void SaveProfile()
    {
        if (string.IsNullOrWhiteSpace(profilePath) || !File.Exists(profilePath))
        {
            Console.WriteLine("错误：ProfilePath 未设置或文件不存在，无法保存。请先使用 --SetProfilePath 设置。");
            return;
        }
        var options = new JsonSerializerOptions { WriteIndented = false };
        File.WriteAllText(profilePath, root.ToJsonString(options));
        File.Copy(profilePath, profilePath + ".bak", true);
        Restarter.Restart();
    }

    public static class TimeLayoutManager
    {
       public static void GetTimelayouts()
       {
           var timelayouts = root["TimeLayouts"];
           Console.WriteLine(timelayouts.ToString());
       }

        /// <summary>
        /// 添加一个新时间表。
        /// </summary>
        public static void AddTimeLayout(string name)
        {
            var timeLayouts = root["TimeLayouts"] as JsonObject;
            if (timeLayouts == null)
            {
                Console.WriteLine("错误：无法找到 TimeLayouts 节点");
                return;
            }

            // 检查是否已存在同名时间表
            foreach (var layout in timeLayouts)
            {
                var layoutObj = layout.Value as JsonObject;
                if (layoutObj != null && layoutObj["Name"]?.GetValue<string>() == name)
                {
                    Console.WriteLine($"""错误：已存在名称为 "{name}" 的时间表""");
                    return;
                }
            }

            string id = Guid.NewGuid().ToString();

            var newTimeLayout = new JsonObject
            {
                ["IsOverlay"] = false,
                ["OverlaySourceId"] = null,
                ["Name"] = name,
                ["Layouts"] = new JsonArray(),
                ["AttachedObjects"] = new JsonObject(),
                ["IsActive"] = false
            };

            timeLayouts[id] = newTimeLayout;
            SaveProfile();
            Console.WriteLine($"""已添加时间表：{name}（ID: {id}）""");
        }

        /// <summary>
        /// 向指定时间表中添加一个布局（时间块）。
        /// </summary>
        public static void AddLayout(string timeLayoutName, string startTime, string endTime,
           string startSecond = "", string endSecond = "", int timeType = 0,
           bool isHideDefault = false,
           string defaultClassId = "00000000-0000-0000-0000-000000000000",
            string? actionSet = null)
        {
            var timeLayouts = root["TimeLayouts"] as JsonObject;
            if (timeLayouts == null)
            {
                Console.WriteLine("错误：无法找到 TimeLayouts 节点");
                return;
            }

            // 查找指定名称的时间表
            string? targetKey = null;
            JsonObject? targetLayout = null;
            foreach (var layout in timeLayouts)
            {
                var layoutObj = layout.Value as JsonObject;
                if (layoutObj != null && layoutObj["Name"]?.GetValue<string>() == timeLayoutName)
                {
                    targetKey = layout.Key;
                    targetLayout = layoutObj;
                    break;
                }
            }

            if (targetLayout == null)
            {
                Console.WriteLine($"""错误：未找到名称为 "{timeLayoutName}" 的时间表""");
                return;
            }

            // 构造默认 AttachedObjects
            var attachedObjects = new JsonObject
            {
                ["7625de96-38aa-4b71-b478-3f156dd9458d"] = new JsonObject
                {
                    ["AlertShowMode"] = 2,
                    ["ForecastShowMode"] = 2,
                    ["IsAttachSettingsEnabled"] = true,
                    ["IsActive"] = false
                }
            };

            var newLayout = new JsonObject
            {
                ["StartSecond"] = startSecond,
                ["EndSecond"] = endSecond,
                ["StartTime"] = startTime,
                ["EndTime"] = endTime,
                ["TimeType"] = timeType,
                ["IsHideDefault"] = isHideDefault,
                ["DefaultClassId"] = defaultClassId,
                ["BreakName"] = "",
               ["ActionSet"] = actionSet == null ? null : JsonValue.Create(actionSet),
                ["AttachedObjects"] = attachedObjects,
                ["IsActive"] = false
            };

            var layouts = targetLayout["Layouts"] as JsonArray;
            if (layouts == null)
            {
                layouts = new JsonArray();
                targetLayout["Layouts"] = layouts;
            }

            layouts.Add((JsonNode)newLayout);
            SaveProfile();
            Console.WriteLine($"""已向时间表 "{timeLayoutName}" 添加布局：{startTime} - {endTime}（第 {layouts.Count} 个布局）""");
        }

        /// <summary>
        /// 根据时间表名称删除一个时间表。
        /// </summary>
        public static void DeleteTimeLayout(string name)
        {
            var timeLayouts = root["TimeLayouts"] as JsonObject;
            if (timeLayouts == null)
            {
                Console.WriteLine("错误：无法找到 TimeLayouts 节点");
                return;
            }

            string? targetKey = null;
            foreach (var layout in timeLayouts)
            {
                var layoutObj = layout.Value as JsonObject;
                if (layoutObj != null && layoutObj["Name"]?.GetValue<string>() == name)
                {
                    targetKey = layout.Key;
                    break;
                }
            }

            if (targetKey == null)
            {
                Console.WriteLine($"""错误：未找到名称为 "{name}" 的时间表""");
                return;
            }

            timeLayouts.Remove(targetKey);
            SaveProfile();
            Console.WriteLine($"""已删除时间表：{name}（ID: {targetKey}）""");
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

        /// <summary>
        /// 调换指定课表中两个课程的位置。
        /// </summary>
        public static void PExchangeClass(string planName, int indexA, int indexB)
        {
            var classPlans = root["ClassPlans"] as JsonObject;
            if (classPlans == null)
            {
                Console.WriteLine("错误：无法找到 ClassPlans 节点");
                return;
            }

            // 查找指定名称的课表
            JsonObject? targetPlan = null;
            string? sourceKey = null;
            foreach (var plan in classPlans)
            {
                var planObj = plan.Value as JsonObject;
                if (planObj != null && planObj["Name"]?.GetValue<string>() == planName)
                {
                    targetPlan = planObj;
                    sourceKey = plan.Key;
                    break;
                }
            }

            if (targetPlan == null)
            {
                Console.WriteLine($"""错误：未找到名称为 "{planName}" 的课表""");
                return;
            }

            var classes = targetPlan["Classes"] as JsonArray;
            if (classes == null || classes.Count == 0)
            {
                Console.WriteLine($"""错误：课表 "{planName}" 中没有课程""");
                return;
            }

            // 用户输入的是 1-based 索引，转为 0-based
            int idxA = indexA - 1;
            int idxB = indexB - 1;

            if (idxA < 0 || idxA >= classes.Count)
            {
                Console.WriteLine($"""错误：课程索引 {indexA} 超出范围（课表 "{planName}" 共有 {classes.Count} 节课）""");
                return;
            }
            if (idxB < 0 || idxB >= classes.Count)
            {
                Console.WriteLine($"""错误：课程索引 {indexB} 超出范围（课表 "{planName}" 共有 {classes.Count} 节课）""");
                return;
            }

            // 先取出两个节点的 JSON 副本，再从数组中移除，最后按交换后的顺序插入
            var jsonA = classes[idxA].ToJsonString();
            var jsonB = classes[idxB].ToJsonString();

            // 从后往前移除，避免索引偏移
            int removeFirst = Math.Max(idxA, idxB);
            int removeSecond = Math.Min(idxA, idxB);
            classes.RemoveAt(removeFirst);
            var nodeRemoved = JsonNode.Parse(removeFirst == idxA ? jsonA : jsonB)!;
            classes.RemoveAt(removeSecond);
            var nodeInserted = JsonNode.Parse(removeSecond == idxA ? jsonA : jsonB)!;

            // 按原顺序插入，但内容已交换
            classes.Insert(removeSecond, nodeRemoved);
            classes.Insert(removeFirst, nodeInserted);

            SaveProfile();
            Console.WriteLine($"""已将课表 "{planName}" 的第 {indexA} 节课与第 {indexB} 节课进行调换""");
        }
        
        /// <summary>
        /// 临时调课：保留原课表不变，复制一份新课表作为叠加层，并交换课程顺序。
        /// </summary>
        public static void TExchangeClass(string planName, int indexA, int indexB)
        {
            var classPlans = root["ClassPlans"] as JsonObject;
            if (classPlans == null)
            {
                Console.WriteLine("错误：无法找到 ClassPlans 节点");
                return;
            }

            // 查找指定名称的课表
            JsonObject? targetPlan = null;
            string? sourceKey = null;
            foreach (var plan in classPlans)
            {
                var planObj = plan.Value as JsonObject;
                    if (planObj != null && planObj["Name"]?.GetValue<string>() == planName)
                    {
                        targetPlan = planObj;
                        sourceKey = plan.Key;
                        break;
                    }
            }

            if (targetPlan == null)
            {
                Console.WriteLine($"""错误：未找到名称为 "{planName}" 的课表""");
                return;
            }

            var classes = targetPlan["Classes"] as JsonArray;
            if (classes == null || classes.Count == 0)
            {
                Console.WriteLine($"""错误：课表 "{planName}" 中没有课程""");
                return;
            }

            int idxA = indexA - 1;
            int idxB = indexB - 1;

            if (idxA < 0 || idxA >= classes.Count)
            {
                Console.WriteLine($"""错误：课程索引 {indexA} 超出范围（课表 "{planName}" 共有 {classes.Count} 节课）""");
                return;
            }
            if (idxB < 0 || idxB >= classes.Count)
            {
                Console.WriteLine($"""错误：课程索引 {indexB} 超出范围（课表 "{planName}" 共有 {classes.Count} 节课）""");
                return;
            }

            // 深拷贝原课表
            var clonedPlan = JsonNode.Parse(targetPlan.ToJsonString())!.AsObject();

            // 在副本中交换课程
            var clonedClasses = clonedPlan["Classes"]!.AsArray();
            var jsonA = clonedClasses[idxA].ToJsonString();
            var jsonB = clonedClasses[idxB].ToJsonString();
            int removeFirst = Math.Max(idxA, idxB);
            int removeSecond = Math.Min(idxA, idxB);
            clonedClasses.RemoveAt(removeFirst);
            var nodeRemoved = JsonNode.Parse(removeFirst == idxA ? jsonA : jsonB)!;
            clonedClasses.RemoveAt(removeSecond);
            var nodeInserted = JsonNode.Parse(removeSecond == idxA ? jsonA : jsonB)!;
            clonedClasses.Insert(removeSecond, nodeRemoved);
            clonedClasses.Insert(removeFirst, nodeInserted);

            // 重命名
            clonedPlan["Name"] = planName + "（临时层）";
            // 设为叠加层
            clonedPlan["IsOverlay"] = true;

            // 生成新 key 并加入 ClassPlans
            string newKey = Guid.NewGuid().ToString();
            classPlans[newKey] = clonedPlan;

            // 设置叠加层关联属性
            clonedPlan["OverlaySourceId"] = sourceKey;
            root["IsOverlayClassPlanEnabled"] = true;
            root["OverlayClassPlanId"] = newKey;

            // 生成 OverlaySetupTime 为今天日期 00:00:00 当地时间
            var now = DateTime.Now;
            var localOffset = TimeZoneInfo.Local.GetUtcOffset(now);
            var setupTime = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, localOffset);
            string setupTimeStr = setupTime.ToString("yyyy-MM-ddTHH:mm:sszzz");
            clonedPlan["OverlaySetupTime"] = setupTimeStr;

            // 在 OrderedSchedules 中添加调度项
            var orderedSchedules = root["OrderedSchedules"] as JsonObject;
            if (orderedSchedules == null)
            {
                orderedSchedules = new JsonObject();
                root["OrderedSchedules"] = orderedSchedules;
            }
            orderedSchedules[setupTimeStr] = new JsonObject
            {
                ["ClassPlanId"] = newKey,
                ["IsActive"] = false
            };

            SaveProfile();
            Console.WriteLine($"""已为课表 "{planName}" 创建临时调课叠加层，第 {indexA} 节课与第 {indexB} 节课已调换""");
        }
    }

    public static class SubjectManager
    {
        /// <summary>
        /// 设置科目：同名则覆盖（保留原 UUID），否则新增。
        /// </summary>
        public static void SetSubject(SubjectInfo info)
        {
            var subjects = root["Subjects"] as JsonObject;
            if (subjects == null)
            {
                Console.WriteLine("错误：无法找到 Subjects 节点");
                return;
            }

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

            // 查找是否已存在同名科目
            string? existingKey = null;
            foreach (var subject in subjects)
            {
                var subjectObj = subject.Value as JsonObject;
                if (subjectObj != null && subjectObj["Name"]?.GetValue<string>() == info.Name)
                {
                    existingKey = subject.Key;
                    break;
                }
            }

            if (existingKey != null)
            {
                // 同名科目已存在：更新字段，保留原有 key（UUID）
                var existing = subjects[existingKey] as JsonObject;
                if (existing != null)
                {
                    existing["Name"] = info.Name;
                    existing["Initial"] = info.Initial;
                    existing["TeacherName"] = info.TeacherName;
                    existing["IsOutDoor"] = info.IsOutDoor;
                    existing["AttachedObjects"] = attachedObj;
                    existing["IsActive"] = false;
                }
                SaveProfile();
                Console.WriteLine($"""已更新科目：{info.Name}（ID: {existingKey}）""");
            }
            else
            {
                // 不存在同名科目：新增
                string id = Guid.NewGuid().ToString();
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
                Console.WriteLine($"""已添加科目：{info.Name}（ID: {id}）""");
            }
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
                Console.WriteLine($"""错误：未找到名称为 "{name}" 的科目""");
                return;
            }

            subjects.Remove(targetKey);
            SaveProfile();
            Console.WriteLine($"""已删除科目：{name}（ID: {targetKey}）""");
        }
    }
}
