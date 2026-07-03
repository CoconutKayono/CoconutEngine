using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class ControlDodgeNotifyStateSO : NotifyStateSO
    {
        public override void Enter(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnDodgeBegin(new TimelineActorEvent
            {
                Context = ctx,
                StateInstanceId = instanceId,
            });
        }

        public override void Exit(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnDodgeEnd(new TimelineActorEvent
            {
                Context = ctx,
                StateInstanceId = instanceId,
            });
        }
    }
}
