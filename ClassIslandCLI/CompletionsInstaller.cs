//此文件为AI生成，不能保证补全脚本能够正确添加并运行，尤其是Mac，这个平台我目前无法测试
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

        private static readonly string PsProfileSnippet = @"

# region ClassIslandCLI completion
try {
    [string[]]$c = @('--help', '-h', '--version', '-v', '--SetProfilePath', '--SetClassIslandPath', '--GetSubjects', '--GetTimelayouts', '--GetClassplans', '--SetSubject', '--DeleteSubject', '--AddTimeLayout', '--AddLayout', '--DeleteTimeLayout', '--PExchangeClass', '--TExchangeClass', '--InstallCompletions', '--InstallSkills')
    Register-ArgumentCompleter -Native -CommandName classislandcli, ClassIslandCLI -ScriptBlock {
        param($w, $a, $p)
        if ($w -match '^-') { return $c | ? { $_ -like ""$w*"" } }
        $r = @($a.CommandElements | select -Skip 1 | % { $_.Extent.Text })
        if (-not ($r | ? { $_ -match '^--' })) { return $c | ? { $_ -like ""$w*"" } }
        return @()
    }
} catch {}
# endregion
";



        private static readonly string FishCompletion = @"
# ClassIslandCLI fish shell 补全
# Auto-installed by ClassIslandCLI

complete -c ClassIslandCLI -s h -l help        -d ""显示帮助""
complete -c ClassIslandCLI -s v -l version     -d ""显示版本""
complete -c ClassIslandCLI -l SetProfilePath    -d ""设置 Default.json 路径""          -x -a ""(__fish_complete_path)""
complete -c ClassIslandCLI -l SetClassIslandPath -d ""设置 ClassIsland 路径""       -x -a ""(__fish_complete_path)""
complete -c ClassIslandCLI -l GetSubjects       -d ""获取科目信息""
complete -c ClassIslandCLI -l GetTimelayouts    -d ""获取时间表信息""
complete -c ClassIslandCLI -l GetClassplans     -d ""获取课表信息（含科目名称）""
complete -c ClassIslandCLI -l SetSubject        -d ""添加/编辑科目""                      -x
complete -c ClassIslandCLI -l DeleteSubject     -d ""删除科目""                        -x
complete -c ClassIslandCLI -l AddTimeLayout     -d ""添加新时间表""                    -x
complete -c ClassIslandCLI -l AddLayout         -d ""向时间表添加时间段""              -x
complete -c ClassIslandCLI -l DeleteTimeLayout  -d ""删除时间表""                      -x
complete -c ClassIslandCLI -l PExchangeClass    -d ""永久调课：直接交换课表中两节课""  -x
complete -c ClassIslandCLI -l TExchangeClass    -d ""临时调课：创建叠加层副本交换课程""-x
complete -c ClassIslandCLI -l InstallCompletions -d ""安装 Shell 补全""
complete -c ClassIslandCLI -l InstallSkills       -d ""安装 ClassIslandCLI Skills""

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
    complete -c ClassIslandCLI -l ""$opt"" -d ""$desc"" -x -n ""__fish_seen_subcommand_from --AddLayout; or __fish_seen_subcommand_from --SetSubject""
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
    '--SetClassIslandPath[设置 ClassIsland 路径]:文件路径:_files'
    '--GetSubjects[获取科目信息]'
    '--GetTimelayouts[获取时间表信息]'
    '--GetClassplans[获取课表信息（含科目名称）]'
    '--SetSubject[添加/编辑科目]: :->addsubject-args'
    '--DeleteSubject[删除科目]:科目名称:'
    '--AddTimeLayout[添加新时间表]:时间表名称:'
    '--AddLayout[向时间表添加时间段]: :->addlayout-args'
    '--DeleteTimeLayout[删除时间表]:时间表名称:'
    '--PExchangeClass[永久调课：直接交换两节课]: :->exchange-args'
    '--TExchangeClass[临时调课：创建叠加层副本交换课程]: :->exchange-args'
    '--InstallCompletions[安装 Shell 补全]'
    '--InstallSkills[安装 ClassIslandCLI Skills]'
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

        private static readonly string BashCompletion = @"
# ClassIslandCLI bash 补全
# Auto-installed by ClassIslandCLI

