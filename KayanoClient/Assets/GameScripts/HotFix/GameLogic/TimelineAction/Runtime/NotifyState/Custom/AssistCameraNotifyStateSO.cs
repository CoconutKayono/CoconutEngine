using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class AssistCameraNotifyStateSO : NotifyStateSO
    {
        public override void Enter(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnAssistCamera(new AssistCameraEvent
            {
                Context = ctx,
                Enter = true,
                StateInstanceId = instanceId,
            });
        }

        public override void Exit(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnAssistCamera(new AssistCameraEvent
            {
                Context = ctx,
                Enter = false,
                StateInstanceId = instanceId,
            });
        }
    }
}
