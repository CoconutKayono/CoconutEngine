using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class PlayerInputHandler : IActionInputHandler
    {
        #region States
        private CharacterModule _characterModule;
        private CharacterAttributeModel _characterAttributes;
        private ActionStateModel _actionState;
        private int _instanceId;
        private InputBufferModel _buffer;
        #endregion

        #region Constructor
        public PlayerInputHandler(CharacterModule characterModule)
        {
            _characterModule = characterModule;
            _characterAttributes = characterModule.CharacterAttributes;
            _actionState = characterModule.ActionState;
            _instanceId = characterModule.InstanceId;
            _buffer = InputModule.Instance.Buffer;
        }
        #endregion

        #region Methods
        public void ProcessInput()
        {
            if (!_characterAttributes.IsAlive) return;

            var input = InputModule.Instance.Input;

            // 离散按键：Down / Press / Up 统一从缓冲消费（支持提前输入 + 不同 Phase 触发）
            while (_buffer.Pop(out var entry))
            {
                PublishBufferEntry(entry);
            }

            // 连续输入：仅读实时快照
            if (input.IsMovePress)
            {
                GameEvent.Get<IActionIntentEvents>().OnMoveIntent(new()
                {
                    InstanceId = _instanceId,
                    Action = EIntentAction.Move,
                    Phase = EInputPhase.Press,
                    Direction = input.Move,
                });
            }

            if (input.IsSprinting)
            {
                GameEvent.Get<IActionIntentEvents>().OnSprintIntent(new()
                {
                    InstanceId = _instanceId,
                    Action = EIntentAction.Sprint,
                    Phase = EInputPhase.Press,
                    HoldTime = input.HoldTimes.Sprint
                });
            }

            // 保底意图：每帧都发，由决策层以低优先级仲裁
            GameEvent.Get<IActionIntentEvents>().OnNoneIntent(new()
            {
                InstanceId = _instanceId,
                Action = EIntentAction.None,
                Phase = EInputPhase.Press
            });
        }

        private void PublishBufferEntry(InputBufferModel.Entry entry)
        {
            var intentEvent = entry.ToIntentEvent(_instanceId);

            switch (entry.Action)
            {
                case EIntentAction.Attack:
                    GameEvent.Get<IActionIntentEvents>().OnAttackIntent(intentEvent);
                    break;
                case EIntentAction.SpecialAttack:
                    GameEvent.Get<IActionIntentEvents>().OnSpecialAttackIntent(intentEvent);
                    break;
                case EIntentAction.Dodge:
                    GameEvent.Get<IActionIntentEvents>().OnDodgeIntent(intentEvent);
                    break;
                case EIntentAction.Ultimate:
                    GameEvent.Get<IActionIntentEvents>().OnUltimateIntent(intentEvent);
                    break;
                case EIntentAction.Interact:
                    GameEvent.Get<IActionIntentEvents>().OnInteractIntent(intentEvent);
                    break;
                case EIntentAction.Chain:
                    GameEvent.Get<IActionIntentEvents>().OnChainIntent(intentEvent);
                    break;
            }
        }

        public void Dispose()
        {
            _characterModule = null;
            _characterAttributes = null;
            _actionState = null;
        }
        #endregion
    }
}
