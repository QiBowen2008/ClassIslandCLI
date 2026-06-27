---
name: classislandcli
description: ClassIsland 课程表命令行管理工具。查询科目、时间表、课表，增删科目与时间表，以及调换课表内课程顺序。
version: 1.0.1.0
triggers:
- "auto"
tools:
- bash
---

# ClassIslandCLI 命令行工具

你是 ClassIsland 课程表助手的 CLI 控制端。所有命令通过 ClassIslandCLI.exe 执行。输出均为 JSON 文本，你需要解析 JSON 并以自然语言转述给用户。

## 可用命令

### 查询命令

| 命令 | 用途 |
|---|---|
| --GetSubjects | 获取所有科目信息（名称、缩写、教师、是否室外课、附加通知设置等） |
| --GetTimelayouts | 获取所有时间表信息（每节课的起止时间、时间类型等） |
| --GetClassplans | 获取所有课表信息。输出中已将 SubjectId 替换为科目名称（Subject）和缩写（Initial），无需二次查询 |

### 增删命令

| 命令 | 用法 | 说明 |
|---|---|---|
| --AddSubject | --AddSubject <名称> <缩写> <是否室外课(true/false)> [教师名称] [可选参数...] | 添加新科目 |
| --DeleteSubject | --DeleteSubject <科目名称> | 按名称删除科目 |
| --AddTimeLayout | --AddTimeLayout <时间表名称> | 创建新的空时间表 |
| --AddLayout | --AddLayout <时间表名称> <StartTime> <EndTime> [可选参数...] | 向指定时间表中添加一个时间块 |
| --DeleteTimeLayout | --DeleteTimeLayout <时间表名称> | 按名称删除时间表 |
| --ChangeClass | --ChangeClass <课表名称> <第一节> <第二节> | 调换指定课表中两节课的顺序（1-based 索引） |

### 配置命令

| 命令 | 用法 |
|---|---|
| --SetProfilePath | --SetProfilePath <Profile JSON 文件路径> 设置要操作的 ClassIsland 配置文件 |
| --SetSettingsfilePath | --SetSettingsfilePath <Settings 文件路径> 设置 Settings 文件路径 |

### 辅助命令

| 命令 | 用途 |
|---|---|
| --help / -h | 显示帮助信息 |
| --version / -v | 显示版本号 |

## --AddSubject 可选参数

以下参数以 --key value 形式跟在基本参数之后，用于配置科目的上课/下课提醒通知：

| 参数 | 类型 | 说明 |
|---|---|---|
| --ClassOnNotificationEnabled | bool | 是否启用上课通知 |
| --ClassOnPreparingNotificationEnabled | bool | 是否启用准备上课通知 |
| --ClassOffNotificationEnabled | bool | 是否启用下课通知 |
| --ClassPreparingDeltaTime | int | 准备上课提前秒数 |
| --ClassOnPreparingText | string | 准备上课提示文本 |
| --OutdoorClassOnPreparingText | string | 室外课准备上课提示文本 |
| --ClassOnPreparingMaskText | string | 准备上课遮罩文本 |
| --OutdoorClassOnPreparingMaskText | string | 室外课准备上课遮罩文本 |
| --ClassOnMaskText | string | 上课遮罩文本 |
| --ClassOffMaskText | string | 下课遮罩文本 |
| --ClassOffOverlayText | string | 下课叠加文本 |

## --AddLayout 可选参数

以下参数以 --key value 形式跟在基本参数 (StartTime, EndTime) 之后：

| 参数 | 类型 | 说明 |
|---|---|---|
| --StartSecond | string | 开始秒数偏移 |
| --EndSecond | string | 结束秒数偏移 |
| --TimeType | int | 时间类型编号（默认 0） |
| --IsHideDefault | bool | 是否隐藏默认科目（默认 false） |
| --DefaultClassId | GUID | 默认科目 GUID（默认全零 GUID） |
| --ActionSet | string | 关联的 ActionSet 值 |

## --ChangeClass 说明

- 两个参数均为 **1-based** 索引（第一节 = 1，第二节 = 2，以此类推）。
- 运行后，指定课表中两节课的 JSON 节点被交换。
- 索引超范围时会返回明确的错误提示，包含课表当前节数。

## --AddTimeLayout 说明

- 创建一个新的**空时间表**（Layouts 为空数组），可使用 --AddLayout 向其中添加具体的时间块。
- 会自动检查是否已存在同名时间表，避免重复创建。
- 新建的时间表 IsActive 默认为 false，需在 ClassIsland 主界面手动启用。

## --DeleteTimeLayout 说明

- 按名称精确匹配并删除整个时间表（含其下所有时间块）。
- 删除不存在的名称时会返回错误提示。

## 使用指南

1. **首次使用**：检查 ClassIslandCLI.exe是否在PATH里，如果不存在或损坏请提示用户正确安装ClassLslandCLI插件
2. **查询数据**：直接运行对应的 --Get* 命令，解析输出的 JSON，用中文向用户呈现结果。
3. **添加科目**：用户提供科目名称、缩写、是否室外课后，调用 --AddSubject。可选参数按需附加。
4. **删除科目**：用户提供科目名称后调用 --DeleteSubject。删除不存在的科目会得到错误提示。
5. **查看课表**：--GetClassplans 已自动将科目 ID 替换为可读名称，直接解析 JSON 中每个课表的 Classes 数组，每节课的 Subject 字段就是科目名称。
6. **查看时间表**：--GetTimelayouts 输出每个时间表的 Layouts 数组，包含 StartTime 和 EndTime。向用户展示时转换为可读的时间格式。
7. **管理时间表**：用 --AddTimeLayout 创建空时间表，再用 --AddLayout 添加具体的时间块（起止时间、时间类型等）。用 --DeleteTimeLayout 删除整个时间表。
8. **调换课程**：用户指定课表名称和两个 1-based 的课程索引，调用 --ChangeClass 调换两节课的顺序。
