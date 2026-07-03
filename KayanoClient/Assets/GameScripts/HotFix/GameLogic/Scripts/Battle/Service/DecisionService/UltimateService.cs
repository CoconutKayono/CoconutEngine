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

        protected override bool CheckCondition(CharacterStore store, ChActionConfig config, IntentEvent intent)
        {
            if (config.ActionType != EActionType.Ultimate) return false;

            var characterAttributes = store.ChAttribute;
            if (characterAttributes.CurrentDecibels < characterAttributes.MaxDecibels)
            {
                Log.Debug($"[UltimateService] 喧响值不足！当前: {characterAttributes.CurrentDecibels}, 需要: {characterAttributes.MaxDecibels}");
                return false;
            }
            return true;
        }

        protected override ExecutableIntent? CreateExecutableIntent(
            CharacterStore store,
            int actionId,
            CommandTransitionInfo info,
            ChActionConfig config,
            IntentEvent intent)
        {
            var characterAttributes = store.ChAttribute;
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
                DecibelCost = characterAttributes.MaxDecibels,
            };
        }
    }
}