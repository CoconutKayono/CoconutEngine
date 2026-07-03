using GameConfig.Main;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 冲刺攻击服务 — 处理冲刺攻击（DashAttack）意图
    /// </summary>
    public class DashAttackService : DecisionServiceBase
    {
        public DashAttackService() : base(EIntentAction.Attack) { }

        protected override bool CheckCondition(CharacterStore store, ChActionConfig config, IntentEvent intent)
        {
            return config.ActionType == EActionType.DashAttack;
        }
    }
}