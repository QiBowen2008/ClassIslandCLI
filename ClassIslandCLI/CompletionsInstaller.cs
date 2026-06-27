using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace ClassIslandCLI
{
    public static class CompletionsInstaller
    {
        private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private static bool IsMacOS   => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        private static bool IsLinux   => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        private static string FlagFile => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ClassIslandCLI", ".completions_installed");

        private static string DataDir => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ClassIslandCLI", "completions");

        private static readonly string PsCompletion = @"
# ClassIslandCLI PowerShell 补全
# Auto-installed by ClassIslandCLI

Register-ArgumentCompleter -CommandName classislandcli, ClassIslandCLI, dotnet -ParameterName args -ScriptBlock {
    param($wordToComplete, $commandAst, $cursorPosition)

    $allArgs = $commandAst.CommandElements |
        Where-Object { $_.GetType().Name -eq 'StringConstantExpressionAst' } |
        Select-Object -ExpandProperty Value

    $start = 0
    if ($allArgs[0] -eq 'dotnet' -and $allArgs[1] -eq 'run') {
        $start = 2
        if ($allArgs[2] -eq 'classislandcli' -or $allArgs[2] -eq 'ClassIslandCLI') {
            $start = 3
        }
    }
    $cliArgs = $allArgs[$start..($allArgs.Count - 1)]
    if ($null -eq $cliArgs) { $cliArgs = @() }

    $topCommands = @(
        '--help', '-h',
        '--version', '-v',
        '--SetProfilePath',
        '--SetSettingsfilePath',
        '--GetSubjects',
        '--GetTimelayouts',
        '--GetClassplans',
        '--AddSubject',
        '--DeleteSubject',
        '--AddTimeLayout',
        '--AddLayout',
        '--DeleteTimeLayout',
        '--PExchangeClass',
        '--TExchangeClass',
        '--InstallCompletions'
    )

    $foundCommand = $false
    $currentCommand = ''
    $cmdIndex = -1
    for ($i = 0; $i -lt $cliArgs.Count; $i++) {
        if ($cliArgs[$i] -match '^--') {
            $foundCommand = $true
            $currentCommand = $cliArgs[$i]
            $cmdIndex = $i
            break
        }
    }

    if (-not $foundCommand) {
        return $topCommands | Where-Object { $_ -like ""$wordToComplete*"" }
    }

    $paramsAfterCmd = $cliArgs[($cmdIndex + 1)..($cliArgs.Count - 1)]
    if ($null -eq $paramsAfterCmd) { $paramsAfterCmd = @() }
    $posArgCount = ($paramsAfterCmd | Where-Object { -not ($_ -match '^--') }).Count

    $optionalParams = @(
        '--StartSecond', '--EndSecond', '--TimeType', '--IsHideDefault',
        '--DefaultClassId', '--ActionSet', '--ClassOnNotificationEnabled',
        '--ClassOnPreparingNotificationEnabled', '--ClassOffNotificationEnabled',
        '--ClassOnMaskText', '--ClassOnPreparingMaskText', '--ClassOffMaskText',
        '--ClassOnPreparingText', '--ClassOffOverlayText',
        '--OutdoorClassOnPreparingText', '--OutdoorClassOnPreparingMaskText',
        '--ClassPreparingDeltaTime'
    )

    switch ($currentCommand) {
        '--SetProfilePath' {
            if ($posArgCount -eq 0) { return @() }
        }
        '--SetSettingsfilePath' {
            if ($posArgCount -eq 0) { return @() }
        }
        '--AddSubject' {
            if ($posArgCount -le 2) {
                switch ($posArgCount) {
                    0 { return @() }
                    1 { return @() }
                    2 { return 'true', 'false' | Where-Object { $_ -like ""$wordToComplete*"" } }
                }
            }
            if ($wordToComplete -match '^--') {
                return $optionalParams | Where-Object { $_ -like ""$wordToComplete*"" }
            }
            return @()
        }
        '--DeleteSubject' {
            if ($posArgCount -eq 0) { return @() }
        }
        '--AddTimeLayout' {
            if ($posArgCount -eq 0) { return @() }
        }
        '--AddLayout' {
            if ($posArgCount -le 2) {
                if ($posArgCount -eq 0) { return @() }
                if ($posArgCount -eq 1) { return @() }
                if ($posArgCount -eq 2) { return @() }
            }
            if ($wordToComplete -match '^--') {
                return $optionalParams | Where-Object { $_ -like ""$wordToComplete*"" }
            }
            return @()
        }
        '--DeleteTimeLayout' {
            if ($posArgCount -eq 0) { return @() }
        }
        '--PExchangeClass' {
            if ($posArgCount -le 2) { return @() }
        }
        '--TExchangeClass' {
            if ($posArgCount -le 2) { return @() }
        }
        default { return @() }
    }
}
";

        private static readonly string FishCompletion = @"
# ClassIslandCLI fish shell 补全
# Auto-installed by ClassIslandCLI

complete -c ClassIslandCLI -s h -l help        -d ""显示帮助""
complete -c ClassIslandCLI -s v -l version     -d ""显示版本""
complete -c ClassIslandCLI -l SetProfilePath    -d ""设置 Default.json 路径""          -x -a ""(__fish_complete_path)""
complete -c ClassIslandCLI -l SetSettingsfilePath -d ""设置 Settings.json 路径""       -x -a ""(__fish_complete_path)""
complete -c ClassIslandCLI -l GetSubjects       -d ""获取科目信息""
complete -c ClassIslandCLI -l GetTimelayouts    -d ""获取时间表信息""
complete -c ClassIslandCLI -l GetClassplans     -d ""获取课表信息（含科目名称）""
complete -c ClassIslandCLI -l AddSubject        -d ""添加新科目""                      -x
complete -c ClassIslandCLI -l DeleteSubject     -d ""删除科目""                        -x
complete -c ClassIslandCLI -l AddTimeLayout     -d ""添加新时间表""                    -x
complete -c ClassIslandCLI -l AddLayout         -d ""向时间表添加时间段""              -x
complete -c ClassIslandCLI -l DeleteTimeLayout  -d ""删除时间表""                      -x
complete -c ClassIslandCLI -l PExchangeClass    -d ""永久调课：直接交换课表中两节课""  -x
complete -c ClassIslandCLI -l TExchangeClass    -d ""临时调课：创建叠加层副本交换课程""-x
complete -c ClassIslandCLI -l InstallCompletions -d ""安装 Shell 补全""

set -l optional_opts \
    StartSecond     '秒数' \
    EndSecond       '秒数' \
    TimeType        '类型编号' \
    IsHideDefault   'true/false' \
    DefaultClassId  '科目 GUID' \
    ActionSet       '动作集名称' \
    ClassOnNotificationEnabled         'true/false' \
    ClassOnPreparingNotificationEnabled 'true/false' \
    ClassOffNotificationEnabled        'true/false' \
    ClassOnMaskText        '文本' \
    ClassOnPreparingMaskText '文本' \
    ClassOffMaskText       '文本' \
    ClassOnPreparingText   '文本' \
    ClassOffOverlayText    '文本' \
    OutdoorClassOnPreparingText     '文本' \
    OutdoorClassOnPreparingMaskText '文本' \
    ClassPreparingDeltaTime '秒数'

for i in (seq 1 2 (count $optional_opts))
    set opt $optional_opts[$i]
    set desc $optional_opts[(math $i + 1)]
    complete -c ClassIslandCLI -l ""$opt"" -d ""$desc"" -x -n ""__fish_seen_subcommand_from --AddLayout; or __fish_seen_subcommand_from --AddSubject""
end
";

        private static readonly string ZshCompletion = @"
#compdef classislandcli

# ClassIslandCLI zsh 补全
# Auto-installed by ClassIslandCLI

local -a top_commands
top_commands=(
    '(-h --help)'{-h,--help}'[显示帮助]'
    '(-v --version)'{-v,--version}'[显示版本]'
    '--SetProfilePath[设置 Default.json 路径]:文件路径:_files'
    '--SetSettingsfilePath[设置 Settings.json 路径]:文件路径:_files'
    '--GetSubjects[获取科目信息]'
    '--GetTimelayouts[获取时间表信息]'
    '--GetClassplans[获取课表信息（含科目名称）]'
    '--AddSubject[添加新科目]: :->addsubject-args'
    '--DeleteSubject[删除科目]:科目名称:'
    '--AddTimeLayout[添加新时间表]:时间表名称:'
    '--AddLayout[向时间表添加时间段]: :->addlayout-args'
    '--DeleteTimeLayout[删除时间表]:时间表名称:'
    '--PExchangeClass[永久调课：直接交换两节课]: :->exchange-args'
    '--TExchangeClass[临时调课：创建叠加层副本交换课程]: :->exchange-args'
    '--InstallCompletions[安装 Shell 补全]'
)

local -a optional_opts
optional_opts=(
    '--StartSecond[秒数]:秒数:'
    '--EndSecond[秒数]:秒数:'
    '--TimeType[类型编号]:编号:'
    '--IsHideDefault[是否隐藏默认]:布尔:(true false)'
    '--DefaultClassId[科目 GUID]:GUID:'
    '--ActionSet[动作集名称]:动作集:'
    '--ClassOnNotificationEnabled[上课通知启用]:布尔:(true false)'
    '--ClassOnPreparingNotificationEnabled[预备通知启用]:布尔:(true false)'
    '--ClassOffNotificationEnabled[下课通知启用]:布尔:(true false)'
    '--ClassOnMaskText[上课遮罩文本]:文本:'
    '--ClassOnPreparingMaskText[预备遮罩文本]:文本:'
    '--ClassOffMaskText[下课遮罩文本]:文本:'
    '--ClassOnPreparingText[预备文本]:文本:'
    '--ClassOffOverlayText[下课叠加文本]:文本:'
    '--OutdoorClassOnPreparingText[室外课预备文本]:文本:'
    '--OutdoorClassOnPreparingMaskText[室外课预备遮罩文本]:文本:'
    '--ClassPreparingDeltaTime[预备提前秒数]:秒数:'
)

_arguments -C $top_commands && return

case $state in
    addsubject-args)
        _arguments \
            '1: :_message ""科目名称""' \
            '2: :_message ""缩写""' \
            '3: :(true false)' \
            '4: :_message ""教师名称（可选）""' \
            $optional_opts
        ;;
    addlayout-args)
        _arguments \
            '1: :_message ""时间表名称""' \
            '2: :_message ""开始时间 (HH:mm:ss)""' \
            '3: :_message ""结束时间 (HH:mm:ss)""' \
            $optional_opts
        ;;
    exchange-args)
        _arguments \
            '1: :_message ""课表名称""' \
            '2: :_message ""第一节 (1-based 索引)""' \
            '3: :_message ""第二节 (1-based 索引)""'
        ;;
