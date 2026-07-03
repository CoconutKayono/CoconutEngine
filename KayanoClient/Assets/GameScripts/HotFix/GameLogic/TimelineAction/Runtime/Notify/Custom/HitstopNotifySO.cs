using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class HitstopNotifySO : NotifySO
    {
        public float duration = 0.05f;
        public float animationSpeed;

        public override void Notify(in ActionTimelineContext ctx)
        {
            GameEvent.Get<IActionTimelineEvents>().OnHitstop(new HitstopEvent
            {
                Context = ctx,
                Duration = duration,
                AnimationSpeed = animationSpeed,
            });
        }
    }
}
