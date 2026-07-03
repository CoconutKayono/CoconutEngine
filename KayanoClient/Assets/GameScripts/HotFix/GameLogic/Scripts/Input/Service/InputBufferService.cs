using TEngine;
using UnityEngine.InputSystem;

namespace GameLogic
{
    /// <summary>
    /// 输入缓冲服务 — 采集 Down / Press / Up 并写入 InputBufferModel。
    /// </summary>
    public class InputBufferService
    {
        public InputBufferModel Buffer { get; } = new();

        public void Tick(InputSystem_Actions.PlayerActions player, in InputStateModel state)
        {
            CaptureButton(player.Attack, EIntentAction.Attack, state);
            CaptureButton(player.SpecialAttack, EIntentAction.SpecialAttack, state);
            CaptureButton(player.Dodge, EIntentAction.Dodge, state);
            CaptureButton(player.Ultimate, EIntentAction.Ultimate, state);
            CaptureButton(player.Interact, EIntentAction.Interact, state);

            CaptureChain(player.ChainComboL, EIntentAction.Chain, EChainDirection.Previous, state);
            CaptureChain(player.ChainComboR, EIntentAction.Chain, EChainDirection.Next, state);
        }

        private void CaptureButton(InputAction action, EIntentAction intent, in InputStateModel state)
        {
            if (action.WasPerformedThisFrame())
            {
                Buffer.Push(InputBufferModel.Entry.FromSnapshot(intent, EInputPhase.Down, state));
            }

            if (action.IsPressed())
            {
                Buffer.Push(InputBufferModel.Entry.FromSnapshot(intent, EInputPhase.Press, state));
            }

            if (action.WasReleasedThisFrame())
            {
                Buffer.Push(InputBufferModel.Entry.FromSnapshot(intent, EInputPhase.Up, state));
            }
        }

        private void CaptureChain(
            InputAction action,
            EIntentAction intent,
            EChainDirection chainDir,
            in InputStateModel state)
        {
            if (action.WasPerformedThisFrame())
            {
                Buffer.Push(InputBufferModel.Entry.FromSnapshot(intent, EInputPhase.Down, state, chainDir));
            }

            if (action.IsPressed())
            {
                Buffer.Push(InputBufferModel.Entry.FromSnapshot(intent, EInputPhase.Press, state, chainDir));
            }

            if (action.WasReleasedThisFrame())
            {
                Buffer.Push(InputBufferModel.Entry.FromSnapshot(intent, EInputPhase.Up, state, chainDir));
            }
        }
    }
}
