namespace ClassIslandCLI;

class Program
{
    static void PrintHelp()
    {
                Console.WriteLine("ClassIsLand的命令行工具");
                Console.WriteLine("--GetSubjects:获取科目信息");
                Console.WriteLine("--GetTimelayouts:获取时间表信息");
                Console.WriteLine("--GetClassplans:获取课表信息（含科目名称）");
                Console.WriteLine("--AddSubject <名称> <缩写> <是否室外课(true/false)> [教师名称] [可选参数...]");
                Console.WriteLine("  可选参数（--key value 形式）：");
                Console.WriteLine("    --ClassOnNotificationEnabled <true/false>");
                Console.WriteLine("    --ClassOnPreparingNotificationEnabled <true/false>");
                Console.WriteLine("    --ClassOffNotificationEnabled <true/false>");
                Console.WriteLine("    --ClassPreparingDeltaTime <秒数>");
                Console.WriteLine("    --ClassOnPreparingText <文本>");
                Console.WriteLine("    --OutdoorClassOnPreparingText <文本>");
                Console.WriteLine("    --ClassOnPreparingMaskText <文本>");
                Console.WriteLine("    --OutdoorClassOnPreparingMaskText <文本>");
                Console.WriteLine("    --ClassOnMaskText <文本>");
                Console.WriteLine("    --ClassOffMaskText <文本>");
                Console.WriteLine("    --ClassOffOverlayText <文本>");
                Console.WriteLine("--help:显示帮助");
    }
    static void Main(string[] args)
    {
        if (args.Length == 0)
            {
                PrintHelp();
            }

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-h" || args[i] == "--help")
            {
               Program.PrintHelp();
            }

            if (args[i] == "-v" || args[i] == "--version")
            {
                Console.WriteLine("版本号1.0");
            }

            if (args[i] == "--SetSettingsfilePath")
            {
                SetConfig.SetSettingsfilePath(args[i + 1]);
            }
            if (args[i] == "--SetProfilePath")
            {
                SetConfig.SetProfilePath(args[i + 1]);
            }

            if (args[i] == "--AddSubject")
            {
                if (i + 3 >= args.Length)
                {
                    Console.WriteLine("用法: --AddSubject <名称> <缩写> <是否室外课(true/false)> [教师名称] [可选参数...]");
                    return;
                }

                string name = args[i + 1];
                string initial = args[i + 2];
                bool isOutDoor = bool.TryParse(args[i + 3], out var b) && b;
                string teacherName = (i + 4 < args.Length && !args[i + 4].StartsWith("--")) ? args[i + 4] : "";

                // 解析可选命名参数
                var settings = ParseAttachedSettings(args, i + (string.IsNullOrEmpty(teacherName) ? 4 : 5));

                var info = new SubjectInfo(name, isOutDoor, initial, teacherName, settings);
                ProfileManager.SubjectManager.AddSubject(info);
                return;
            }

            if (args[i] == "--GetSubjects")
            {
                ProfileManager.SubjectManager.GetSubjects();
            }

            if (args[i] == "--DeleteSubject")
            {
                if (i + 1 >= args.Length)
                {
                    Console.WriteLine("用法: --DeleteSubject <科目名称>");
                    return;
                }
                string name = args[i + 1];
                ProfileManager.SubjectManager.DeleteSubject(name);
            }

            if (args[i] == "--GetTimelayouts")
            {
                ProfileManager.TimeLayoutManager.GetTimelayouts();
            }

            if (args[i] == "--GetClassplans")
            {
                ProfileManager.ClassPlanManager.GetClassplans();
            }

        }
    }

    static SubjectAttachedSettings? ParseAttachedSettings(string[] args, int startIndex)
    {
        bool? classOnNotif = null;
        bool? classOnPrepNotif = null;
        bool? classOffNotif = null;
        int? prepDelta = null;
        string? classOnPrepText = null;
        string? outdoorClassOnPrepText = null;
        string? classOnPrepMaskText = null;
        string? outdoorClassOnPrepMaskText = null;
        string? classOnMaskText = null;
        string? classOffMaskText = null;
        string? classOffOverlayText = null;

        for (int i = startIndex; i < args.Length; i++)
        {
            if (!args[i].StartsWith("--")) break;

            string key = args[i];
            string? val = (i + 1 < args.Length && !args[i + 1].StartsWith("--")) ? args[i + 1] : null;

            switch (key)
            {
                case "--ClassOnNotificationEnabled":
                    if (val != null && bool.TryParse(val, out var b1)) { classOnNotif = b1; i++; }
                    break;
                case "--ClassOnPreparingNotificationEnabled":
                    if (val != null && bool.TryParse(val, out var b2)) { classOnPrepNotif = b2; i++; }
                    break;
                case "--ClassOffNotificationEnabled":
                    if (val != null && bool.TryParse(val, out var b3)) { classOffNotif = b3; i++; }
                    break;
                case "--ClassPreparingDeltaTime":
                    if (val != null && int.TryParse(val, out var n)) { prepDelta = n; i++; }
                    break;
                case "--ClassOnPreparingText":
                    if (val != null) { classOnPrepText = val; i++; }
                    break;
                case "--OutdoorClassOnPreparingText":
                    if (val != null) { outdoorClassOnPrepText = val; i++; }
                    break;
                case "--ClassOnPreparingMaskText":
                    if (val != null) { classOnPrepMaskText = val; i++; }
                    break;
                case "--OutdoorClassOnPreparingMaskText":
                    if (val != null) { outdoorClassOnPrepMaskText = val; i++; }
                    break;
                case "--ClassOnMaskText":
                    if (val != null) { classOnMaskText = val; i++; }
                    break;
                case "--ClassOffMaskText":
                    if (val != null) { classOffMaskText = val; i++; }
                    break;
                case "--ClassOffOverlayText":
                    if (val != null) { classOffOverlayText = val; i++; }
                    break;
            }
        }

        // 如果所有字段都为 null，返回 null（不添加 AttachedObjects）
        if (classOnNotif == null && classOnPrepNotif == null && classOffNotif == null &&
            prepDelta == null && classOnPrepText == null && outdoorClassOnPrepText == null &&
            classOnPrepMaskText == null && outdoorClassOnPrepMaskText == null &&
            classOnMaskText == null && classOffMaskText == null && classOffOverlayText == null)
            return null;

        return new SubjectAttachedSettings(
            classOnNotif, classOnPrepNotif, classOffNotif,
            prepDelta, classOnPrepText, outdoorClassOnPrepText,
            classOnPrepMaskText, outdoorClassOnPrepMaskText,
            classOnMaskText, classOffMaskText, classOffOverlayText
        );
    }
}
