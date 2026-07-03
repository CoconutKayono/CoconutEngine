using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class CameraShakeNotifySO : NotifySO
    {
        public float angle = 1f;
        public float speed = 1f;

        public override void Notify(in ActionTimelineContext ctx)
        {
            GameEvent.Get<IActionTimelineEvents>().OnCameraShake(new CameraShakeEvent
            {
                Context = ctx,
                Angle = angle,
                Speed = speed,
            });
        }
    }
}
