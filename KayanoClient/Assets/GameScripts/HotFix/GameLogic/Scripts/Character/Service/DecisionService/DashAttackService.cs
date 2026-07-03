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

        protected override bool CheckCondition(CharacterModule module, ChActionConfig config, IntentEvent intent)
        {
            // TODO: 待 Sprint 状态维护完善后，补充检查 module.CharacterState.IsSprinting
            return config.ActionType == EActionType.DashAttack;
        }
    }
}