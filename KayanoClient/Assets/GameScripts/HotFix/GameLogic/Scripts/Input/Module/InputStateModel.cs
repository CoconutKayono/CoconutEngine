using System;
using UnityEngine;

namespace GameLogic
{
    [Flags]
    public enum EInputState : int
    {
        None = 0,

        AttackDown = 1 << 0,
        AttackPress = 1 << 1,
        AttackUp = 1 << 2,

        SpecialDown = 1 << 3,
        SpecialPress = 1 << 4,
        SpecialUp = 1 << 5,

        DodgeDown = 1 << 6,
        DodgePress = 1 << 7,
        DodgeUp = 1 << 8,

        UltimateDown = 1 << 9,
        UltimatePress = 1 << 10,
        UltimateUp = 1 << 11,

        ChainLDown = 1 << 12,
        ChainLPress = 1 << 13,
        ChainLUp = 1 << 14,

        ChainRDown = 1 << 15,
        ChainRPress = 1 << 16,
        ChainRUp = 1 << 17,

        SprintPress = 1 << 18,
        InteractDown = 1 << 19,
        InteractPress = 1 << 20,
        InteractUp = 1 << 21,
    }

    public struct InputHoldTimes
    {
        public float Move;
        public float Sprint;
        public float Interact;
        public float BaseAttack;
        public float SpecialAttack;
        public float Dodge;
        public float Ultimate;
        public float ChainComboL;
        public float ChainComboR;

        public void Reset()
        {
            Move = 0f;
            Sprint = 0f;
            Interact = 0f;
            BaseAttack = 0f;
            SpecialAttack = 0f;
            Dodge = 0f;
            Ultimate = 0f;
            ChainComboL = 0f;
            ChainComboR = 0f;
        }
    }

    /// <summary>
    /// 当前帧实时输入快照（Flags + 连续量 + HoldTime）。
    /// </summary>
    public struct InputStateModel
    {
        public EInputState States;
        public InputHoldTimes HoldTimes;
        public Vector2 Move;
        public Vector2 Look;
        public float Time;

        public bool HasState(EInputState state) => (States & state) != 0;

        public float GetHoldTime(EIntentAction action)
        {
            return action switch
            {
                EIntentAction.Move => HoldTimes.Move,
                EIntentAction.Sprint => HoldTimes.Sprint,
                EIntentAction.Interact => HoldTimes.Interact,
                EIntentAction.Attack => HoldTimes.BaseAttack,
                EIntentAction.SpecialAttack => HoldTimes.SpecialAttack,
                EIntentAction.Dodge => HoldTimes.Dodge,
                EIntentAction.Ultimate => HoldTimes.Ultimate,
                EIntentAction.Chain => Mathf.Max(HoldTimes.ChainComboL, HoldTimes.ChainComboR),
                _ => 0f,
            };
        }

        public bool IsAttackDown => HasState(EInputState.AttackDown);
        public bool IsAttackPress => HasState(EInputState.AttackPress);
        public bool IsAttackUp => HasState(EInputState.AttackUp);
        public bool IsSpecialDown => HasState(EInputState.SpecialDown);
        public bool IsSpecialPress => HasState(EInputState.SpecialPress);
        public bool IsSpecialUp => HasState(EInputState.SpecialUp);
        public bool IsDodgeDown => HasState(EInputState.DodgeDown);
        public bool IsDodgePress => HasState(EInputState.DodgePress);
        public bool IsDodgeUp => HasState(EInputState.DodgeUp);
        public bool IsUltimateDown => HasState(EInputState.UltimateDown);
        public bool IsUltimatePress => HasState(EInputState.UltimatePress);
        public bool IsUltimateUp => HasState(EInputState.UltimateUp);
        public bool IsChainLDown => HasState(EInputState.ChainLDown);
        public bool IsChainLPress => HasState(EInputState.ChainLPress);
        public bool IsChainLUp => HasState(EInputState.ChainLUp);
        public bool IsChainRDown => HasState(EInputState.ChainRDown);
        public bool IsChainRPress => HasState(EInputState.ChainRPress);
        public bool IsChainRUp => HasState(EInputState.ChainRUp);
        public bool IsSprinting => HasState(EInputState.SprintPress);
        public bool IsInteractDown => HasState(EInputState.InteractDown);
        public bool IsInteractPress => HasState(EInputState.InteractPress);
        public bool IsInteractUp => HasState(EInputState.InteractUp);
        public readonly bool IsMovePress => Move.sqrMagnitude > 0.01f;
    }
}
