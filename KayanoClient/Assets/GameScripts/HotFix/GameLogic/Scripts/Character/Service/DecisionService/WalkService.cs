using GameConfig.Main;
using KayanoAction.Runtime;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 行走服务 — 处理普通行走意图
    /// </summary>
    public class WalkService : DecisionServiceBase
    {
        public WalkService() : base(EIntentAction.Move) { }

        protected override bool CheckCondition(CharacterStore store, ChActionConfig config, IntentEvent intent)
        {
            return config.ActionType == EActionType.Walk;
        }

        protected override ExecutableIntent? CreateExecutableIntent(
            CharacterStore store,
            int actionId,
            CommandTransitionInfo info,
            ChActionConfig config,
            IntentEvent intent)
        {
            Vector2 worldDir = intent.Direction;
            if (worldDir.sqrMagnitude < 0.001f) return null;

            return new ExecutableIntent
            {
                InstanceId = store.InstanceId,
                ActionId = actionId,
                ActionName = info.actionName,
                Priority = config.Priority,
                Phase = intent.Phase,
                HoldTime = intent.HoldTime,
                Direction = worldDir,
                ChainDir = intent.ChainDir,
            };
        }
    }
}