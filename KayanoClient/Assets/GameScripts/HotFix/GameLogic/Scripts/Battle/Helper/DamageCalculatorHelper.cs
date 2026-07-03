using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 伤害计算结果
    /// </summary>
    public struct DamageResult
    {
        public float FinalDamage;   // 最终伤害
        public float FinalStun;     // 最终失衡值
        public bool IsCritical;     // 是否暴击
    }

    /// <summary>
    /// 伤害计算服务
    /// </summary>
    public static class DamageCalculatorHelper
    {
        /// <summary>
        /// 计算完整战斗命中结果（伤害 + 失衡 + 暴击）
        /// </summary>
        /// <param name="attacker">攻击方属性</param>
        /// <param name="defender">防御方属性</param>
        /// <param name="damageMult">招式伤害系数</param>
        /// <param name="dazeMult">招式失衡系数</param>
        /// <param name="isEnemyStunned">敌人是否处于失衡状态</param>
        public static DamageResult CalculateHitResult(
            CharacterAttributeModel attacker,
            CharacterAttributeModel defender,
            float damageMult,
            float dazeMult,
            bool isEnemyStunned)
        {
            // 1. 基础属性
            float attack = attacker.GetAttr(100);
            float defense = defender.GetAttr(200);
            float penetrationValue = attacker.GetAttr(1100);
            float penetrationRate = attacker.GetAttr(1000);
            float damageBonus = attacker.GetAttr(1200);
            float critRate = attacker.GetAttr(800);
            float critDmg = attacker.GetAttr(900);

            // 防御减伤率 = 有效防御 / (攻击力 + 有效防御)
            // 有效防御 = (防御力 - 穿透值) * (1 - 穿透率)
            float effectiveDefense = defense - penetrationValue;
            effectiveDefense *= (1 - penetrationRate);
            effectiveDefense = Mathf.Max(0, effectiveDefense);

            // 基础伤害 = 攻击力 * 招式系数 * (1 - 减伤率)
            float damage = attack * damageMult * (1 - effectiveDefense / (attack + effectiveDefense));

            // 增伤乘区：伤害 * (1 + 增伤%)
            damage *= (1 + damageBonus);

            // 暴击判定
            bool isCritical = Random.value < critRate;
            if (isCritical)
            {
                damage *= (1 + critDmg);
            }

            // 失衡易伤乘区（敌人失衡时生效）
            if (isEnemyStunned)
            {
                float vulnerability = defender.GetAttr(2100);
                if (vulnerability > 0)
                {
                    damage *= vulnerability;
                }
            }

            // 保底伤害
            float finalDamage = Mathf.Max(1, damage);

            // 失衡值 = 冲击力 * 招式失衡系数
            float impact = attacker.GetAttr(400);
            float finalStun = Mathf.Max(1, impact * dazeMult);

            return new DamageResult
            {
                FinalDamage = finalDamage,
                FinalStun = finalStun,
                IsCritical = isCritical,
            };
        }
    }
}