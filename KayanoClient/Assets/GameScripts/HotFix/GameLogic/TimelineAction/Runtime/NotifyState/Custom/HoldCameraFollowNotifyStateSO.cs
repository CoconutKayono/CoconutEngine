using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class HoldCameraFollowNotifyStateSO : NotifyStateSO
    {
        public override void Enter(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnHoldCameraFollow(new HoldCameraFollowEvent
            {
                Context = ctx,
                Phase = HoldCameraFollowPhase.Begin,
                StateInstanceId = instanceId,
            });
        }

        public override void Exit(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnHoldCameraFollow(new HoldCameraFollowEvent
            {
                Context = ctx,
                Phase = HoldCameraFollowPhase.End,
                StateInstanceId = instanceId,
            });
        }
    }
}
