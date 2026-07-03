# KayanoClient 变更日志

## 2026-06-21 — ActionModule Phase 1 骨架

| 项 | 内容 |
|----|------|
| **时间** | 2026-06-21 |
| **范围** | `Assets/GameScripts/HotFix/GameLogic/Module/ActionModule/` |
| **原因** | Grill 定案 Phase 1：单机 3 角编队 + Pipeline 骨架 |

### 新增目录

- `Squad/` — `SquadEntityDriver`, `RoleDriver`
- `Runtime/` — `SquadRuntimeData`, `RoleSlotRuntimeData`, `EActionEntityMode`, `ELocoStateType`
- `Pipeline/` — `ActionPipeline`, 8 个 Stage
- `Sync/` — `IActionSyncBridge`, `ActionSyncBridge`, `SquadInterpolator`
- `Locomotion/` — `ILocomotionDriver`, `LocomotionDriver`
- `Combat/` — `CombatActionExecutor`, `ActionConfig`, `ActionCatalogSetting`
- `Presentation/` — `ActionPresentationDriver`

### 行为

- **旧**：无 ActionModule 运行时骨架。
- **新**：`SquadEntityDriver` 唯一 Update；`Apply → Pipeline → Collect`；LocalOwner 全 Stage，RemoteProxy 裁剪路径占位。

### 风险

- Phase 1 未接 InputSystem / Timeline ActionCtrl 驱动；需在 Prefab 上手动绑定 3× `RoleDriver`。
- `ActionSyncBridge` 为空实现，联机需 Phase 2。

### 测试点

1. 场景挂 `SquadEntityDriver`，绑定 3 个带 `CharacterController` 的子物体。
2. 运行时调用 `SetMoveAxis` / `SwitchToNextMember`，验证 Front 移动与 ShowState 切换。
3. Unity 编译 GameLogic 程序集无报错。

---

## 2026-06-21 — ActionModule 注释补充

| 项 | 内容 |
|----|------|
| **范围** | `Module/ActionModule/` 全部 .cs |
| **原因** | 为英文术语（Loco、Combat、Pipeline 等）补充中文对照注释 |

### 变更

- 新增 `Runtime/ActionModuleGlossary.cs` 集中术语表。
- 各类/字段/Stage 增加 XML 注释与 `【术语】` 中英对照。

---

## 2026-06-21 — InputSystem + ActionCtrl 接入

| 项 | 内容 |
|----|------|
| **范围** | `SquadPlayerInput`, `SourceStage`, `CombatActionExecutor`, `SquadEntityDriver` |
| **原因** | Phase 1：InputSystem 驱动 MoveAxis/Combat 输入；ActionCtrl 并入 CombatExecute |

### 行为

- **SourceStage**：`SquadPlayerInput` 读 `InputActionAsset`（Player/Move/Look/Attack/Jump/Next/Previous）→ `SquadRuntimeData`。
- **CombatExecuteStage**：`ForwardInput` 转发 `InputCommand` 到前台 `ActionCtrl`（早于 ActionCtrl.Update，ExecutionOrder -300）。
- **LateUpdate**：`SyncFromActionCtrl` 回写 `CombatActionId`、`LockLocomotion`。
- **换人**：Next/Previous → `SwitchToNextMember` / `SetFrontRoleIndex`。

### 配置

Prefab 上挂 `SquadPlayerInput`，绑定 `InputSystem_Actions.inputactions` 资产。

### 测试点

1. Move 时 Loco 位移 + ActionCtrl 收到 Move Press。
2. Attack/Jump 触发 ActionCtrl 连段/闪避 Transition。
3. Next/Previous 切换 Front 槽与 ShowState。
4. SpecialAttack 触发 ActionCtrl SpecialAttack Transition。

---

## 2026-06-21 — 输入缓冲窗口与长按时间

| 项 | 内容 |
|----|------|
| **范围** | `SquadPlayerInput`, `SquadRuntimeData`, `CombatActionExecutor` |
| **原因** | 输入缓冲窗口提升连段手感；HoldTimes 供长按招式/蓄力扩展 |

### 行为

