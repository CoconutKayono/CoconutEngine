using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 战斗相关事件接口
    /// </summary>
    [EventInterface(EEventGroup.GroupLogic)]
    public interface IBattleEvents
    {
        /// <summary>
        /// 命中请求 — 由 BattleHitboxService 发送，HitService 订阅
        /// 传递物理检测到的原始命中数据，伤害尚未计算
        /// </summary>
        void OnHitRequest(HitRequestEvent evt);

        /// <summary>
        /// 命中结算 — 由 HitService 发送，各表现层订阅
        /// 传递已计算完成的命中结果
        /// </summary>
        void OnHitResult(HitResultEvent evt);

        /// <summary>
        /// 单位死亡 — 由 HitService 发送，各表现层/系统订阅
        /// 传递死亡单位和击杀者的信息
        /// </summary>
        void OnUnitDeath(UnitDeathEvent evt);
    }

    /// <summary>
    /// 命中请求事件 — 由 BattleHitboxService 发送，HitService 订阅
    /// 职责：传递物理检测到的命中数据，伤害尚未计算
    /// </summary>
    public struct HitRequestEvent
    {
        public int AttackerId;
        public int TargetId;
        public Vector3 HitPoint;
        public Vector3 HitDirection;
        public BoxData BoxData;
    }

    /// <summary>
    /// 命中结算事件 — 由 HitService 发送，各表现层订阅
    /// 职责：传递已计算完成的命中结果
    /// </summary>
    public struct HitResultEvent
    {
        public int AttackerId;
        public int TargetId;
        public float Damage;
        public float StunValue;
        public bool IsCritical;
        public bool IsKilled;
        public Vector3 HitPoint;
        public Vector3 HitDirection;
    }

    /// <summary>
    /// 单位死亡事件 — 由 HitService 在检测到击杀时发送
    /// 职责：通知各系统单位已死亡，触发死亡表现、任务进度、经验结算等
    /// </summary>
    public struct UnitDeathEvent
    {
        public int TargetId;      // 死亡单位 InstanceId
        public int AttackerId;    // 击杀者 InstanceId
        public Vector3 DeathPoint; // 死亡位置
        public EUnitType UnitType; // 单位类型（Player/Enemy/Summon）
    }
}