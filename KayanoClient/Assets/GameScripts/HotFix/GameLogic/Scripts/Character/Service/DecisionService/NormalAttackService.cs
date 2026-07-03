using GameConfig.Main;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 攻击服务 — 处理普通攻击（NormalAttack）意图
    /// </summary>
    public class NormalAttackService : DecisionServiceBase
    {
        public NormalAttackService() : base(EIntentAction.Attack) { }

        protected override bool CheckCondition(CharacterModule module, ChActionConfig config, IntentEvent intent)
        {
            return config.ActionType == EActionType.NormalAttack;
        }
    }
}