- Down/Up 写入 `InputBuffer` 并带 `Time.timeAsDouble`；超 `inputBufferWindow`（默认 0.25s）丢弃。
- `HoldTimes` 累计 Move/Attack/SpecialAttack/Dodge 按住时长，松开归零。
- `CombatActionExecutor` 每帧先 FIFO 重放缓冲，再发本帧 Press/Down。

### 配置

`SquadPlayerInput.inputBufferWindow` 可在 Inspector 调整（建议 0.2~0.35s）。

---

## 2026-06-21 — IntentStage 意图域与 Handler

| 项 | 内容 |
|----|------|
| **范围** | `Intent/`, `Runtime/ELocoIntentType`, `ECombatIntentType`, `SquadIntentData`, `IntentStage`, `LocoDispatchStage` |
| **原因** | Input → Intent 分层；Dispatch 消费 Intent，不再在 IntentStage 直接写 LocoState |

### 行为

- **IntentStage**：每帧 Clear Intent 域 → 跑 Loco/Combat Handler 链 → 写入 `SquadRuntimeData.Intent`。
- **Handler 默认集**：Move、Dodge（Loco）；Attack、SpecialAttack、Switch（Combat）。
- **IntentInputQueries**：FrameInput + InputBuffer 窗口内预输入均算有效 Intent。
- **LocoDispatchStage**：`EffectiveLocoIntents` → FSM（Dodge > Move > Idle）；`LockLocomotion` 时保持当前状态。
- **SourceStage**：`ClearFrameIntent` 重命名为 `ClearFrameInput`（只清 Input 域）。

### 测试点

1. 只按 Move → `Intent.LocoIntents` 含 Move，`LocoState = Move`。
2. 提前按 Attack（缓冲窗内）→ `Intent.CombatIntents` 含 Attack（即使本帧未切招）。
3. LockLocomotion 时 LocoDispatch 不改 LocoState。

---

## 2026-06-21 — ArbitrateStage 仲裁链

| 项 | 内容 |
|----|------|
| **范围** | `Arbitrate/`, `RoleSlotStatusData`, `ArbitrateStage` |
| **原因** | Intent → Block 掩码 → EffectiveIntents；Dispatch 只消费允许后的意图 |

### 行为

- **RoleSlotStatusData**：Hp、Stamina、IsAlive、StunRemaining（挂 `RoleSlotRuntimeData.Status`）。
- **默认 Arbiter 链**：Health → Stun → Stamina → CombatLock → CombatPriority。
- **HealthArbiter**：死亡阻断全部 Loco/Combat Intent。
- **StunArbiter**：硬直期间阻断全部 Intent，并递减 `StunRemaining`。
- **StaminaArbiter**：体力 &lt; 25 时阻断 Dodge（`DodgeStaminaCost`）。
- **CombatLockArbiter**：`LockLocomotion` 时阻断 Move/Dodge Loco Intent。
- **CombatPriorityArbiter**：出招中（`CombatActionId &gt; 0`）阻断换人 Intent。
- **CombatDispatchStage**：消费 `EffectiveCombatIntents` 执行换人（从 `SquadPlayerInput` 迁出）。
- **SquadFrontSwitchUtility**：换人 ShowState 逻辑复用。

### 测试点

1. `Status.IsAlive = false` → 无 Effective Intent，LocoState 不变。
2. `Status.StunRemaining = 1f` → 硬直期间无法 Attack/Move。
3. `Status.Stamina = 0` + 按 Jump → Dodge Intent 被阻断。
4. 出招中按 Next → `SwitchNext` 被阻断。

---

## 2026-06-21 — CombatDispatch Transition 时间窗 + Runtime 分域

| 项 | 内容 |
|----|------|
| **范围** | `CombatTransitionDispatch`, `Runtime/Combat/*`, `ActionCtrl.CollectActiveCommandTransitions`, `CombatDispatchStage`, `CombatActionExecutor` |
| **原因** | Dispatch 维护 Active/Allowed Transition；Execute 只转发 Allowed 指令 |

### Runtime 目录（按 Pipeline 阶段）

| 子目录 | 内容 |
|--------|------|
| `Blackboard/` | SquadRuntimeData, RoleSlotRuntimeData |
| `Input/` | SquadFrameInput, SquadHoldTimes, SquadInputBufferItem |
| `Intent/` | SquadIntentData, ELocoIntentType, ECombatIntentType |
| `Status/` | RoleSlotStatusData |
| `Loco/` | ELocoStateType |
| `Combat/` | RoleSlotCombatDispatchData, CombatAllowedTransition, CombatTransitionKey |
| `Core/` | EActionEntityMode, ActionModuleGlossary |

