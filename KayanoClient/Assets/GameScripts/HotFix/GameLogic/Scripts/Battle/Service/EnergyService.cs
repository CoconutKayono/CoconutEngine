using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 能量执行服务 — 监听 HitRequestEvent，独立处理攻击方能量恢复
    /// </summary>
    public class EnergyExecutionService
    {
        public EnergyExecutionService()
        {
            GameEvent.AddEventListener<HitRequestEvent>(
                IBattleEvents_Event.OnHitRequest,
                OnHitRequest);
        }

        private void OnHitRequest(HitRequestEvent evt)
        {
            var attackerStore = CharacterModule.Instance.GetUnit(evt.AttackerId);
            if (attackerStore == null) return;

            var attackerStats = attackerStore.ChAttribute;
            if (!attackerStats.IsAlive) return;

            float energyGain = 10f;
            attackerStats.CurrentEnergy += Mathf.RoundToInt(energyGain);
        }
    }
}