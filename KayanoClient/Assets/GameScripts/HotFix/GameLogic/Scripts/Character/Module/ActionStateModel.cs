using System.Collections.Generic;
using GameConfig.Main;
using KayanoAction.Runtime;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    #region 数据结构定义

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

        // 资源消耗字段
        public float EnergyCost;
        public float DecibelCost;
        public float ChainGaugeCost;
        public float DodgeStaminaCost;
    }

    public struct ActionTimelineContext
    {
        public int InstanceId { get; set; }
        public int ConfigId { get; set; }
        public Transform Owner { get; set; }
        public int ActionId { get; set; }
        public bool IsValid => ConfigId > 0 && Owner != null;
    }

    public struct RunningNotifyStateContext
    {
        public NotifyStateSO State;
        public int StateInstanceId;
    }

    public struct RunningIntentContext
    {
        public CommandTransitionInfo Info;
        public int StateInstanceId;
    }

    #endregion

    #region 动作状态模型

    public class ActionStateModel
    {
        #region States

        private readonly Dictionary<string, TimelineActionSO> _actionDic;
        private TimelineActionSO _currentAction;

        private readonly List<RunningNotifyStateContext> _runningNotifyStates = new(16);

        /// <summary>运行中的指令窗口 — 按 Command 分组，O(1) 查询</summary>
        private readonly Dictionary<EIntentAction, List<RunningIntentContext>> _runningCommands = new();

        /// <summary>运行中的信号窗口 — 按 SignalName 分组，O(1) 查询</summary>
        private readonly Dictionary<string, List<SignalTransitionInfo>> _runningSignals = new();

        private readonly List<ExecutableIntent> _pendingExecutableIntents = new(32);
        private readonly HashSet<int> _pendingLookup = new();

        #endregion

        #region Construct

        public ActionStateModel(List<TimelineActionSO> actions, string defaultActionName)
        {
            _actionDic = new Dictionary<string, TimelineActionSO>();

            foreach (var action in actions)
            {
                if (_actionDic.ContainsKey(action.actionName))
                {
                    Log.Error($"动作名称 '{action.actionName}' 重复，将覆盖之前的条目");
                }
                _actionDic[action.actionName] = action;
            }

            if (_actionDic.TryGetValue(defaultActionName, out var defaultAction))
            {
                _currentAction = defaultAction;
                return;
            }

            if (actions.Count > 0)
            {
                _currentAction = actions[0];
                Log.Warning($"已自动使用第一个动作 '{_currentAction.actionName}' 作为默认");
                return;
            }

            _currentAction = null;
            Log.Error("动作列表为空，无法初始化 ActionStateModel");
        }

        #endregion

        #region Actions

        public bool SwitchTo(string actionName)
        {
            if (string.IsNullOrEmpty(actionName))
            {
                Log.Warning("SwitchTo 失败：动作名称为空");
                return false;
            }

            if (!_actionDic.TryGetValue(actionName, out var newAction))
            {
                Log.Warning($"SwitchTo 失败：动作 '{actionName}' 不存在");
                return false;
            }

            if (_currentAction == newAction)
            {
                return false;
            }

            _currentAction = newAction;
            return true;
        }

        /// <summary>
        /// 清空所有运行中的窗口/状态（动作切换时调用）
        /// </summary>
        public void ClearRunningStates()
        {
            _runningNotifyStates.Clear();

            // 清空指令窗口（按 Command 分组）
            foreach (var kvp in _runningCommands)
            {
                kvp.Value.Clear();
            }

            // 清空信号窗口（按 SignalName 分组）
            foreach (var kvp in _runningSignals)
            {
                kvp.Value.Clear();
            }
        }

        #region 指令窗口（Command Window）— O(1)

        /// <summary>
        /// 检查是否存在指定指令的运行中窗口（O(1)）
        /// </summary>
        public bool HasRunningCommand(EIntentAction command)
        {
            return _runningCommands.ContainsKey(command) && _runningCommands[command].Count > 0;
        }

        /// <summary>
        /// 获取指定指令的所有运行中窗口（O(1)，返回只读列表）
        /// </summary>
        public IReadOnlyList<RunningIntentContext> GetRunningCommands(EIntentAction command)
        {
            if (_runningCommands.TryGetValue(command, out var list))
            {
                return list;
            }
            return System.Array.Empty<RunningIntentContext>();
        }

        /// <summary>
        /// 添加运行中的指令窗口（O(1)）
        /// </summary>
        public void AddRunningCommand(EIntentAction command, RunningIntentContext context)
        {
            if (!_runningCommands.TryGetValue(command, out var list))
            {
                list = new List<RunningIntentContext>();
                _runningCommands[command] = list;
            }

            // 按 StateInstanceId 去重
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].StateInstanceId == context.StateInstanceId)
                {
                    return;
                }
            }

            list.Add(context);
        }

        /// <summary>
        /// 移除运行中的指令窗口（按 StateInstanceId，O(1)）
        /// </summary>
        public bool RemoveRunningCommand(EIntentAction command, int stateInstanceId)
        {
            if (!_runningCommands.TryGetValue(command, out var list))
            {
                return false;
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].StateInstanceId == stateInstanceId)
                {
                    list.RemoveAt(i);
                    if (list.Count == 0)
                    {
                        _runningCommands.Remove(command);
                    }
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 信号窗口（Signal Window）— O(1)

        /// <summary>
        /// 检查是否存在指定信号的运行中窗口（O(1)）
        /// </summary>
        public bool HasRunningSignal(string signalName)
        {
            return _runningSignals.ContainsKey(signalName) && _runningSignals[signalName].Count > 0;
        }

        /// <summary>
        /// 获取指定信号的所有运行中窗口（O(1)，返回只读列表）
        /// </summary>
        public IReadOnlyList<SignalTransitionInfo> GetRunningSignals(string signalName)
        {
            if (_runningSignals.TryGetValue(signalName, out var list))
            {
                return list;
            }
            return System.Array.Empty<SignalTransitionInfo>();
        }

        /// <summary>
        /// 添加运行中的信号窗口（O(1)）
        /// </summary>
        public void AddRunningSignal(SignalTransitionInfo signal)
        {
            if (string.IsNullOrEmpty(signal.signalName))
            {
                Log.Warning("AddRunningSignal 失败：signalName 为空");
                return;
            }

            if (!_runningSignals.TryGetValue(signal.signalName, out var list))
            {
                list = new List<SignalTransitionInfo>();
                _runningSignals[signal.signalName] = list;
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].signalName == signal.signalName
                    && list[i].actionName == signal.actionName
                    && list[i].openTime == signal.openTime)
                {
                    return;
                }
            }

            list.Add(signal);
        }

        /// <summary>
        /// 移除运行中的信号窗口（O(1)）
        /// </summary>
        public bool RemoveRunningSignal(string signalName, string actionName)
        {
            if (!_runningSignals.TryGetValue(signalName, out var list))
            {
                return false;
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].signalName == signalName && list[i].actionName == actionName)
                {
                    list.RemoveAt(i);
                    if (list.Count == 0)
                    {
                        _runningSignals.Remove(signalName);
                    }
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 可执行意图候选列表

        public bool HasPendingIntent(int actionId)
        {
            return _pendingLookup.Contains(actionId);
        }

        public void AddPendingExecutableIntent(ExecutableIntent intent)
        {
            if (intent.ActionId <= 0)
            {
                Log.Warning($"[ActionStateModel] 尝试添加 ActionId 无效的可执行意图");
                return;
            }

            if (!_pendingLookup.Add(intent.ActionId))
            {
                return;
            }

            _pendingExecutableIntents.Add(intent);
        }

        public void ClearPendingExecutableIntents()
        {
            _pendingExecutableIntents.Clear();
            _pendingLookup.Clear();
        }

        #endregion

        public List<string> GetAvailableActionNames()
        {
            return new List<string>(_actionDic.Keys);
        }

        public bool HasAction(string actionName)
        {
            return _actionDic.ContainsKey(actionName);
        }

        public bool TryGetActionId(string actionName,out int actionId)
        {
            if (_actionDic.TryGetValue(actionName, out var action))
            {
                actionId = action.actionId;
                return true;
            }
            actionId = -1;
            return false;
        }

        #endregion

        #region Getter & Setter

        public Dictionary<string, TimelineActionSO> ActionDic => _actionDic;
        public TimelineActionSO CurrentAction => _currentAction;
        public string CurrentActionName => _currentAction?.actionName;

        public List<RunningNotifyStateContext> RunningNotifyStates => _runningNotifyStates;
        public IReadOnlyList<ExecutableIntent> PendingExecutableIntents => _pendingExecutableIntents;

        public float TotalPlayTime { get; set; }
        #endregion
    }

    #endregion
}