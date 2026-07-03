# KayanoServer — Cursor Agent 指南

请使用中文写提案和回答。

KayanoServer 基于 **Fantasy.Net**（Entity / Hotfix / Main 三层）构建。Fantasy 开发规范见 `fantasy-net` skill。

## 强制工作流

1. **判断任务等级**（L1 简单 / L2 调用 / L3 功能 / L4 架构）
2. **L2+ 先读 `fantasy-net` skill**（`.cursor/skills/fantasy-net/references/`）
3. **再编码**；文档与代码冲突时以代码为准

## 编码红线

- 异步：用 `FTask`，不用 `Task`
- 三层：`Entity` 数据与定义 · `Hotfix` Handler/System · `Main` 启动入口
- 注册：Source Generator 自动注册，不手改 `.g.cs`
- 日志：`Log.Debug/Info/Error()`；业务错误用 `response.ErrorCode`，不抛异常
- 解耦：模块间用 `Event` / `EventAwaiter`，分清与 Address / Roaming / SphereEvent 边界

## 项目结构

```
KayanoServer/
├── Entity/     # Entity、Component、协议定义
├── Hotfix/     # Handler、System、业务逻辑（热重载）
└── Main/       # Program.cs、Fantasy.config、启动
```

## Cursor 工作区

**服务端开发时**，Cursor 工作区根目录建议为 `KayanoServer`（或包含本目录的 monorepo 根，并 @ 引用本 AGENTS.md）。

Skill 权威来源：`E:\Kayano Project\Fantasy-2026.0.1023\Skills\fantasy-net`（已安装到 `.cursor/skills/fantasy-net`）。