### 行为

- **ActionCtrl.CollectActiveCommandTransitions**：全局 `commandTransitions` + 运行中 `CommandTransitionNotifyState` 时间窗。
- **CombatDispatch**：写入 `ActiveTransitions` → Intent+Input 匹配 → `AllowedTransitions`。
- **CombatExecute**：`IsCommandAllowed` 门禁后再 `InputCommand`（含 Squad InputBuffer 重放）。
- **PermissiveCombatInput**：未出招时不做 Transition 门禁（Idle 起手）。

### 测试点

1. 攻击 Cancel 窗外按连段 → Execute 不转发 BaseAttack。
2. NotifyState 窗内按 → `AllowedTransitions` 含对应 Command+Phase。
3. Idle 状态按 Attack → 正常起手（Permissive）。

---

## 2026-06-21 — Loco 手感包

| 项 | 内容 |
|----|------|
| **范围** | `LocomotionDriver`, `LocomotionCameraUtility`, `SquadLocoConstants`, `LocoDispatchStage`, `RoleSlotRuntimeData`, `SquadEntityDriver` |
| **原因** | Phase 1 可玩手感：相机相对移动、朝向、Loco 闪避、体力消耗/回复 |

### 行为

- **相机相对移动**：`MoveAxis` 相对 `locomotionCamera`（或 `Camera.main`）Yaw 平面投影。
- **朝向**：走跑/闪避时 `RotateTowards` 面向移动方向。
- **Loco 闪避**：`DodgeDown`/缓冲触发 → `DodgeTimeRemaining` 内水平位移；无输入则沿相机前方。
- **体力**：闪避开始扣 `SquadLocoConstants.DodgeStaminaCost`（25）；前台自然回复 15/s。
- **LocoDispatch**：闪避片内保持 Dodge；新闪避需 Down/缓冲边沿，避免按住连跳。

### 配置

`SquadEntityDriver.locomotionCamera` 可绑主相机 Transform；留空回退 `Camera.main`。

### 测试点

1. WASD 相对相机方向移动，角色朝向移动方向。
2. 按 Jump 闪避 → 位移 + 体力 -25；体力不足时 Arbitrate 阻断。
3. 按住 Jump 不连段闪避（0.35s 内只触发一次 Loco Dodge）。

---

## 2026-06-21 — Root Motion 双轨位移

| 项 | 内容 |
|----|------|
| **范围** | `RoleDriver.OnAnimatorMove`, `CombatLocomotionRules`, `CombatActionExecutor`, `LocomotionDriver` |
| **原因** | LockLocomotion 招式由动画 Root Motion 驱动 cc；走跑仍走 Loco 轨 |

### 行为

- **RoleDriver**：`applyRootMotion = false`；`OnAnimatorMove` 在 Combat 招式片内 `cc.Move(deltaPosition)` + 旋转。
- **判定**：`CombatLocomotionRules.ShouldLockLocomotion`（与 LateUpdate 回写 LockLocomotion 同源，同帧可生效）。
- **Loco 轨**：`LockLocomotion` 时 LocomotionDriver 只补重力。

### 前提

动画 Clip 须烘焙 **Root Motion**，`Animator.deltaPosition` 才有位移。

### 测试点

1. 有 Root Motion 的普攻/闪避随动画位移。
2. Idle/Move Loop 仍走相机相对 Loco。
3. 出招中 MoveAxis 不驱动平面移动。

---

## 2026-06-21 — ActionCtrl 内聚 Presentation（第一步）

| 项 | 内容 |
|----|------|
| **范围** | `CombatTimelinePresenter`, `ActionPresentationDriver`, `CombatActionExecutor`, `CombatExecuteStage`, `RoleSlotCombatPresentationData` |
| **原因** | ActionCtrl 逐步退为 Timeline/Notify 数据源；Presentation 统一 Apply/Sync |

### 行为