_classislandcli() {
    local cur=""${COMP_WORDS[COMP_CWORD]}""
    local prev=""${COMP_WORDS[COMP_CWORD-1]}""

    # First argument: suggest all top-level commands
    if [[ $COMP_CWORD -eq 1 ]]; then
        COMPREPLY=($(compgen -W ""--help -h --version -v --SetProfilePath --SetClassIslandPath --GetSubjects --GetTimelayouts --GetClassplans --SetSubject --DeleteSubject --AddTimeLayout --AddLayout --DeleteTimeLayout --PExchangeClass --TExchangeClass --InstallCompletions --InstallSkills"" -- ""$cur""))
        return
    fi

    # Find which command was given
    local cmd=""""
    for ((i=1; i<COMP_CWORD; i++)); do
        if [[ ""${COMP_WORDS[i]}"" == --* ]] || [[ ""${COMP_WORDS[i]}"" == -* ]]; then
            cmd=""${COMP_WORDS[i]}""
            break
        fi
    done
    [[ -z ""$cmd"" ]] && return

    # Count positional (non-option) args before the current word
    local pos=0
    for ((i=2; i<COMP_CWORD; i++)); do
        [[ ""${COMP_WORDS[i]}"" != --* ]] && ((pos++))
    done

    local optional=""--StartSecond --EndSecond --TimeType --IsHideDefault --DefaultClassId --ActionSet --ClassOnNotificationEnabled --ClassOnPreparingNotificationEnabled --ClassOffNotificationEnabled --ClassOnMaskText --ClassOnPreparingMaskText --ClassOffMaskText --ClassOnPreparingText --ClassOffOverlayText --OutdoorClassOnPreparingText --OutdoorClassOnPreparingMaskText --ClassPreparingDeltaTime""

    case ""$cmd"" in
        --SetSubject)
            if [[ ""$cur"" == --* ]]; then
                COMPREPLY=($(compgen -W ""$optional"" -- ""$cur""))
            elif [[ $pos -eq 2 ]]; then
                COMPREPLY=($(compgen -W ""true false"" -- ""$cur""))
            fi
            ;;
        --AddLayout)
            if [[ ""$cur"" == --* ]]; then
                COMPREPLY=($(compgen -W ""$optional"" -- ""$cur""))
            fi
            ;;
        --SetProfilePath|--SetClassIslandPath|--DeleteSubject|--AddTimeLayout|--DeleteTimeLayout)
            ;;
        --PExchangeClass|--TExchangeClass)
            ;;
    esac
}

complete -F _classislandcli classislandcli ClassIslandCLI
";

        private static readonly string ClinkCompletion = @"
-- ClassIslandCLI clink 补全
-- Auto-installed by ClassIslandCLI

local commands = {
    ""--help"", ""-h"",
    ""--version"", ""-v"",
    ""--SetProfilePath"", ""--SetClassIslandPath"",
    ""--GetSubjects"", ""--GetTimelayouts"", ""--GetClassplans"",
    ""--SetSubject"", ""--DeleteSubject"",
    ""--AddTimeLayout"", ""--AddLayout"",
    ""--DeleteTimeLayout"",
    ""--PExchangeClass"", ""--TExchangeClass"",
    ""--InstallCompletions"", ""--InstallSkills""
}

clink.argmatcher(""classislandcli"")
    :addflags(commands)

clink.argmatcher(""ClassIslandCLI"")
    :addflags(commands)
";

        private static readonly string NuCompletion = @"
# ClassIslandCLI nushell 补全
# Auto-installed by ClassIslandCLI

export extern ""classislandcli"" [
    --help(-h)                      # 显示帮助信息
    --version(-v)                   # 显示版本号
    --SetProfilePath: path          # 设置 Default.json 路径
    --SetClassIslandPath: path      # 设置 ClassIsland 路径
    --GetSubjects                   # 获取科目信息
    --GetTimelayouts                # 获取时间表信息
    --GetClassplans                 # 获取课表信息
    --SetSubject                    # 添加/编辑科目
    --DeleteSubject                 # 删除科目
    --AddTimeLayout                 # 添加新时间表
    --AddLayout                     # 向时间表添加时间段
    --DeleteTimeLayout              # 删除时间表
    --PExchangeClass                # 永久调课
    --TExchangeClass                # 临时调课
    --InstallCompletions            # 安装 Shell 补全
    --InstallSkills                 # 安装 Skills
]

export extern ""ClassIslandCLI"" [
    --help(-h)
    --version(-v)
    --SetProfilePath: path
    --SetClassIslandPath: path
    --GetSubjects
    --GetTimelayouts
    --GetClassplans
    --SetSubject
    --DeleteSubject
    --AddTimeLayout
    --AddLayout
    --DeleteTimeLayout
    --PExchangeClass
    --TExchangeClass
    --InstallCompletions
    --InstallSkills
]
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
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // 1) pwsh (PowerShell 7+)
            if (TryInstallPwsh(userProfile))
            {
                Console.WriteLine("  [OK] pwsh 补全已安装");
                count++;
            }
            else
                Console.WriteLine("  [--] pwsh 未检测到或安装失败");

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

                if (TryInstallBash(home))
                { Console.WriteLine("  [OK] bash (Cygwin) 补全已安装"); count++; }
                else
                    Console.WriteLine("  [--] bash (Cygwin) 未检测到或安装失败");
            }
            else
                Console.WriteLine("  [--] Cygwin 未检测到，跳过 fish/zsh/bash");

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

                if (TryInstallBash(home))
                { Console.WriteLine("  [OK] bash (MSYS2) 补全已安装"); count++; }
                else
                    Console.WriteLine("  [--] bash (MSYS2) 未检测到或安装失败");
            }
            else
                Console.WriteLine("  [--] MSYS2 未检测到，跳过 fish/zsh/bash");

            // 4) Clink
            if (TryInstallClink())
            { Console.WriteLine("  [OK] clink 补全已安装"); count++; }
            else
                Console.WriteLine("  [--] clink 未检测到或安装失败");

            // 5) Nushell
            if (TryInstallNushell(userProfile))
            { Console.WriteLine("  [OK] nushell 补全已安装"); count++; }
            else
                Console.WriteLine("  [--] nushell 未检测到或安装失败");

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
            if (TryInstallPwsh(home))
            { Console.WriteLine("  [OK] pwsh 补全已安装"); count++; }
            else
                Console.WriteLine("  [--] pwsh 未检测到或安装失败");

            // 4) bash
            if (TryInstallBash(home))
            { Console.WriteLine("  [OK] bash 补全已安装"); count++; }
            else
                Console.WriteLine("  [--] bash 未检测到或安装失败");

            // 5) nushell
            if (TryInstallNushell(home))
            { Console.WriteLine("  [OK] nushell 补全已安装"); count++; }
            else
                Console.WriteLine("  [--] nushell 未检测到或安装失败");

            return count;
        }

        // ====================================================================
        // pwsh (跨平台，PowerShell 7+)
        // ====================================================================

        private static bool TryInstallPwsh(string home)
        {
            try
            {
                if (!HasPwshInPath()) return false;

                string pwshDir;
                if (IsWindows)
                    pwshDir = Path.Combine(home, "Documents", "PowerShell");
                else
                    pwshDir = Path.Combine(home, ".config", "powershell");

                string profilePath = Path.Combine(pwshDir, "Microsoft.PowerShell_profile.ps1");
                Directory.CreateDirectory(pwshDir);

                // 强制覆盖：先移除旧版 ClassIslandCLI 段落，再写入新版
                string content = "";
                if (File.Exists(profilePath))
                    content = File.ReadAllText(profilePath);

                content = StripSection(content, "# region ClassIslandCLI completion", "# endregion");

                File.WriteAllText(profilePath,
                    content.TrimEnd() + PsProfileSnippet);

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

                // 确保 .zshrc 中有 fpath 设置（强制覆盖）
                string zshrc = Path.Combine(home, ".zshrc");
                string fpathLine = "fpath=(~/.zsh/completions $fpath)";

                if (!File.Exists(zshrc) && !HasCommandInPath("zsh"))
                    return false;  // zsh 未安装

                string content = File.Exists(zshrc) ? File.ReadAllText(zshrc) : "";
                content = StripSection(content, "# ClassIslandCLI completion", fpathLine);

                File.WriteAllText(zshrc,
                    content.TrimEnd() +
                    $"{Environment.NewLine}# ClassIslandCLI completion{Environment.NewLine}{fpathLine}{Environment.NewLine}");

                return true;
            }
            catch
            {
                return false;
            }
        }

        // ====================================================================
        // bash (跨平台)
        // ====================================================================

        private static bool TryInstallBash(string home)
        {
            try
            {
                string bashCompDir = Path.Combine(home, ".bash_completion.d");

                // 检查 bash 是否可用：有 .bashrc 或在 PATH 中
                if (!File.Exists(Path.Combine(home, ".bashrc")) &&
                    !File.Exists(Path.Combine(home, ".bash_profile")) &&
                    !HasCommandInPath("bash"))
                    return false;

                Directory.CreateDirectory(bashCompDir);
                File.WriteAllText(Path.Combine(bashCompDir, "classislandcli"), BashCompletion);

                // 在 .bashrc 中添加自动加载行（强制覆盖）
                string bashrc = Path.Combine(home, ".bashrc");
                string sourceLine = "for f in ~/.bash_completion.d/*; do [ -f \"$f\" ] && source \"$f\"; done";

                string content = File.Exists(bashrc) ? File.ReadAllText(bashrc) : "";
                content = StripSection(content, "# ClassIslandCLI completion", sourceLine);

                File.WriteAllText(bashrc,
                    content.TrimEnd() +
                    $"{Environment.NewLine}# ClassIslandCLI completion{Environment.NewLine}{sourceLine}{Environment.NewLine}");

                return true;
            }
            catch
            {
                return false;
            }
        }

        // ====================================================================
        // clink (仅 Windows)
        // ====================================================================

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private static bool TryInstallClink()
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string clinkDir = Path.Combine(localAppData, "clink");

                // 检查 clink 是否已初始化
                if (!Directory.Exists(clinkDir))
                {
                    if (!HasCommandInPath("clink.exe") && !HasCommandInPath("clink"))
                        return false;
                    Directory.CreateDirectory(clinkDir);
                }

                File.WriteAllText(Path.Combine(clinkDir, "classislandcli.lua"), ClinkCompletion);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ====================================================================
        // nushell (跨平台)
        // ====================================================================

        private static bool TryInstallNushell(string home)
        {
            try
            {
                string nuCompDir;

                if (IsWindows)
                {
                    string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    nuCompDir = Path.Combine(appData, "nushell", "completions");
                }
                else if (IsMacOS)
                {
                    nuCompDir = Path.Combine(home, "Library", "Application Support", "nushell", "completions");
                }
                else // Linux
                {
                    string xdgConfig = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")
                        ?? Path.Combine(home, ".config");
                    nuCompDir = Path.Combine(xdgConfig, "nushell", "completions");
                }

                // 检查 nushell 是否已初始化
                if (!Directory.Exists(Path.GetDirectoryName(nuCompDir)!))
                {
                    if (!HasCommandInPath("nu") && !HasCommandInPath("nu.exe"))
                        return false;
                }

                Directory.CreateDirectory(nuCompDir);

                // 写入两个文件以兼容大小写
                File.WriteAllText(Path.Combine(nuCompDir, "classislandcli.nu"), NuCompletion);
                File.WriteAllText(Path.Combine(nuCompDir, "ClassIslandCLI.nu"), NuCompletion);
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

        /// <summary>
        /// 从文本中移除匹配 startMarker 到 endMarker 之间的内容（含标记行本身）。
        /// 用于 profile 文件更新时清理旧版 ClassIslandCLI 段落。
        /// </summary>
        private static string StripSection(string text, string startMarker, string endMarker)
        {
            var startIdx = text.IndexOf(startMarker, StringComparison.Ordinal);
            if (startIdx < 0) return text;

            var endIdx = text.IndexOf(endMarker, startIdx + startMarker.Length, StringComparison.Ordinal);
            if (endIdx < 0) return text;

            // 延伸到 endMarker 行尾
            var lineEnd = text.IndexOf('\n', endIdx);
            if (lineEnd >= 0) endIdx = lineEnd;

            // 向前延伸到 startMarker 行首或前一个换行符
            var lineStart = text.LastIndexOf('\n', startIdx);
            if (lineStart < 0) lineStart = 0;

            return text.Remove(lineStart, endIdx - lineStart);
        }

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
