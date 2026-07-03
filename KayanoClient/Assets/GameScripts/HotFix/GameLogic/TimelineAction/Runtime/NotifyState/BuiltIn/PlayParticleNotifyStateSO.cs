using GameLogic;
using TEngine;
using UnityEngine;

namespace KayanoAction.Runtime
{
    public sealed class PlayParticleNotifyStateSO : NotifyStateSO
    {
        public GameObject prefab;
        public Vector3 localPosition;
        public Quaternion localRotation = Quaternion.identity;
        public Vector3 localScale = Vector3.one;

        public override void Enter(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnPlayParticle(new PlayParticleEvent
            {
                Context = ctx,
                Prefab = prefab,
                LocalPosition = localPosition,
                LocalRotation = localRotation,
                LocalScale = localScale,
                StateInstanceId = instanceId,
                IsStateEnd = false,
            });
        }

        public override void Exit(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnPlayParticle(new PlayParticleEvent
            {
                Context = ctx,
                StateInstanceId = instanceId,
                IsStateEnd = true,
            });
        }
    }
}
