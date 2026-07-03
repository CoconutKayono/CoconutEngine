using GameConfig.Main;
using KayanoAction.Runtime;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 冲刺服务 — 处理冲刺/奔跑意图
    /// </summary>
    public class SprintService : DecisionServiceBase
    {
        public SprintService() : base(EIntentAction.Move) { }

        protected override bool CheckCondition(CharacterStore store, ChActionConfig config, IntentEvent intent)
        {
            return config.ActionType == EActionType.Sprint;
        }

        protected override ExecutableIntent? CreateExecutableIntent(
            CharacterStore store,
            int actionId,
            CommandTransitionInfo info,
            ChActionConfig config,
            IntentEvent intent)
        {
            // TODO: 补充冲刺状态检查（module.ChState.IsSprinting）
            // 当前由 MoveDirectionService 维护 IsSprinting，后续完善后恢复检查
            // if (!module.ChState.IsSprinting) return null;

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