using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class AttackingNotifyStateSO : NotifyStateSO
    {
        public override void Enter(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnAttackingBegin(new TimelineActorEvent
            {
                Context = ctx,
                StateInstanceId = instanceId,
            });
        }

        public override void Exit(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnAttackingEnd(new TimelineActorEvent
            {
                Context = ctx,
                StateInstanceId = instanceId,
            });
        }
    }
}
