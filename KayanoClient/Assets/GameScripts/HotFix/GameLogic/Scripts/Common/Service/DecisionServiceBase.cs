using GameConfig.Main;
using KayanoAction.Runtime;
using System;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 决策服务基类 — 处理输入意图，生成可执行意图
    /// </summary>
    public abstract class DecisionServiceBase
    {
        private EIntentAction _command;
        private int _event;

        protected DecisionServiceBase(EIntentAction command)
        {
            _command = command;
            _event = GetEventType(command);
            GameEvent.AddEventListener<IntentEvent>(_event, OnIntent);
        }

        /// <summary>
        /// 意图事件处理入口
        /// </summary>
        private void OnIntent(IntentEvent intent)
        {
            if (intent.Action != _command) return;

            var store = CharacterModule.Instance.GetUnit(intent.InstanceId);
            if (store == null) return;

            var actionState = store.ChActionState;
            if (actionState == null) return;

            if (!actionState.HasRunningCommand(_command)) return;

            var runningCommands = actionState.GetRunningCommands(_command);
            if (runningCommands == null || runningCommands.Count == 0) return;

            for (int i = 0; i < runningCommands.Count; i++)
            {
                var info = runningCommands[i].Info;
                if (string.IsNullOrEmpty(info.actionName))
                {
                    Log.Warning($"[{GetType().Name}] 通道中 ActionName 为空。InstanceId: {intent.InstanceId}");
                    continue;
                }

                if (!actionState.TryGetActionId(info.actionName, out int actionId))
                {
                    Log.Warning($"[{GetType().Name}] 未找到 ActionId，ActionName: {info.actionName}");
                    continue;
                }

                var config = IntentHelper.GetActionConfig(actionId);
                if (config == null) continue;

                // 子类重写此方法实现具体逻辑
                if (!CheckCondition(store, config, intent))
                {
                    continue;
                }

                // 子类可重写此方法自定义 ExecutableIntent 构造
                var executable = CreateExecutableIntent(store, actionId, info, config, intent);
                if (executable.HasValue)
                {
                    actionState.AddPendingExecutableIntent(executable.Value);
                }
            }
        }

        /// <summary>
        /// 子类实现：检查资源/条件是否满足
        /// </summary>
        protected virtual bool CheckCondition(CharacterStore store, ChActionConfig config, IntentEvent intent)
        {
            return true;
        }

        /// <summary>
        /// 子类可重写：自定义 ExecutableIntent 构造
        /// </summary>
        protected virtual ExecutableIntent? CreateExecutableIntent(
            CharacterStore store,
            int actionId,
            CommandTransitionInfo info,
            ChActionConfig config,
            IntentEvent intent)
        {
            return new ExecutableIntent
            {
                InstanceId = store.InstanceId,
                ActionId = actionId,
                ActionName = info.actionName,
                Priority = config.Priority,
                Phase = intent.Phase,
                HoldTime = intent.HoldTime,
                Direction = intent.Direction,
                ChainDir = intent.ChainDir,
            };
        }

        /// <summary>
        /// 获取意图类型对应的事件枚举
        /// </summary>
        private int GetEventType(EIntentAction command)
        {
            return command switch
            {
                EIntentAction.Move => IActionIntentEvents_Event.OnMoveIntent,
                EIntentAction.Attack => IActionIntentEvents_Event.OnAttackIntent,
                EIntentAction.SpecialAttack => IActionIntentEvents_Event.OnSpecialAttackIntent,
                EIntentAction.Dodge => IActionIntentEvents_Event.OnDodgeIntent,
                EIntentAction.Ultimate => IActionIntentEvents_Event.OnUltimateIntent,
                EIntentAction.Sprint => IActionIntentEvents_Event.OnSprintIntent,
                EIntentAction.Interact => IActionIntentEvents_Event.OnInteractIntent,
                EIntentAction.Chain => IActionIntentEvents_Event.OnChainIntent,
                EIntentAction.None => IActionIntentEvents_Event.OnNoneIntent,
                _ => throw new System.NotImplementedException(),
            };
        }

        public void Dispose()
        {
            GameEvent.RemoveEventListener<IntentEvent>(_event, OnIntent);
        }
    }
}