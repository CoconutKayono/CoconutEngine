using GameLogic;
using TEngine;
using UnityEngine;

namespace KayanoAction.Runtime
{
    public sealed class PlayParticleNotifySO : NotifySO
    {
        public GameObject prefab;
        public Vector3 localPosition;
        public Quaternion localRotation = Quaternion.identity;
        public Vector3 localScale = Vector3.one;

        public override void Notify(in ActionTimelineContext ctx)
        {
            GameEvent.Get<IActionTimelineEvents>().OnPlayParticle(new PlayParticleEvent
            {
                Context = ctx,
                Prefab = prefab,
                LocalPosition = localPosition,
                LocalRotation = localRotation,
                LocalScale = localScale,
                IsStateEnd = false,
            });
        }
    }
}
