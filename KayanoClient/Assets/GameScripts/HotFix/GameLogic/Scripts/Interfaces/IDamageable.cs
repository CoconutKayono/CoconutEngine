using UnityEngine;

namespace GameLogic
{
    public interface IDamageable
    {
        int InstanceId { get; }
        void TakeDamage(DamageContext context);
        
    }

    /// <summary>
    /// 伤害信息 — 由 Hitbox 检测到命中后发送，由 HitService 处理
    /// </summary>
    public struct DamageContext
    {
        /// <summary>攻击者 InstanceId</summary>
        public int AttackerId;


        /// <summary>命中点（世界坐标）</summary>
        public Vector3 HitPoint;

        /// <summary>命中方向（用于击退方向）</summary>
        public Vector3 HitDirection;

        /// <summary>碰撞盒数据（用于获取额外参数）</summary>
        public BoxData BoxData;
    }
}
