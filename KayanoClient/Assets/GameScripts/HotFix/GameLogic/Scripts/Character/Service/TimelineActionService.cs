using KayanoAction.Runtime;
using System;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 推进 Timeline 调度并调用 Notify / NotifyState（Scripts 侧唯一执行入口）。
    /// </summary>
    public class TimelineActionService
    {
        #region States
        private CharacterStore _characterStore;
        private ChActionModel _actionState;
        private TimelineActionSO _currentAction;

        private float _time;
        private float _totalPlayTime;
        private int _notifyIndex;
        private int _notifyStateIndex;
        private int _commandIndex;
        private int _signalIndex;

        private static int _nextStateInstanceId = 1;
        #endregion

        #region Event
        public event Action<string> OnActionChanged;
        #endregion

        #region Constructor
        public TimelineActionService(CharacterStore characterStore)
        {
            _characterStore = characterStore;
            _actionState = characterStore.ChActionState;
            _currentAction = _actionState.CurrentAction;
        }
        #endregion

        #region Actions
        /// <summary>
        /// 每帧推进 Timeline
        /// </summary>
        public void Tick(float deltaTime)
        {
            // 1. 安全检查
            if (_currentAction == null || _currentAction.clip == null)
            {
                Log.Warning("[TimelineActionService] 当前动作或动画剪辑为空，跳过执行");
                return;
            }

            // 2. 推进时间
            _time += deltaTime;
            _totalPlayTime += deltaTime;

            // 同步更新到 ActionStateModel，供外部读取
            _actionState.TotalPlayTime = _totalPlayTime;

            var clipLength = _currentAction.clip.length;

            // 3. 动作播完处理（循环 / 停止）
            if (_time > clipLength)
            {
                EndAllRunningStates();
                ResetSchedule();

                var finish = _currentAction.finishTransition;
                if (finish != null && !string.IsNullOrEmpty(finish.actionName))
                {
                    SwitchTo(finish.actionName, finish.fadeDuration);
                }
                else
                {
                    Log.Error($"[TimelineActionService] 动作 '{_currentAction?.actionName ?? "Unknown"}' 播完但未配置 finishTransition");
                }
            }

            var ctx = BuildContext();

            // 4. 处理瞬时 Notify（触发一次）
            var notifies = _currentAction.notifies;
            while (_notifyIndex < notifies.Count && notifies[_notifyIndex].scheduleTime <= _time)
            {
                notifies[_notifyIndex].Notify(ctx);
                _notifyIndex++;
            }

            // 5. 处理 NotifyState Begin（进入）
            var notifyStates = _currentAction.notifyStates;
            while (_notifyStateIndex < notifyStates.Count && notifyStates[_notifyStateIndex].start <= _time)
            {
                var state = notifyStates[_notifyStateIndex];
                int instanceId = AllocateStateInstanceId();
                _actionState.RunningNotifyStates.Add(new RunningNotifyStateContext
                {
                    State = state,
                    StateInstanceId = instanceId,
                });
                state.Enter(ctx, instanceId);
                _notifyStateIndex++;
            }

            // 6. 处理 NotifyState End（到期退出）
            var running = _actionState.RunningNotifyStates;
            for (int i = running.Count - 1; i >= 0; i--)
            {
                var context = running[i];
                if (context.State.start + context.State.length <= _time)
                {
                    context.State.Exit(ctx, context.StateInstanceId);
                    running.RemoveAt(i);
                }
            }

            // 7. 处理全局指令转移
            var commands = _currentAction.commandTransitions;
            while (_commandIndex < commands.Count && commands[_commandIndex].openTime <= _totalPlayTime)
            {
                var command = commands[_commandIndex];
                _actionState.AddRunningCommand(command.command, new RunningIntentContext
                {
                    Info = command,
                    StateInstanceId = -1
                });
                _commandIndex++;
            }

            // 8. 处理 全局信号过渡信息
            var signals = _currentAction.signalTransitions;
            while (_signalIndex < signals.Count && signals[_signalIndex].openTime <= _totalPlayTime)
            {
                var signal = signals[_signalIndex];
                _actionState.AddRunningSignal(signal);
                _signalIndex++;
            }
        }

        /// <summary>
        /// 切换动作（外部调用）
        /// </summary>
        public bool SwitchTo(string actionName, float fadeDuration = 0.15f)
        {
            if (!_actionState.SwitchTo(actionName))
            {
                Log.Warning($"[TimelineActionService] 切换动作失败：{actionName}");
                return false;
            }

            EndAllRunningStates();

            _currentAction = _actionState.CurrentAction;
            ResetSchedule();

            _totalPlayTime = 0;

            // 同步重置到 ActionStateModel
            _actionState.TotalPlayTime = 0;

            RefreshOpenTransitions(); // 解决首帧空窗问题

            var animator = _characterStore.Animator;
            if (animator != null)
            {
                animator.CrossFadeInFixedTime(actionName, fadeDuration);
            }

            OnActionChanged?.Invoke(actionName);
            Log.Info($"[TimelineActionService] 切换到动作：{actionName}");
            return true;
        }

        public void Dispose()
        {
            EndAllRunningStates();
            _characterStore = null;
            _actionState = null;
            _currentAction = null;
        }
        #endregion

        #region Utils Methods
        /// <summary>
        /// 结束所有正在运行的 NotifyState（倒序遍历，调用 Exit ）
        /// </summary>
        private void EndAllRunningStates()
        {
            var running = _actionState.RunningNotifyStates;
            if (running.Count == 0) return;

            var ctx = BuildContext();

            for (int i = running.Count - 1; i >= 0; i--)
            {
                var context = running[i];
                try
                {
                    context.State.Exit(ctx, context.StateInstanceId);
                }
                catch (Exception e)
                {
                    Log.Error($"[TimelineActionService] NotifyState Exit 异常：{e.Message}");
                }
                running.RemoveAt(i);
            }

            _actionState.ClearRunningStates();
        }

        private static int AllocateStateInstanceId()
        {
            return _nextStateInstanceId++;
        }

        /// <summary>
        /// 重置调度游标和时间
        /// </summary>
        private void ResetSchedule()
        {
            _time = 0f;
            _notifyIndex = 0;
            _notifyStateIndex = 0;
            _commandIndex = 0;
            _signalIndex = 0;
        }

        /// <summary>
        /// 构建 Timeline 上下文
        /// </summary>
        private ActionTimelineContext BuildContext()
        {
            return new ActionTimelineContext
            {
                ConfigId = _characterStore.ChAttribute.ConfigId,
                Owner = _characterStore.Owner,
                ActionId = _currentAction?.actionId ?? 0,
                InstanceId = _characterStore.InstanceId
            };
        }

        private void RefreshOpenTransitions()
        {
            if (_currentAction == null) return;

            var commands = _currentAction.commandTransitions;
            while (_commandIndex < commands.Count && commands[_commandIndex].openTime <= _totalPlayTime)
            {
                var command = commands[_commandIndex];
                _actionState.AddRunningCommand(command.command, new RunningIntentContext
                {
                    Info = command,
                    StateInstanceId = -1
                });
                _commandIndex++;
            }

            var signals = _currentAction.signalTransitions;
            while (_signalIndex < signals.Count && signals[_signalIndex].openTime <= _totalPlayTime)
            {
                _actionState.AddRunningSignal(signals[_signalIndex]);
                _signalIndex++;
            }
        }
        #endregion

        #region Getter
        public TimelineActionSO CurrentAction => _currentAction;
        public float CurrentTime => _time;
        public float TotalPlayTime => _totalPlayTime;
        public bool IsPlaying => _currentAction != null && _time <= _currentAction?.clip?.length;
        #endregion
    }
}