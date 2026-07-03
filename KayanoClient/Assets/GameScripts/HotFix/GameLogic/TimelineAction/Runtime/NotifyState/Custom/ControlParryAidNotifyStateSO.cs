using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class ControlParryAidNotifyStateSO : NotifyStateSO
    {
        public override void Enter(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnParryAidBegin(new TimelineActorEvent
            {
                Context = ctx,
                StateInstanceId = instanceId,
            });
        }

        public override void Exit(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnParryAidEnd(new TimelineActorEvent
            {
                Context = ctx,
                StateInstanceId = instanceId,
            });
        }
    }
}
