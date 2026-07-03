using UnityEngine;

namespace GameLogic
{
    public enum EIntentAction
    {
        None = 0,

        // 移动类 (100-199)
        Move = 100,
        Sprint = 110,
        Dodge = 120,

        // 攻击类 (200-299)
        Attack = 200,
        SpecialAttack = 210,
        Ultimate = 220,

        // 交互类 (300-399)
        Interact = 300,

        // 队伍类 (400-499)
        Chain = 400,
    }
}
