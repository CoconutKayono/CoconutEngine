using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 伤害执行服务 — 监听 HitRequestEvent，独立执行扣血和击杀判定
    /// </summary>
    public class DamageService
    {

        public DamageService()
        {
            GameEvent.AddEventListener<HitRequestEvent>(
                IBattleEvents_Event.OnHitRequest,
                OnHitRequest);
        }

        private void OnHitRequest(HitRequestEvent evt)
        {
            // 1. 获取攻击者/受击者模块
            var attackerStore = CharacterModule.Instance.GetUnit(evt.AttackerId);
            var defenderStore = CharacterModule.Instance.GetUnit(evt.TargetId);
            if (attackerStore == null || defenderStore == null) return;

            var attackerStats = attackerStore.ChAttribute;
            var defenderStats = defenderStore.ChAttribute;

            // 2. 检查受击者是否存活
            if (!defenderStats.IsAlive) return;

            // 3. 判断是否处于失衡状态
            bool isEnemyStunned = BattleModule.Instance.Store.IsEnemyStunned(evt.TargetId);

            // 4. 获取招式配置
            var hitConfig = HitHelper.GetHitConfig(evt.BoxData.HitId);

            // 5. 计算伤害
            var result = DamageCalculatorHelper.CalculateHitResult(
                attackerStats,
                defenderStats,
                hitConfig.DamageMult,
                hitConfig.DazeMult,
                isEnemyStunned
            );

            // 6. 执行扣血
            defenderStats.Health -= Mathf.RoundToInt(result.FinalDamage);

            // 7. 检查是否击杀
            bool isKilled = !defenderStats.IsAlive;

            // 8. 发送命中结算事件
            GameEvent.Get<IBattleEvents>().OnHitResult(new HitResultEvent
            {
                AttackerId = evt.AttackerId,
                TargetId = evt.TargetId,
                Damage = result.FinalDamage,
                StunValue = result.FinalStun,
                IsCritical = result.IsCritical,
                IsKilled = isKilled,
                HitPoint = evt.HitPoint,
                HitDirection = evt.HitDirection,
            });

            // 9. 如果死亡，发送死亡事件
            if (isKilled)
            {
                GameEvent.Get<IBattleEvents>().OnUnitDeath(new UnitDeathEvent
                {
                    AttackerId = evt.AttackerId,
                    TargetId = evt.TargetId,
                    DeathPoint = defenderStore.Owner.transform.position,
                    UnitType = defenderStore.UnitType,
                });
            }
        }
    }
}