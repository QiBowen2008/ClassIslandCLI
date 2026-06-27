# ClassIslandCLI fish shell 补全
# 将此文件放置到 ~/.config/fish/completions/ 即可生效
# 或使用 --InstallCompletions 自动安装

complete -c classislandcli -s h -l help        -d "显示帮助"
complete -c classislandcli -s v -l version     -d "显示版本"
complete -c classislandcli -l SetProfilePath    -d "设置 Default.json 路径"          -x -a "(__fish_complete_path)"
complete -c classislandcli -l SetSettingsfilePath -d "设置 Settings.json 路径"       -x -a "(__fish_complete_path)"
complete -c classislandcli -l GetSubjects       -d "获取科目信息"
complete -c classislandcli -l GetTimelayouts    -d "获取时间表信息"
complete -c classislandcli -l GetClassplans     -d "获取课表信息（含科目名称）"
complete -c classislandcli -l AddSubject        -d "添加新科目"                      -x
complete -c classislandcli -l DeleteSubject     -d "删除科目"                        -x
complete -c classislandcli -l AddTimeLayout     -d "添加新时间表"                    -x
complete -c classislandcli -l AddLayout         -d "向时间表添加时间段"              -x
complete -c classislandcli -l DeleteTimeLayout  -d "删除时间表"                      -x
complete -c classislandcli -l PExchangeClass    -d "永久调课：直接交换课表中两节课"  -x
complete -c classislandcli -l TExchangeClass    -d "临时调课：创建叠加层副本交换课程"-x
complete -c classislandcli -l InstallCompletions -d "安装 Shell 补全"

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
    complete -c classislandcli -l "$opt" -d "$desc" -x -n "__fish_seen_subcommand_from --AddLayout; or __fish_seen_subcommand_from --AddSubject"
end
