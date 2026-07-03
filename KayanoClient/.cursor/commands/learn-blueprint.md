---
name: /learn:blueprint
id: unity-learning-blueprint
category: Learning
description: 按通用学习模板 The Blueprint 生成 Unity 高质量中文学习资料（新建 Markdown）
---

按【通用学习模板 The Blueprint】生成 Unity 高质量中文学习资料。

**本命令是唯一启用方式**——不因普通对话关键词自动执行。

---

**Input**：`/learn:blueprint` 后面的参数为学习主题（如 `URP 渲染管线`），可选第二参数为输出路径（如 `docs/`）。

示例：
- `/learn:blueprint URP 渲染管线`
- `/learn:blueprint Shader Graph docs/`

---

**Steps**

1. **Read skill 与模板**（必须，按顺序）：
   - `.agents/skills/unity-learning-blueprint/SKILL.md`
   - `.agents/skills/unity-learning-blueprint/blueprint-template.md`

2. **解析输入**：
   - 无参数 → 用 AskQuestion 询问主题、受众、输出路径
   - 有主题无路径 → 默认 `docs/{主题}高质量学习资料.md`

3. **生成文档**（严格执行 skill 内规则）：
   - 资深 Unity 教育专家口吻，中文
   - **新建**独立 Markdown，不合并、不覆盖已有文档（除非用户明确要求）
   - 五章节齐全 + 对比表 + 知识小结表
   - 文首/文尾元信息块按 skill 格式

4. **输出结果**：
   - 返回新建文件路径
   - 用 3～5 条 bullet 摘要各章内容

**Guardrails**

- 不注册、不依赖其他自动触发；仅在本 `/learn:blueprint` 命令下执行
- 不复用旧稿凑章节；每次按模板从零填充
- Unity 主题需区分 Editor vs 真机/IL2CPP（若适用）
- 代码示例须标注文件路径