- **CombatExecute**：`CollectPresentationRequests` → 写 `CombatPresentation.PendingCommands`（不碰 ActionCtrl）。
- **Presentation**：`CombatTimelinePresenter.ApplyCombatInput` → InputBuffer 重放 + InputCommand。
- **LateUpdate**：`ActionPresentationDriver.SyncFromActionCtrl` → 回写 `CombatActionId` / `LockLocomotion`。
- **Catalog 起招**：`TryPlayCombatAction` 经 Presentation → `ActionCtrl.PlayAction`。

### 下一步（未做）

- Animator 播放迁出 ActionCtrl，由 Presentation 读黑板驱 Animancer。
- Notify 事件先进 Events 域，Presentation 消费 VFX/SFX。

---

## 2026-06-21 — Phase 2：Animancer + Events 域

| 项 | 内容 |
|----|------|
| **范围** | `ActionCtrl`, `CombatAnimancerPresenter`, `CombatEventBridge`, `CombatEventConsumer`, `RoleSlotCombatEventsData` |
| **原因** | Animator 迁出 ActionCtrl；Notify 先进 Events 域；ActionCtrl 退化为 Timeline/Notify 数据源 |

### 行为

- **UseExternalAnimator**：Role 挂 `AnimancerComponent` 时自动启用；ActionCtrl 不再 `CrossFadeInFixedTime`。
- **CombatAnimancerPresenter**：Presentation 读 ActionCtrl 解包 clip → `Animancer.Play`；LateUpdate 对齐 `Time/Speed`。
- **CombatEvents 域**：`CombatEventBridge` 路由 Particle/SFX/CameraShake → `RoleSlotCombatEventsData`。
- **CombatEventConsumer**：LateUpdate 消费 VFX/SFX/Camera，清空 Pending。
- **ActionCtrl**：`EventBridge` + `DispatchNotify*`；逻辑时钟与 Notify 调度保留。

### Prefab

- 每个 Role 加 `AnimancerComponent`（Animator 字段指向同物体 Animator；Controller 留空）。
- 无 Animancer 时回退 ActionCtrl 内置 Animator 播放（`UseExternalAnimator=false`）。

---

## 2026-06-21 — 统一 Action 通道：删除 Loco Pipeline，Combat 重命名为 Action

| 项 | 内容 |
|----|------|
| **范围** | `ActionModule/` 全模块 |
| **原因** | 与 Kirara/ARPGdemo 对齐：走跑/闪避/攻击统一经 ActionCtrl Timeline + Root Motion，不再维护 Loco 代码位移双轨 |

### 删除

- `LocoDispatchStage` / `LocoExecuteStage`
- `Locomotion/`（`LocomotionDriver`, `ILocomotionDriver`, `LocomotionCameraUtility`）
- `Runtime/Loco/`（`ELocoStateType`, `SquadLocoConstants`）
- `ELocoIntentType` / `ECombatIntentType` → 合并为 `EActionIntentType`

### 重命名

- `Combat*` → `Action*`（Stage、Executor、Presenter、Events、Dispatch 域）
- `Combat/` → `Action/`；`Runtime/Combat/` → `Runtime/Action/`

### 新 Pipeline（6 Stage）

```
Source → Intent → Arbitrate → ActionDispatch → ActionExecute → Presentation
```

- **ActionExecute** 额外调用 `RolePhysics`（重力 + 体力回复，原 LocomotionDriver 职责）
- **RoleDriver.OnAnimatorMove**：ActionCtrl 播放中一律应用 Root Motion（Kirara 风格）

### 黑板

- `RoleSlotRuntimeData` 移除 `LocoState` / `DodgeTimeRemaining` / `DodgeDirection`
- Intent 域：`ActionIntents` / `BlockedActionIntents` / `EffectiveActionIntents`

---

## 2026-06-21 — ActionModule 目录整理

| 项 | 内容 |
|----|------|
| **范围** | `ActionModule/Action/`、`ActionModule/Source/` |
| **原因** | 调度层与执行层分文件夹；Source 输入采集从 Squad 拆出 |

### 目录

- `Action/Dispatch/` — `ActionTransitionDispatch`, `ActionLocomotionRules`
- `Action/Execute/` — `ActionExecutor`, `ActionConfig`, `ActionCatalogSetting`
- `Source/` — `ISquadInputCollector`, `SquadPlayerInput`, `SourceStage`

命名空间不变（`GameLogic.ActionModule`），Prefab 上 `SquadPlayerInput` 组件引用不受影响。

