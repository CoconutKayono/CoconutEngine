using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class AttackTipNotifySO : NotifySO
    {
        public bool canParry;

        public override void Notify(in ActionTimelineContext ctx)
        {
            GameEvent.Get<IActionTimelineEvents>().OnAttackTip(new AttackTipEvent
            {
                Context = ctx,
                CanParry = canParry,
            });
        }
    }
}
