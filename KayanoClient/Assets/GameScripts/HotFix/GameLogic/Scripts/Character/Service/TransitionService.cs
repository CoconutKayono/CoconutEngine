using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 过渡服务 — 监听信号和意图过渡事件，驱动动作切换
    /// </summary>
    public class TransitionService
    {
        #region States
        private TimelineActionService _timelineAction;
        private CharacterStore _characterStore;
        #endregion

        #region Constructor
        public TransitionService(TimelineActionService timelineAction, CharacterStore characterStore)
        {
            _timelineAction = timelineAction;
            _characterStore = characterStore;

            GameEvent.AddEventListener<SignalTransitionEvent>(
                IActionTimelineEvents_Event.OnSignalTransition,
                OnSignalTriggered);

            GameEvent.AddEventListener<IntentTransitionEvent>(
                IActionTimelineEvents_Event.OnIntentTransitionEnter,
                OnEnterIntentTriggered);

            GameEvent.AddEventListener<IntentTransitionEvent>(
                IActionTimelineEvents_Event.OnIntentTransitionExit,
                OnExitIntentTriggered);
        }
        #endregion

        #region Event Handlers

        public void OnSignalTriggered(SignalTransitionEvent signal)
        {
            if (signal.Context.InstanceId != _characterStore.InstanceId)
                return;

            var targetSignalName = signal.TransitionInfo.signalName;
            if (string.IsNullOrEmpty(targetSignalName))
            {
                Log.Warning($"[TransitionService] 信号名为空，无法处理");
                return;
            }

            // O(1) 获取特定信号的运行中窗口
            var runningSignals = _characterStore.ChActionState.GetRunningSignals(targetSignalName);
            if (runningSignals == null || runningSignals.Count == 0)
            {
                Log.Debug($"[TransitionService] 角色 {_characterStore.InstanceId} 收到信号 {targetSignalName}，但无匹配通道");
                return;
            }

            // 按 Timeline 顺序取第一个匹配的信号通道（列表本身已按 openTime 排序）
            for (int i = 0; i < runningSignals.Count; i++)
            {
                var signalInfo = runningSignals[i];
                if (string.IsNullOrEmpty(signalInfo.actionName))
                {
                    Log.Warning($"[TransitionService] 信号 {targetSignalName} 的 actionName 为空");
                    continue;
                }

                Log.Debug($"[TransitionService] 角色 {_characterStore.InstanceId} 信号 {targetSignalName} → 切换到动作: {signalInfo.actionName}");
                _timelineAction?.SwitchTo(signalInfo.actionName, signalInfo.fadeDuration);
                return;
            }
        }

        public void OnEnterIntentTriggered(IntentTransitionEvent intent)
        {
            if (intent.Context.InstanceId != _characterStore.InstanceId) return;

            _characterStore.ChActionState.AddRunningCommand(
                intent.TransitionInfo.command,
                new RunningIntentContext
                {
                    Info = intent.TransitionInfo,
                    StateInstanceId = intent.StateInstanceId,
                }
            );
        }

        public void OnExitIntentTriggered(IntentTransitionEvent intent)
        {
            if (intent.Context.InstanceId != _characterStore.InstanceId) return;

            _characterStore.ChActionState.RemoveRunningCommand(
                intent.TransitionInfo.command,
                intent.StateInstanceId
            );
        }

        #endregion

        #region Dispose
        public void Dispose()
        {
            GameEvent.RemoveEventListener<SignalTransitionEvent>(
                IActionTimelineEvents_Event.OnSignalTransition,
                OnSignalTriggered);

            GameEvent.RemoveEventListener<IntentTransitionEvent>(
                IActionTimelineEvents_Event.OnIntentTransitionEnter,
                OnEnterIntentTriggered);

            GameEvent.RemoveEventListener<IntentTransitionEvent>(
                IActionTimelineEvents_Event.OnIntentTransitionExit,
                OnExitIntentTriggered);

            _timelineAction = null;
            _characterStore = null;
        }
        #endregion
    }
}