---

## 2026-06-21 — 删 Loco 后一致性修复

| 项 | 内容 |
|----|------|
| **范围** | Execute / Dispatch / Arbitrate / Pipeline / Presentation |
| **原因** | Move 绕过 Intent；BlockMoveIntent 1 帧滞后；闪避不扣体力；遗留 Loco 命名 |

### 修复

- **Move 统一门禁**：`TryEnqueueMove` 检查 `EffectiveActionIntents.Move`；走跑 Loop 不经过 Transition 门禁；移除 `MovePress` 旁路
- **BlockMoveIntent**：`LockLocomotion` 重命名；`ActionIntentLockRules` 替代 `ActionLocomotionRules`；Pipeline 帧初 `SyncBlockMoveIntent`
- **闪避体力**：`Dodge Down` 入队成功时扣 `DodgeStaminaCost`
- **Dispatch**：`IntentCoversCommand(Move)` 需 Move Intent；`IsCommandPhaseRequested(Move)` 需 MoveAxis

---

## 2026-06-21 — Runtime/Action 拆 Dispatch / Execute

| 项 | 内容 |
|----|------|
| **范围** | `Runtime/Action/` |
| **原因** | 与 `Stages/Dispatch`、`Stages/Execute` 黑板域对齐 |

### 目录

- `Runtime/Action/Dispatch/` — `RoleSlotActionDispatchData`, `ActionAllowedTransition`, `ActionTransitionKey`
- `Runtime/Action/Execute/` — `RoleSlotActionPresentationData`, `ActionPresentationCommand`, `SquadActionConstants`

---

## 2026-06-21 — KayanoTimeline 迁移（Phase A–D）

| 项 | 内容 |
|----|------|
| **范围** | `KayanoTimeline/`、`ActionModule/Runtime/ActionGraph/`、`TimelineAction/`（Legacy）、`TimelineActionCustom/` |
| **原因** | KayanoTimeline 负责编辑/解包；ActionModule 拥有全部运行时；移除 `ActionCtrl` MonoBehaviour |

### Phase A — KayanoTimeline 骨架

- 新增 `Module/KayanoTimeline/`：`Authoring`（`KayanoActionTimelineSO`、`KayanoActionCatalogSO`、Notify/Transition）+ `Runtime`（`KayanoActionUnpacker`、`IActionGraphHost`、`Deque`）
- `KayanoActionCatalogSO` 支持 `legacyCatalog`（`KiraraActionListSO`）过渡旧资产

### Phase B — RoleActionGraphPlayer

- 新增 `RoleActionGraphPlayer`（非 MonoBehaviour），由 `ActionTimelinePresenter.ApplyActionInput` 驱动 `Tick`
- `RoleDriver` 移除 `ActionCtrl`；新增 `KayanoActionCatalogSO` + `GraphPlayer` 属性

### Phase C — Notify 迁移

- `TimelineActionCustom/*` 改为 `IActionGraphHost`；`ActionEventBridge` 路由 VFX/SFX/Shake
- ARPG 专用 Notify（Box/RoleCtrl/NetFn）暂存根实现，待 ActionModule Events 扩展

### Phase D — 清理

- 删除 `TimelineAction/Runtime/ActionCtrl.cs` 及重复运行时类型
- 保留 `TimelineAction/Runtime/` 仅 Legacy 资产类型（`KiraraActionSO`、`ActionArgs`、TransitionInfo 等）
- 注释/术语表 `ActionCtrl` → `GraphPlayer`

### 帧序

```
SyncBlockMoveIntent → Source → Intent → Arbitrate → Dispatch → Execute
→ Presentation (InputCommand + GraphPlayer.Tick) → LateUpdate Sync
```

### Prefab 迁移

1. 移除 `ActionCtrl` 组件
2. `RoleDriver` 绑定 `KayanoActionCatalogSO`（可设 `legacyCatalog` 复用旧 Kirara 列表）
3. Unity 编译 GameLogic 无报错后验证走跑/攻击/闪避

---

## 2026-06-21 — 删除 TimelineAction，统一 KayanoTimeline

| 项 | 内容 |
|----|------|
| **范围** | 删除 `Module/TimelineAction/`；清理 KayanoTimeline Legacy 桥接；迁移 Editor |
| **原因** | 运行时与资产编辑统一走 KayanoTimeline，不再维护 Kirara 双轨 |

