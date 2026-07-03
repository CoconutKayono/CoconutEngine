using KayanoAction.Runtime;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class HitDetectionService
    {
        #region States
        private int _monsterLayerMask;
        private Collider[] _hitColliders;
        #endregion

        public HitDetectionService(int monsterLayerMask, int maxHitCount = 50)
        {
            _monsterLayerMask = monsterLayerMask;
            _hitColliders = new Collider[maxHitCount];

            GameEvent.AddEventListener<BoxNotifyBeginEvent>(
                IActionTimelineEvents_Event.OnBoxNotifyBegin,
                OnBoxNotifyBegin);
        }

        #region Event Handlers

        public void OnBoxNotifyBegin(BoxNotifyBeginEvent e)
        {
            if (e.Context.Owner == null)
            {
                Debug.LogError("OnBoxNotifyBegin: Context or ActionTimeline is null");
                return;
            }

            // 根据碰撞盒形状执行不同的检测
            switch (e.Box.Shape)
            {
                case EBoxShape.Sphere:
                    OverlapSphere(e);
                    break;
                case EBoxShape.Box:
                    OverlapBox(e);
                    break;
                default:
                    Debug.LogWarning($"未支持的碰撞盒形状: {e.Box.Shape}");
                    break;
            }
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// 检查球形碰撞盒的重叠情况，并对每个命中的碰撞体执行 Hit 逻辑。
        /// </summary>
        private void OverlapSphere(BoxNotifyBeginEvent e)
        {
            Vector3 position = e.Context.Owner.position;
            Quaternion orientation = e.Context.Owner.rotation;

            Vector3 worldCenter = position + orientation * e.Box.Center;
            float radius = e.Box.Radius;

            int hitCount = Physics.OverlapSphereNonAlloc(worldCenter, radius, _hitColliders, _monsterLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                var hitCollider = _hitColliders[i];
                if (hitCollider != null)
                {
                    Hit(hitCollider, e);
                }
            }
        }

        /// <summary>
        /// 检查盒形碰撞盒的重叠情况，并对每个命中的碰撞体执行 Hit 逻辑。
        /// </summary>
        private void OverlapBox(BoxNotifyBeginEvent e)
        {
            Vector3 position = e.Context.Owner.position;
            Quaternion orientation = e.Context.Owner.rotation;

            Vector3 worldCenter = position + orientation * e.Box.Center;
            Vector3 halfExtents = e.Box.Size * 0.5f;

            int hitCount = Physics.OverlapBoxNonAlloc(
                worldCenter,
                halfExtents,
                _hitColliders,
                orientation,
                _monsterLayerMask
            );

            for (int i = 0; i < hitCount; i++)
            {
                var hitCollider = _hitColliders[i];
                if (hitCollider != null)
                {
                    Hit(hitCollider, e);
                }
            }
        }

        /// <summary>
        /// 发送命中请求事件，通知战斗系统处理伤害逻辑。
        /// </summary>
        /// <param name="hitCollider">命中对象</param>
        private void Hit(Collider hitCollider, BoxNotifyBeginEvent e)
        {
            // 1. 尝试从碰撞体上获取 IDamageable
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable == null)
            {
                // 如果没有 IDamageable，可能命中墙壁、地面等不可攻击物体
                return;
            }

            int attackerId = e.Context.InstanceId;
            int targetId = damageable.InstanceId;

            // 2. 计算位置和方向
            Vector3 attackerPos = e.Context.Owner.position;
            Vector3 targetPos = hitCollider.transform.position;
            Vector3 hitPoint = hitCollider.ClosestPoint(attackerPos);
            Vector3 hitDirection = (targetPos - attackerPos).normalized;

            // 3. 发送伤害请求事件
            GameEvent.Get<IBattleEvents>().OnHitRequest(new HitRequestEvent
            {
                AttackerId = attackerId,
                TargetId = targetId,
                HitPoint = hitPoint,
                HitDirection = hitDirection,
                BoxData = e.Box,
            });
        }
        #endregion

        public void Dispose()
        {
            GameEvent.RemoveEventListener<BoxNotifyBeginEvent>(
                IActionTimelineEvents_Event.OnBoxNotifyBegin,
                OnBoxNotifyBegin);
        }
    }
}