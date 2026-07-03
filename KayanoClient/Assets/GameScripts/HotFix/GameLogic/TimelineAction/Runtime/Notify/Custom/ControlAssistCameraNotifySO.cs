using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class ControlAssistCameraNotifySO : NotifySO
    {
        public bool enter;

        public override void Notify(in ActionTimelineContext ctx)
        {
            GameEvent.Get<IActionTimelineEvents>().OnAssistCamera(new AssistCameraEvent
            {
                Context = ctx,
                Enter = enter,
            });
        }
    }
}
