using System.Collections.Generic;
using GameConfig.Main;
using KayanoAction.Runtime;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    #region 数据结构定义

    /// <summary>
    /// 可执行意图（由输入层生成，待仲裁层消费）
    /// </summary>
    public struct ExecutableIntent
    {
        public int InstanceId;
        public int ActionId;
        public string ActionName;
        public EActionType LogicType;
        public int Priority;
        public EInputPhase Phase;
        public float HoldTime;
        public Vector2 Direction;
        public EChainDirection ChainDir;

        // 队伍切换目标成员
        public int memberInstanceId;
        public string memberActionName;
        public EActionType memberEActionType;

        // 资源消耗字段
        public float EnergyCost;
        public float DecibelCost;
        public float ChainGaugeCost;
        public float DodgeStaminaCost;
    }

    /// <summary>
    /// 动作时间线上下文（用于定位动作播放环境）
    /// </summary>
    public struct ActionTimelineContext
    {
        public int InstanceId { get; set; }
        public int ConfigId { get; set; }
        public Transform Owner { get; set; }
        public int ActionId { get; set; }
        public bool IsValid => ConfigId > 0 && Owner != null;
    }

    /// <summary>
    /// 运行中的 NotifyState 上下文
    /// </summary>
    public struct RunningNotifyStateContext
    {
        public NotifyStateSO State;
        public int StateInstanceId;
    }

    /// <summary>
    /// 运行中的指令窗口上下文
    /// </summary>
    public struct RunningIntentContext
    {
        public CommandTransitionInfo Info;
        public int StateInstanceId;
    }

    #endregion

    /// <summary>
    /// 动作状态模型 — 组合三个子模型（数据、运行时、队列），提供统一访问接口
    /// 
    /// 核心区分：
    ///   - "表现动作" = TimelineActionSO（实际播放的资产，按动作名索引）
    ///   - "逻辑配置" = ChActionConfig（配置表数据，按 EActionType 索引）
    /// </summary>
    public class ChActionModel
    {
        #region 私有字段 / 子模型引用

        public ChActionDataModel Data { get; }
        public ChActionRuntimeModel Runtime { get; }
        public ChActionQueueModel Queue { get; }

        #endregion

        #region 构造与初始化

        public ChActionModel(List<TimelineActionSO> actions, string defaultActionName)
        {
            Data = new ChActionDataModel(actions);
            Runtime = new ChActionRuntimeModel();
            Queue = new ChActionQueueModel();

            if (Data.TryGetAction(defaultActionName, out var defaultAction))
            {
                Runtime.SetCurrentAction(defaultAction);
            }
            else if (actions.Count > 0)
            {
                Runtime.SetCurrentAction(actions[0]);
                Log.Warning($"已自动使用第一个动作 '{actions[0].actionName}' 作为默认");
            }
            else
            {
                Log.Error("动作列表为空，无法初始化 ChActionModel");
            }
        }

        #endregion

        #region 表现动作查询（TimelineActionSO，按动作名索引）

        /// <summary>
        /// 切换当前表现动作（按动作名称）
        /// </summary>
        public bool SwitchTo(string actionName)
        {
            if (string.IsNullOrEmpty(actionName))
            {
                Log.Warning("SwitchTo 失败：动作名称为空");
                return false;
            }

            if (!Data.TryGetAction(actionName, out var newAction))
            {
                Log.Warning($"SwitchTo 失败：表现动作 '{actionName}' 不存在");
                return false;
            }

            if (Runtime.CurrentAction == newAction) return false;
            Runtime.SetCurrentAction(newAction);
            return true;
        }

        /// <summary>
        /// 获取表现动作资产（按动作名）
        /// </summary>
        public bool TryGetAction(string actionName, out TimelineActionSO action)
            => Data.TryGetAction(actionName, out action);

        /// <summary>
        /// 获取表现动作 ID（按动作名）
        /// </summary>
        public bool TryGetActionId(string actionName, out int actionId)
            => Data.TryGetActionId(actionName, out actionId);

        /// <summary>
        /// 检查是否存在指定名称的表现动作
        /// </summary>
        public bool HasAction(string actionName)
            => Data.HasAction(actionName);

        /// <summary>
        /// 获取所有可用表现动作名称列表
        /// </summary>
        public List<string> GetAvailableActionNames()
            => Data.GetAvailableActionNames();

        #endregion

        #region 逻辑配置查询（ChActionConfig，按 EActionType 索引）

        /// <summary>
        /// 根据逻辑动作类型获取该类型下的所有配置列表（只读）
        /// </summary>
        public IReadOnlyList<ChActionConfig> GetConfigsByType(EActionType type)
            => Data.GetConfigsByType(type);

        /// <summary>
        /// 根据逻辑动作类型获取该类型下的第一个配置
        /// </summary>
        public ChActionConfig GetFirstConfigByType(EActionType type)
            => Data.GetFirstConfigByType(type);

        #endregion

        #region 运行时状态管理（代理 RuntimeModel）

        /// <summary>
        /// 检查是否存在指定指令的运行中窗口
        /// </summary>
        public bool HasRunningCommand(EIntentAction command)
            => Runtime.HasRunningCommand(command);

        /// <summary>
        /// 获取指定指令的所有运行中窗口
        /// </summary>
        public IReadOnlyList<RunningIntentContext> GetRunningCommands(EIntentAction command)
            => Runtime.GetRunningCommands(command);

        /// <summary>
        /// 添加运行中的指令窗口
        /// </summary>
        public void AddRunningCommand(EIntentAction command, RunningIntentContext context)
            => Runtime.AddRunningCommand(command, context);

        /// <summary>
        /// 移除运行中的指令窗口
        /// </summary>
        public bool RemoveRunningCommand(EIntentAction command, int stateInstanceId)
            => Runtime.RemoveRunningCommand(command, stateInstanceId);

        /// <summary>
        /// 检查是否存在指定信号的运行中窗口
        /// </summary>
        public bool HasRunningSignal(string signalName)
            => Runtime.HasRunningSignal(signalName);

        /// <summary>
        /// 获取指定信号的所有运行中窗口
        /// </summary>
        public IReadOnlyList<SignalTransitionInfo> GetRunningSignals(string signalName)
            => Runtime.GetRunningSignals(signalName);

        /// <summary>
        /// 添加运行中的信号窗口
        /// </summary>
        public void AddRunningSignal(SignalTransitionInfo signal)
            => Runtime.AddRunningSignal(signal);

        /// <summary>
        /// 移除运行中的信号窗口
        /// </summary>
        public bool RemoveRunningSignal(string signalName, string actionName)
            => Runtime.RemoveRunningSignal(signalName, actionName);

        /// <summary>
        /// 清空所有运行中的窗口/状态
        /// </summary>
        public void ClearRunningStates()
            => Runtime.ClearRunningStates();

        #endregion

        #region 队列管理（代理 QueueModel）

        /// <summary>
        /// 检查指定 ActionId 是否已在队列中
        /// </summary>
        public bool HasPendingIntent(int actionId)
            => Queue.HasPendingIntent(actionId);

        /// <summary>
        /// 添加可执行意图到队列
        /// </summary>
        public void AddPendingExecutableIntent(ExecutableIntent intent)
            => Queue.AddPendingExecutableIntent(intent);

        /// <summary>
        /// 清空队列
        /// </summary>
        public void ClearPendingExecutableIntents()
            => Queue.Clear();

        #endregion

        #region 属性访问

        /// <summary>表现动作字典（按动作名索引 TimelineActionSO）</summary>
        public Dictionary<string, TimelineActionSO> ActionDic => Data.ActionDic;

        /// <summary>当前正在播放的表现动作</summary>
        public TimelineActionSO CurrentAction => Runtime.CurrentAction;

        /// <summary>当前表现动作名称</summary>
        public string CurrentActionName => Runtime.CurrentAction?.actionName;

        /// <summary>待仲裁的可执行意图列表（只读）</summary>
        public IReadOnlyList<ExecutableIntent> PendingExecutableIntents => Queue.PendingExecutableIntents;

        /// <summary>当前动作累计播放时间</summary>
        public float TotalPlayTime
        {
            get => Runtime.TotalPlayTime;
            set => Runtime.TotalPlayTime = value;
        }

        /// <summary>运行中的 NotifyState 列表</summary>
        public List<RunningNotifyStateContext> RunningNotifyStates => Runtime.RunningNotifyStates;

        #endregion
    }
}