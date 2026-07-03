using TEngine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameLogic
{
    /// <summary>
    /// 实时输入服务 — 每帧采样 Unity Input System，写入 InputStateModel。
    /// </summary>
    public class InputStateService
    {
        public void Tick(InputSystem_Actions.PlayerActions player, ref InputStateModel state)
        {
            state.States = EInputState.None;
            state.Time = GameTime.time;

            state.Move = player.Move.ReadValue<Vector2>();
            state.Look = player.Look.ReadValue<Vector2>();

            SetInputState(ref state.States,
                EInputState.AttackDown, EInputState.AttackPress, EInputState.AttackUp,
                player.Attack);

            SetInputState(ref state.States,
                EInputState.SpecialDown, EInputState.SpecialPress, EInputState.SpecialUp,
                player.SpecialAttack);

            SetInputState(ref state.States,
                EInputState.DodgeDown, EInputState.DodgePress, EInputState.DodgeUp,
                player.Dodge);

            SetInputState(ref state.States,
                EInputState.UltimateDown, EInputState.UltimatePress, EInputState.UltimateUp,
                player.Ultimate);

            SetInputState(ref state.States,
                EInputState.ChainLDown, EInputState.ChainLPress, EInputState.ChainLUp,
                player.ChainComboL);

            SetInputState(ref state.States,
                EInputState.ChainRDown, EInputState.ChainRPress, EInputState.ChainRUp,
                player.ChainComboR);

            if (player.Sprint.IsPressed())
            {
                state.States |= EInputState.SprintPress;
            }

            SetInputState(ref state.States,
                EInputState.InteractDown, EInputState.InteractPress, EInputState.InteractUp,
                player.Interact);

            UpdateHoldTimes(player, ref state);
        }

        private static void UpdateHoldTimes(InputSystem_Actions.PlayerActions player, ref InputStateModel state)
        {
            float delta = GameTime.deltaTime;

            state.HoldTimes.BaseAttack = player.Attack.IsPressed()
                ? state.HoldTimes.BaseAttack + delta : 0f;

            state.HoldTimes.SpecialAttack = player.SpecialAttack.IsPressed()
                ? state.HoldTimes.SpecialAttack + delta : 0f;

            state.HoldTimes.Dodge = player.Dodge.IsPressed()
                ? state.HoldTimes.Dodge + delta : 0f;

            state.HoldTimes.Ultimate = player.Ultimate.IsPressed()
                ? state.HoldTimes.Ultimate + delta : 0f;

            state.HoldTimes.ChainComboL = player.ChainComboL.IsPressed()
                ? state.HoldTimes.ChainComboL + delta : 0f;

            state.HoldTimes.ChainComboR = player.ChainComboR.IsPressed()
                ? state.HoldTimes.ChainComboR + delta : 0f;

            state.HoldTimes.Sprint = player.Sprint.IsPressed()
                ? state.HoldTimes.Sprint + delta : 0f;

            state.HoldTimes.Interact = player.Interact.IsPressed()
                ? state.HoldTimes.Interact + delta : 0f;

            state.HoldTimes.Move = state.Move.sqrMagnitude > 0.01f
                ? state.HoldTimes.Move + delta : 0f;
        }

        private static void SetInputState(
            ref EInputState state,
            EInputState downState,
            EInputState pressState,
            EInputState upState,
            InputAction action)
        {
            if (action.WasPerformedThisFrame())
            {
                state |= downState;
            }

            if (action.IsPressed())
            {
                state |= pressState;
            }

            if (action.WasReleasedThisFrame())
            {
                state |= upState;
            }
        }
    }
}
