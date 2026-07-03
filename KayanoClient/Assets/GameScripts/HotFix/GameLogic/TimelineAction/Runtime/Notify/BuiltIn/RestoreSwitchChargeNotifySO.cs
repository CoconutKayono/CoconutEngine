using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class RestoreSwitchChargeNotifySO : NotifySO
    {
        public int amount = 1;

        public override void Notify(in ActionTimelineContext ctx)
        {
            GameEvent.Get<IActionTimelineEvents>().OnRestoreSwitchCharge(new RestoreSwitchChargeEvent
            {
                Context = ctx,
                Amount = amount,
            });
        }
    }
}
