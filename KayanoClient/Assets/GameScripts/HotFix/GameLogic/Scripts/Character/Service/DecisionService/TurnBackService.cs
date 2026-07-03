using GameConfig.Main;
using KayanoAction.Runtime;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 转身服务 — 处理转身意图（LogicType = TurnBack）
    /// </summary>
    public class TurnBackService : DecisionServiceBase
    {
        public TurnBackService() : base(EIntentAction.Move) { }

        protected override bool CheckCondition(CharacterStore store, ChActionConfig config, IntentEvent intent)
        {
            if (config.ActionType != EActionType.TurnBack) return false;

            var state = store.ChState;
            if (state == null || !state.IsMoving) return false;

            Vector2 worldDir = intent.Direction;
            float angle = Vector2.Angle(state.LastInputDirection, worldDir);
            float threshold = store.TurnThreshold;

            return angle > threshold;
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