esac
";

        // ====================================================================
        // 公开 API
        // ====================================================================

        public static bool NeedsInstall()
        {
            return !File.Exists(FlagFile);
        }

        /// <summary>
        /// 每次启动时调用，首次运行时自动安装补全。
        /// </summary>
        public static void EnsureInstalled()
        {
            if (!NeedsInstall()) return;
            try
            {
                Install();
            }
            catch
            {
                // 自动安装失败不阻塞程序运行
            }
        }

        /// <summary>
        /// 显式安装 Shell 补全（--InstallCompletions 调用）。
        /// </summary>
        public static void Install()
        {
            Console.WriteLine("=== ClassIslandCLI Shell 补全安装 ===");
            int count = 0;

            if (IsWindows)
                count += InstallOnWindows();
            else
                count += InstallOnUnix();

            if (count == 0)
                Console.WriteLine("未安装任何补全。请确保已安装支持的 Shell。");
            else
                Console.WriteLine($"安装完成，共 {count} 个 Shell 补全生效。");

            // 写入标记文件
            Directory.CreateDirectory(Path.GetDirectoryName(FlagFile)!);
            File.WriteAllText(FlagFile, DateTime.Now.ToString("O"));
        }

        // ====================================================================
        // Windows 安装
        // ====================================================================

        private static int InstallOnWindows()
        {
            int count = 0;

            // 1) PowerShell
            if (TryInstallPowerShell())
            {
                Console.WriteLine("  [OK] PowerShell 补全已安装");
                count++;
            }
            else
                Console.WriteLine("  [--] PowerShell 未检测到或安装失败");

            // 2) Cygwin
            string? cygwinRoot = FindCygwinRoot();
            if (cygwinRoot != null)
            {
                string home = GetCygwinHome(cygwinRoot);
                if (TryInstallFish(home))
                { Console.WriteLine("  [OK] fish (Cygwin) 补全已安装"); count++; }
                else
                    Console.WriteLine("  [--] fish (Cygwin) 未检测到或安装失败");

                if (TryInstallZsh(home))
                { Console.WriteLine("  [OK] zsh (Cygwin) 补全已安装"); count++; }
                else
                    Console.WriteLine("  [--] zsh (Cygwin) 未检测到或安装失败");
            }
            else
                Console.WriteLine("  [--] Cygwin 未检测到，跳过 fish/zsh");

            // 3) MSYS2
            string? msys2Root = FindMsys2Root();
            if (msys2Root != null)
            {
                string home = GetCygwinHome(msys2Root);
                if (TryInstallFish(home))
                { Console.WriteLine("  [OK] fish (MSYS2) 补全已安装"); count++; }
                else
                    Console.WriteLine("  [--] fish (MSYS2) 未检测到或安装失败");

                if (TryInstallZsh(home))
                { Console.WriteLine("  [OK] zsh (MSYS2) 补全已安装"); count++; }
                else
                    Console.WriteLine("  [--] zsh (MSYS2) 未检测到或安装失败");
            }
            else
                Console.WriteLine("  [--] MSYS2 未检测到，跳过 fish/zsh");

            return count;
        }

        // ====================================================================
        // Unix (macOS / Linux) 安装
        // ====================================================================

        private static int InstallOnUnix()
        {
            int count = 0;
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // 1) fish
            if (TryInstallFish(home))
            { Console.WriteLine("  [OK] fish 补全已安装"); count++; }
            else
                Console.WriteLine("  [--] fish 未检测到或安装失败");

            // 2) zsh
            if (TryInstallZsh(home))
            { Console.WriteLine("  [OK] zsh 补全已安装"); count++; }
            else
                Console.WriteLine("  [--] zsh 未检测到或安装失败");

            // 3) pwsh
            if (TryInstallPwshUnix(home))
            { Console.WriteLine("  [OK] PowerShell (pwsh) 补全已安装"); count++; }
            else
                Console.WriteLine("  [--] pwsh 未检测到或安装失败");

            return count;
        }

        // ====================================================================
        // PowerShell (Windows)
        // ====================================================================

        private static bool TryInstallPowerShell()
        {
            try
            {
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                var profiles = new List<string>();

                string pwshProfileDir = Path.Combine(userProfile, "Documents", "PowerShell");
                string pwshProfile = Path.Combine(pwshProfileDir, "Microsoft.PowerShell_profile.ps1");
                if (Directory.Exists(pwshProfileDir) || HasPwshInPath())
                    profiles.Add(pwshProfile);

                string winPsProfileDir = Path.Combine(userProfile, "Documents", "WindowsPowerShell");
                string winPsProfile = Path.Combine(winPsProfileDir, "Microsoft.PowerShell_profile.ps1");
                if (Directory.Exists(winPsProfileDir) || HasWindowsPowerShellInPath())
                    profiles.Add(winPsProfile);

                if (profiles.Count == 0) return false;

                Directory.CreateDirectory(DataDir);
                string psScriptPath = Path.Combine(DataDir, "classislandcli.ps1");
                File.WriteAllText(psScriptPath, PsCompletion);

                string dotSourceLine = $@". ""{psScriptPath}""";

                foreach (var profilePath in profiles)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(profilePath)!);

                    if (File.Exists(profilePath))
                    {
                        string content = File.ReadAllText(profilePath);
                        if (content.Contains(dotSourceLine)) continue;
                    }

                    File.AppendAllText(profilePath,
                        (File.Exists(profilePath) ? "\n" : "") +
                        $"# ClassIslandCLI completion{Environment.NewLine}{dotSourceLine}{Environment.NewLine}");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // ====================================================================
        // pwsh (Unix)
        // ====================================================================

        private static bool TryInstallPwshUnix(string home)
        {
            try
            {
                // 检查 pwsh 是否在 PATH 中
                if (!HasPwshInPath()) return false;

                // pwsh 在 Linux/macOS 上通常使用 ~/.config/powershell/
                string pwshDir;
                if (IsMacOS)
                    pwshDir = Path.Combine(home, ".config", "powershell");
                else
                    pwshDir = Path.Combine(home, ".config", "powershell");  // Linux also uses ~/.config/powershell

                string profilePath = Path.Combine(pwshDir, "Microsoft.PowerShell_profile.ps1");
                Directory.CreateDirectory(pwshDir);

                Directory.CreateDirectory(DataDir);
                string psScriptPath = Path.Combine(DataDir, "classislandcli.ps1");
                File.WriteAllText(psScriptPath, PsCompletion);

                string dotSourceLine = $@". ""{psScriptPath}""";

                if (File.Exists(profilePath))
                {
                    string content = File.ReadAllText(profilePath);
                    if (content.Contains(dotSourceLine)) return true;
                }

                File.AppendAllText(profilePath,
                    (File.Exists(profilePath) ? "\n" : "") +
                    $"# ClassIslandCLI completion{Environment.NewLine}{dotSourceLine}{Environment.NewLine}");

                return true;
            }
            catch
            {
                return false;
            }
        }

        // ====================================================================
        // fish (跨平台)
        // ====================================================================

        private static bool TryInstallFish(string home)
        {
            try
            {
                // fish 补全目录在所有平台上都是 ~/.config/fish/completions/
                string fishDir = Path.Combine(home, ".config", "fish", "completions");

                // 只有用户目录中已有 .config/fish 才安装（说明 fish 已初始化过）
                if (!Directory.Exists(Path.Combine(home, ".config", "fish")))
                {
                    // 如果不在 PATH 中，放弃
                    if (!HasCommandInPath("fish")) return false;
                }

                Directory.CreateDirectory(fishDir);
                // fish 自动加载规则：文件名必须与命令名一致（大小写敏感）
                // 写入两个文件以兼容 classislandcli 和 ClassIslandCLI 两种调用方式
                File.WriteAllText(Path.Combine(fishDir, "classislandcli.fish"), FishCompletion);
                File.WriteAllText(Path.Combine(fishDir, "ClassIslandCLI.fish"), FishCompletion);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ====================================================================
        // zsh (跨平台)
        // ====================================================================

        private static bool TryInstallZsh(string home)
        {
            try
            {
                // 在 Unix 上使用 XDG 标准路径；在 Cygwin/MSYS2 上也通用
                string zshCompDir = Path.Combine(home, ".zsh", "completions");
                Directory.CreateDirectory(zshCompDir);
                File.WriteAllText(Path.Combine(zshCompDir, "_classislandcli"), ZshCompletion);

                // 确保 .zshrc 中有 fpath 设置
                string zshrc = Path.Combine(home, ".zshrc");
                string fpathLine = "fpath=(~/.zsh/completions $fpath)";

                if (!File.Exists(zshrc) && !HasCommandInPath("zsh"))
                    return false;  // zsh 未安装

                if (File.Exists(zshrc))
                {
                    string content = File.ReadAllText(zshrc);
                    if (!content.Contains(fpathLine))
                    {
                        File.AppendAllText(zshrc,
                            $"{Environment.NewLine}# ClassIslandCLI completion{Environment.NewLine}{fpathLine}{Environment.NewLine}");
                    }
                }
                else
                {
                    File.WriteAllText(zshrc,
                        $"# ClassIslandCLI completion{Environment.NewLine}{fpathLine}{Environment.NewLine}");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // ====================================================================
        // 环境探测
        // ====================================================================

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private static string? FindCygwinRoot()
        {
            // 注册表（64-bit）
            string? fromReg = TryGetRegistryValue(@"SOFTWARE\Cygwin\setup", "rootdir");
            if (fromReg != null && Directory.Exists(fromReg)) return fromReg;

            // WOW6432Node（32-bit Cygwin on 64-bit Windows）
            fromReg = TryGetRegistryValue(@"SOFTWARE\WOW6432Node\Cygwin\setup", "rootdir");
            if (fromReg != null && Directory.Exists(fromReg)) return fromReg;

            // 回退：常见路径
            string[] commonPaths = { @"C:\cygwin64", @"C:\cygwin" };
            foreach (var p in commonPaths)
                if (Directory.Exists(p)) return p;

            return null;
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private static string? FindMsys2Root()
        {
            // MSYS2 注册表
            var regPaths = new[]
            {
                @"SOFTWARE\MSYS2",
                @"SOFTWARE\WOW6432Node\MSYS2",
                @"SOFTWARE\MSYS2\setup",
                @"SOFTWARE\WOW6432Node\MSYS2\setup"
            };

            foreach (var rp in regPaths)
            {
                string? root = TryGetRegistryValue(rp, "InstallPath") ?? TryGetRegistryValue(rp, "rootdir");
                if (root != null && Directory.Exists(root)) return root;
            }

            // 回退：常见路径
            string[] commonPaths = {
                @"C:\msys64",
                @"C:\msys2",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "msys64"),
                @"D:\msys64",
                @"D:\msys2"
            };
            foreach (var p in commonPaths)
                if (Directory.Exists(p)) return p;

            return null;
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private static string? TryGetRegistryValue(string keyPath, string valueName)
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(keyPath);
                if (key != null)
                {
                    return key.GetValue(valueName) as string;
                }
            }
            catch { }
            return null;
        }

        private static string GetCygwinHome(string root)
        {
            string userName = Environment.UserName;
            return Path.Combine(root, "home", userName);
        }

        private static bool HasPwshInPath()
        {
            return HasCommandInPath("pwsh") || HasCommandInPath("pwsh.exe");
        }

        private static bool HasWindowsPowerShellInPath()
        {
            return HasCommandInPath("powershell.exe");
        }

        private static bool HasCommandInPath(string command)
        {
            try
            {
                var paths = (Environment.GetEnvironmentVariable("PATH") ?? "")
                    .Split(Path.PathSeparator);
                foreach (var p in paths)
                {
                    if (File.Exists(Path.Combine(p, command)))
                        return true;
                    // Windows also checks PATHEXT for .exe-less names
                    if (!command.Contains('.'))
                    {
                        var exts = (Environment.GetEnvironmentVariable("PATHEXT") ?? ".EXE;.CMD;.BAT")
                            .Split(';');
                        foreach (var ext in exts)
                        {
                            if (File.Exists(Path.Combine(p, command + ext)))
                                return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }
    }
}