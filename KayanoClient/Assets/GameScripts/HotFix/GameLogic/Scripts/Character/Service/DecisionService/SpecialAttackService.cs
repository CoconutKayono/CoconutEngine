using GameConfig.Main;
using KayanoAction.Runtime;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 特殊攻击服务 — 处理特殊攻击意图（EnhancedSpecial），检查能量
    /// </summary>
    public class SpecialAttackService : DecisionServiceBase
    {
        public SpecialAttackService() : base(EIntentAction.SpecialAttack) { }

        protected override bool CheckCondition(CharacterModule module, ChActionConfig config, IntentEvent intent)
        {
            // 1. 只处理 EnhancedSpecial 逻辑类型
            if (config.ActionType != EActionType.EnhancedSpecial)
            {
                return false;
            }

            // 2. 检查能量是否充足
            var characterAttributes = module.CharacterAttributes;
            if (characterAttributes.CurrentEnergy < config.EnergyCost)
            {
                Log.Debug($"[SpecialAttackService] 能量不足！当前: {characterAttributes.CurrentEnergy}, 需要: {config.EnergyCost}");
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
                EnergyCost = config.EnergyCost,
            };
        }
    }
}