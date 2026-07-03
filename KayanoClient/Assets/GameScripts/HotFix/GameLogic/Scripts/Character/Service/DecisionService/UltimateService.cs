using GameConfig.Main;
using KayanoAction.Runtime;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 终极技服务 — 处理终极技意图，检查喧响值
    /// </summary>
    public class UltimateService : DecisionServiceBase
    {
        public UltimateService() : base(EIntentAction.Ultimate) { }

        protected override bool CheckCondition(CharacterModule module, ChActionConfig config, IntentEvent intent)
        {
            // 1. 只处理 Ultimate 类型
            if (config.ActionType != EActionType.Ultimate) return false;

            // 2. 检查喧响值是否满
            var characterAttributes = module.CharacterAttributes;
            if (characterAttributes.CurrentDecibels < characterAttributes.MaxDecibels)
            {
                Log.Debug($"[UltimateService] 喧响值不足！当前: {characterAttributes.CurrentDecibels}, 需要: {characterAttributes.MaxDecibels}");
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
            var characterAttributes = module.CharacterAttributes;
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
                DecibelCost = characterAttributes.MaxDecibels,
            };
        }
    }
}