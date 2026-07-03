using GameConfig.Main;
using KayanoAction.Runtime;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 前闪服务 — 方向键 + 闪避
    /// </summary>
    public class FrontDodgeService : DecisionServiceBase
    {
        public FrontDodgeService() : base(EIntentAction.Dodge) { }

        protected override bool CheckCondition(CharacterModule module, ChActionConfig config, IntentEvent intent)
        {
            if (config.ActionType != EActionType.DodgeForward) return false;
            if (intent.Direction.sqrMagnitude < 0.01f) return false;

            if (module.CharacterAttributes.CurrentDodgeStamina < config.DodgeStaminaCost)
            {
                Log.Debug($"[FrontDodgeService] 闪避体力不足！当前: {module.CharacterAttributes.CurrentDodgeStamina}, 需要: {config.DodgeStaminaCost}");
                return false;
            }
            return true;
        }

        protected override ExecutableIntent? CreateExecutableIntent(
            CharacterModule module,
            int actionId,
            CommandTransitionInfo info,
            ChActionConfig config,
            IntentEvent intent)
        {
            return new ExecutableIntent
            {
                InstanceId = module.InstanceId,
                ActionId = actionId,
                ActionName = info.actionName,
                Priority = config.Priority,
                Phase = intent.Phase,
                HoldTime = intent.HoldTime,
                Direction = intent.Direction,
                ChainDir = intent.ChainDir,
                DodgeStaminaCost = config.DodgeStaminaCost,
            };
        }
    }
}