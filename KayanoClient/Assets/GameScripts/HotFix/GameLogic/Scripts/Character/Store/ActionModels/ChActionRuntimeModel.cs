using System.Collections.Generic;
using KayanoAction.Runtime;
using TEngine;

namespace GameLogic
{
    /// <summary>
    /// 动作运行时模型 — 管理运行中的窗口状态（指令窗口、信号窗口、NotifyState）
    /// </summary>
    public class ChActionRuntimeModel
    {
        #region States

        private TimelineActionSO _currentAction;
        private float _totalPlayTime;

        /// <summary>运行中的 NotifyState 上下文</summary>
        private readonly List<RunningNotifyStateContext> _runningNotifyStates = new(16);

        /// <summary>运行中的指令窗口（按 Command 分组）</summary>
        private readonly Dictionary<EIntentAction, List<RunningIntentContext>> _runningCommands = new();

        /// <summary>运行中的信号窗口（按 SignalName 分组）</summary>
        private readonly Dictionary<string, List<SignalTransitionInfo>> _runningSignals = new();

        #endregion

        #region Properties

        /// <summary>当前动作</summary>
        public TimelineActionSO CurrentAction => _currentAction;

        /// <summary>累计播放时间</summary>
        public float TotalPlayTime
        {
            get => _totalPlayTime;
            set => _totalPlayTime = value;
        }

        /// <summary>运行中的 NotifyState 列表</summary>
        public List<RunningNotifyStateContext> RunningNotifyStates => _runningNotifyStates;

        #endregion

        #region 当前动作

        /// <summary>设置当前动作</summary>
        public void SetCurrentAction(TimelineActionSO action)
        {
            _currentAction = action;
        }

        #endregion

        #region 指令窗口（Command Window）

        /// <summary>检查是否存在指定指令的运行中窗口</summary>
        public bool HasRunningCommand(EIntentAction command)
        {
            return _runningCommands.ContainsKey(command) && _runningCommands[command].Count > 0;
        }

        /// <summary>获取指定指令的所有运行中窗口</summary>
        public IReadOnlyList<RunningIntentContext> GetRunningCommands(EIntentAction command)
        {
            if (_runningCommands.TryGetValue(command, out var list))
            {
                return list;
            }
            return System.Array.Empty<RunningIntentContext>();
        }

        /// <summary>添加运行中的指令窗口</summary>
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

        /// <summary>移除运行中的指令窗口</summary>
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

        #region 信号窗口（Signal Window）

        /// <summary>检查是否存在指定信号的运行中窗口</summary>
        public bool HasRunningSignal(string signalName)
        {
            return _runningSignals.ContainsKey(signalName) && _runningSignals[signalName].Count > 0;
        }

        /// <summary>获取指定信号的所有运行中窗口</summary>
        public IReadOnlyList<SignalTransitionInfo> GetRunningSignals(string signalName)
        {
            if (_runningSignals.TryGetValue(signalName, out var list))
            {
                return list;
            }
            return System.Array.Empty<SignalTransitionInfo>();
        }

        /// <summary>添加运行中的信号窗口</summary>
        public void AddRunningSignal(SignalTransitionInfo signal)
        {
            if (string.IsNullOrEmpty(signal.signalName))
            {
                Log.Warning("[ChActionRuntimeModel] AddRunningSignal 失败：signalName 为空");
                return;
            }

            if (!_runningSignals.TryGetValue(signal.signalName, out var list))
            {
                list = new List<SignalTransitionInfo>();
                _runningSignals[signal.signalName] = list;
            }

            // 防重复：相同信号 + 相同动作 + 相同开启时间不重复添加
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

        /// <summary>移除运行中的信号窗口</summary>
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

        #region 清理

        /// <summary>
        /// 清空所有运行中的窗口/状态（动作切换时调用）
        /// </summary>
        public void ClearRunningStates()
        {
            _runningNotifyStates.Clear();

            // 清空指令窗口（保留字典，只清空列表内容）
            foreach (var kvp in _runningCommands)
            {
                kvp.Value.Clear();
            }

            // 清空信号窗口（保留字典，只清空列表内容）
            foreach (var kvp in _runningSignals)
            {
                kvp.Value.Clear();
            }
        }

        #endregion
    }
}