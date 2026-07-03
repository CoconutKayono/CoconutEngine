using GameConfig.Main;
using KayanoAction.Runtime;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 链招服务 — 处理链招意图，检查连携槽
    /// </summary>
    public class ChainService : DecisionServiceBase
    {
        public ChainService() : base(EIntentAction.Chain) { }

        protected override bool CheckCondition(CharacterModule module, ChActionConfig config, IntentEvent intent)
        {
            // 检查连携槽是否充足
            if (!BattleModule.Instance.IsChainReady)
            {
                Log.Debug($"[ChainService] 连携槽不足！当前: {BattleModule.Instance.ChainGauge}");
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
                ChainGaugeCost = config.ChainGaugeCost,
            };
        }
    }
}