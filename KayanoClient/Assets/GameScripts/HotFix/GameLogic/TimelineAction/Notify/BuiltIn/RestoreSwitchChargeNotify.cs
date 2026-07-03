using System.ComponentModel;

namespace KayanoAction.Runtime
{
    [DisplayName("回复支援点")]
    public sealed class RestoreSwitchChargeNotify : ActionNotify
    {
        public int amount = 1;
    }
}
