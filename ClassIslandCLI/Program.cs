namespace ClassIslandCLI;

class Program
{
    static void PrintHelp()
    {
                Console.WriteLine("ClassIsLand的命令行工具");
        Console.WriteLine("--SetProfilePath:设置Default.json的位置");
                Console.WriteLine("--GetSubjects:获取科目信息");
                Console.WriteLine("--GetTimelayouts:获取时间表信息");
       Console.WriteLine("--GetClassplans:获取课表信息（含科目名称）");
       Console.WriteLine("--AddSubject <名称> <缩写> <是否室外课(true/false)> [教师名称] [可选参数...]");
       Console.WriteLine("--PExchangeClass <课表名称> <第一节> <第二节>:永久调换指定课表中两节课的顺序");
       Console.WriteLine("--TExchangeClass <课表名称> <第一节> <第二节>:临时调课（创建叠加层，保留原课表）");
       Console.WriteLine("--DeleteTimeLayout <时间表名称>:删除指定名称的时间表");
                Console.WriteLine("--AddTimeLayout <时间表名称>:添加一个新时间表");
       Console.WriteLine("--AddLayout <时间表名称> <StartTime> <EndTime> [可选参数...]");
       Console.WriteLine("  可选参数（--key value 形式）：");
                Console.WriteLine("    --StartSecond <秒数>");
                Console.WriteLine("    --EndSecond <秒数>");
                Console.WriteLine("    --TimeType <类型编号>");
                Console.WriteLine("    --IsHideDefault <true/false>");
                Console.WriteLine("    --DefaultClassId <GUID>");
               Console.WriteLine("    --ActionSet <值>");
               Console.WriteLine("--InstallCompletions:安装 Shell 补全文件");
                Console.WriteLine("--help:显示帮助");
    }
    static void Main(string[] args)
    {
        CompletionsInstaller.EnsureInstalled();
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

            if (args[i] == "--AddTimeLayout")
            {
                if (i + 1 >= args.Length)
                {
                    Console.WriteLine("用法: --AddTimeLayout <时间表名称>");
                    return;
                }
                string name = args[i + 1];
                ProfileManager.TimeLayoutManager.AddTimeLayout(name);
            }

            if (args[i] == "--AddLayout")
            {
                if (i + 3 >= args.Length)
                {
                    Console.WriteLine("用法: --AddLayout <时间表名称> <StartTime> <EndTime> [--StartSecond value] [--EndSecond value] [--TimeType value] [--IsHideDefault value] [--DefaultClassId value] [--ActionSet value]");
                    return;
                }
                string timeLayoutName = args[i + 1];
                string startTime = args[i + 2];
                string endTime = args[i + 3];

                string startSecond = "";
                string endSecond = "";
                int timeType = 0;
                bool isHideDefault = false;
                string defaultClassId = "00000000-0000-0000-0000-000000000000";
               string? actionSet = null;

                for (int j = i + 4; j < args.Length; j++)
                {
                    if (!args[j].StartsWith("--")) break;
                    string key = args[j];
                    string? val = (j + 1 < args.Length && !args[j + 1].StartsWith("--")) ? args[j + 1] : null;

                    switch (key)
                    {
                        case "--StartSecond":
                            if (val != null) { startSecond = val; j++; }
                            break;
                        case "--EndSecond":
                            if (val != null) { endSecond = val; j++; }
                            break;
                        case "--TimeType":
                            if (val != null && int.TryParse(val, out var tt)) { timeType = tt; j++; }
                            break;
                        case "--IsHideDefault":
                            if (val != null && bool.TryParse(val, out var hd)) { isHideDefault = hd; j++; }
                            break;
                        case "--DefaultClassId":
                           if (val != null) { defaultClassId = val; j++; }
                           break;
                       case "--ActionSet":
                            if (val != null) { actionSet = val; j++; }
                            break;
                    }
                }

                ProfileManager.TimeLayoutManager.AddLayout(timeLayoutName, startTime, endTime,
                    startSecond, endSecond, timeType, isHideDefault, defaultClassId, actionSet);
                return;
            }

            if (args[i] == "--DeleteTimeLayout")
            {
                if (i + 1 >= args.Length)
                {
                    Console.WriteLine("用法: --DeleteTimeLayout <时间表名称>");
                    return;
                }
                string name = args[i + 1];
                ProfileManager.TimeLayoutManager.DeleteTimeLayout(name);
            }

           if (args[i] == "--GetTimelayouts")
            {
                ProfileManager.TimeLayoutManager.GetTimelayouts();
            }

           if (args[i] == "--GetClassplans")
           {
               ProfileManager.ClassPlanManager.GetClassplans();
           }

            if (args[i] == "--PExchangeClass")
            {
                if (i + 3 >= args.Length)
                {
                    Console.WriteLine("用法: --PExchangeClass <课表名称> <第一节> <第二节>");
                    return;
                }
                string planName = args[i + 1];
                if (!int.TryParse(args[i + 2], out int idxA) || !int.TryParse(args[i + 3], out int idxB))
                {
                    Console.WriteLine("错误：课程索引必须为整数");
                    return;
                }
                if (idxA <= 0 || idxB <= 0)
                {
                    Console.WriteLine("错误：课程索引必须大于 0（第一节 = 1，第二节 = 2，以此类推）");
                    return;
                }
                ProfileManager.ClassPlanManager.PExchangeClass(planName, idxA, idxB);
                return;
            }

            if (args[i] == "--TExchangeClass")
            {
                if (i + 3 >= args.Length)
                {
                    Console.WriteLine("用法: --TExchangeClass <课表名称> <第一节> <第二节>");
                    return;
                }
                string planName = args[i + 1];
                if (!int.TryParse(args[i + 2], out int idxA) || !int.TryParse(args[i + 3], out int idxB))
                {
                    Console.WriteLine("错误：课程索引必须为整数");
                    return;
                }
                if (idxA <= 0 || idxB <= 0)
                {
                    Console.WriteLine("错误：课程索引必须大于 0（第一节 = 1，第二节 = 2，以此类推）");
                    return;
                }
                ProfileManager.ClassPlanManager.TExchangeClass(planName, idxA, idxB);
                return;
            }

            if (args[i] == "--InstallCompletions")
            {
                CompletionsInstaller.Install();
                return;
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