### 删除

- `Module/TimelineAction/` 全部（KiraraActionSO、Legacy TransitionInfo、CommandTransitionTrack 等）
- `KayanoActionCatalogSO.legacyCatalog` / `KayanoActionRuntimeEntry.LegacySource` 及转换代码
- `TimelineActionCustom/ActionName.cs`（迁至 KayanoTimeline）

### 保留 / 新增（KayanoTimeline）

- `Authoring/ActionName.cs` — 动作名常量
- `Authoring/BuiltIn/CommandTransitionNotifyState/CommandTransitionTrack.cs`
- `KayanoActionRuntimeEntry` 仅读 `KayanoActionTimelineSO`

### Editor 迁移

- `Kirara/动作列表` → `Kayano/动作列表`
- `KiraraActionSO` → `KayanoActionTimelineSO`；`KiraraActionListSO` → `KayanoActionCatalogSO`
- 绑定 `RoleDriver.ActionCatalog` 替代已删除的 `ActionCtrl`
- Inspector：`KayanoActionTimelineSOInspector`

### 资产迁移注意

- 旧 `KiraraActionListSO` / `KiraraActionSO` 资产需在 Unity 中手动转为 `KayanoActionCatalogSO` + `KayanoActionTimelineSO`（或重新创建 Catalog 并拖入 Timeline）
- Prefab `RoleDriver` 绑定新的 `KayanoActionCatalogSO`

---

## ActionModule 脱离 KayanoTimeline（2026-06-21）

| 项 | 内容 |
|----|------|
| **目标** | ActionModule 不再依赖 `Kayano.TimelineAction` / KayanoTimeline；先跑通 TEngine 配表管线，动画/特效后续接入 |
| **核心替换** | `RoleActionGraphPlayer` → `RoleActionRuntime`；`KayanoActionCatalogSO` → `ActionCatalogSetting` |

### 新增（ActionModule 自有类型）

- `Runtime/Core/`：`EActionCommand`、`EActionCommandPhase`、`ERoleShowState`、`Deque`、`ActionCommandTransition`、`ActiveCommandTransition`
- `Runtime/Action/RoleActionRuntime.cs` — 查表、Transition、Tick（无 Timeline/Notify）
- `Stages/Presentation/ActionRuntimePresenter.cs`

### 扩展 TEngine 配表

- `ActionConfig`：`ActionType`、`IsLoop`、`CommandTransitions`、`FinishTransition`、`InheritActionName`
- `ActionCatalogSetting.TryGetByName`

### 删除

- `RoleActionGraphPlayer.cs`
- `ActionTimelinePresenter.cs`、`ActionAnimancerPresenter.cs`、`ActionEventBridge.cs`

### Prefab / 场景注意

- `RoleDriver` Inspector 改绑 `ActionCatalogSetting`（`TEngine/Action/Action Catalog Setting`）
- 各 `ActionConfig` 需手工配置 Transition 列表（原 Timeline 解包内容不再自动注入）

---

## KayanoAction Authoring + Bake v0（2026-06-21）

| 项 | 内容 |
|----|------|
| **Authoring** | `Assets/TEngine/KayanoAction/Authoring/` 程序集 `KayanoAction.Authoring`（Editor-only，不进 Hotfix / 不进真机包） |
| **Editor Bake** | `Assets/TEngine/KayanoAction/Editor/` 程序集 `KayanoAction.Editor` |
| **Runtime** | `ActionBakedNotifyScheduler` + `ConsumeStaminaNotifyHandler`（Hotfix GameLogic） |

### 使用

1. 创建 `Kayano/Action Timeline`、`Kayano/Action Catalog`（Authoring）
2. Timeline 上添加 Marker：`ConsumeStaminaNotify`
3. `ActionCatalogSetting` 中已有同名 `ActionConfig`
4. 同时选中 Timeline + ActionCatalogSetting → `Kayano/Bake/烘焙选中 Timeline → ActionConfig`
5. Play 时 `RoleActionRuntime` 按 `BakedNotifies` 在对应时间扣体力

### Execute 层

- 已移除 Dodge Down 扣体力；副作用仅在 Baked Notify Handler 触发

