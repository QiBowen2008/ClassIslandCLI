# ClassIslandCLI PowerShell 补全
# 将此文件放置到 $PROFILE 中 dot-source 即可：
#   . "path/to/classislandcli.ps1"
# 或使用 --InstallCompletions 自动安装

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
        return $topCommands | Where-Object { $_ -like "$wordToComplete*" }
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
                    2 { return 'true', 'false' | Where-Object { $_ -like "$wordToComplete*" } }
                }
            }
            if ($wordToComplete -match '^--') {
                return $optionalParams | Where-Object { $_ -like "$wordToComplete*" }
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
                return $optionalParams | Where-Object { $_ -like "$wordToComplete*" }
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
