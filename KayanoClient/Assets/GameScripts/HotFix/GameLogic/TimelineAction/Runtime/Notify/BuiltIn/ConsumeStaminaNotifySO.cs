using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class ConsumeStaminaNotifySO : NotifySO
    {
        public float amount = 25f;

        public override void Notify(in ActionTimelineContext ctx)
        {
            GameEvent.Get<IActionTimelineEvents>().OnConsumeStamina(new ConsumeStaminaEvent
            {
                Context = ctx,
                Amount = amount,
            });
        }
    }
}
