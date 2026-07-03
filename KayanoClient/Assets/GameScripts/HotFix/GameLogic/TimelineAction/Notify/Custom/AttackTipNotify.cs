using System.ComponentModel;

namespace KayanoAction.Runtime
{
    [DisplayName("攻击提示")]
    public sealed class AttackTipNotify : ActionNotify
    {
        public bool canParry;
    }
}
