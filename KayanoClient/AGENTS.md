# KayanoClient — Cursor Agent 指南

请使用中文写提案和回答。

TEngine 基于 HybridCLR + YooAsset + UniTask + Luban 构建。完整工作流见 `CLAUDE.md`。服务端 Fantasy 规范见 `fantasy-net` skill（`.cursor/skills/fantasy-net/`，与 KayanoServer 同源）。

## 强制工作流

1. **判断任务等级**（L1 简单 / L2 调用 / L3 功能 / L4 架构）
2. **L2+ 先读 `tengine-dev` skill**（`.cursor/skills/tengine-dev/references/`）
3. **再编码**；文档与代码冲突时以代码为准

## 编码红线

- 异步优先：IO 用 `UniTask`，禁止同步加载/Coroutine
- 模块访问：`GameModule.XXX`，不用 `ModuleSystem.GetModule<T>()`
- 资源释放：`LoadAssetAsync` 对应 `UnloadAsset`；GameObject 用 `LoadGameObjectAsync`
- 热更边界：`GameScripts/Main` 不热更，`GameScripts/HotFix/` 全部热更
- 事件解耦：模块间 `GameEvent`，UI 内部 `AddUIEvent`

## OpenSpec 变更管理

**前提**：Cursor 工作区根目录必须是本 `KayanoClient` 文件夹（不是 monorepo 根目录或 KayanoLesson）。

在输入框输入 `/` 后选择：

- 探索：`/opsx:explore`
- 提议：`/opsx:propose`
- 实施：`/opsx:apply`
- 归档：`/opsx:archive`
- 同步规格：`/opsx:sync`

Claude Code 使用相同命令名（`.claude/commands/opsx/`），两套工具可并存。

## Unity Editor 自动化

通过 Unity-MCP（`Window > MCP for Unity`）连接 Cursor。操作规范见 `tengine-dev/references/mcp-tools.md`。
