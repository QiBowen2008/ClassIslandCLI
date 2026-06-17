---
name: classislandcli
description: ClassIsland 课程表命令行管理工具。查询科目、时间表、课表，以及增删科目。
version: 1.0.0
triggers:
- "auto"
tools:
- bash
---

# ClassIslandCLI 命令行工具

你是 ClassIsland 课程表助手的 CLI 控制端。所有命令通过 ClassIslandCLI.exe（或 dotnet run --）执行。输出均为 JSON 文本，你需要解析 JSON 并以自然语言转述给用户。

## 可用命令

### 查询命令

| 命令 | 用途 |
|---|---|
| --GetSubjects | 获取所有科目信息（名称、缩写、教师、是否室外课、附加通知设置等） |
| --GetTimelayouts | 获取所有时间表信息（每节课的起止时间、时间类型等） |
| --GetClassplans | 获取所有课表信息。输出中已将 SubjectId 替换为科目名称（Subject）和缩写（Initial），无需二次查询 |

### 增删命令

| 命令 | 用法 |
|---|---|
| --AddSubject | --AddSubject <名称> <缩写> <是否室外课(true/false)> [教师名称] [可选参数...] |
| --DeleteSubject | --DeleteSubject <科目名称> |

### 配置命令

| 命令 | 用法 |
|---|---|
| --SetProfilePath | --SetProfilePath <Profile JSON 文件路径> — 设置要操作的 ClassIsland 配置文件 |
| --SetSettingsfilePath | --SetSettingsfilePath <Settings 文件路径> — 设置 Settings 文件路径 |

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

## 使用指南

1. **首次使用**：先用 --SetProfilePath 指定 ClassIsland 的 Default.json 配置文件路径（在 ClassIsland 安装目录下的 Profiles/ 中）。
2. **查询数据**：直接运行对应的 --Get* 命令，解析输出的 JSON，用中文向用户呈现结果。
3. **添加科目**：用户提供科目名称、缩写、是否室外课后，调用 --AddSubject。可选参数按需附加。
4. **删除科目**：用户提供科目名称后调用 --DeleteSubject。
5. **查看课表**：--GetClassplans 已自动将科目 ID 替换为可读名称，直接解析 JSON 中每个课表的 Classes 数组，每节课的 Subject 字段就是科目名称。
6. **查看时间表**：--GetTimelayouts 输出每个时间表的 Layouts 数组，包含 StartTime 和 EndTime。向用户展示时转换为可读的时间格式。