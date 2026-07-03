using GameLogic;
using System.ComponentModel;
using UnityEngine;

namespace KayanoAction.Runtime
{
    [DisplayName("消耗体力")]
    public sealed class ConsumeStaminaNotify : ActionNotify
    {
        public float amount = 25f;
    }
}
