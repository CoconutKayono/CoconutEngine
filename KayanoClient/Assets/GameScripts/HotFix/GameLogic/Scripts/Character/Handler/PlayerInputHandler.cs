using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class PlayerInputHandler : IActionInputHandler
    {
        #region States
        private CharacterStore _characterStore;
        private ChAttributeModel _characterAttributes;
        private ChActionModel _actionState;
        private int _instanceId;
        private InputBufferModel _buffer;
        #endregion

        #region Constructor
        public PlayerInputHandler(CharacterStore characterStore)
        {
            _characterStore = characterStore;
            _characterAttributes = characterStore.ChAttribute;
            _actionState = characterStore.ChActionState;
            _instanceId = characterStore.InstanceId;
            _buffer = InputModule.Instance.Buffer;
        }
        #endregion

        #region Methods
        public void ProcessInput()
        {
            if (!_characterAttributes.IsAlive) return;

            var input = InputModule.Instance.Input;

            while (_buffer.Pop(out var entry))
            {
                PublishBufferEntry(entry);
            }

            // 实时输入
            if (input.IsMovePress)
            {
                Vector2 worldDir = MoveHelper.GetWorldDirection(input.Move);
                GameEvent.Get<IActionIntentEvents>().OnMoveIntent(new()
                {
                    InstanceId = _instanceId,
                    Action = EIntentAction.Move,
                    Phase = EInputPhase.Press,
                    Direction = worldDir,
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

            Vector2 worldDir = MoveHelper.GetWorldDirection(intentEvent.Direction);
            intentEvent.Direction = worldDir;

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
                case EIntentAction.Move:
                    GameEvent.Get<IActionIntentEvents>().OnMoveIntent(intentEvent);
                    break;
                case EIntentAction.Sprint:
                    GameEvent.Get<IActionIntentEvents>().OnSprintIntent(intentEvent);
                    break;
            }
        }

        public void Dispose()
        {
            _characterStore = null;
            _characterAttributes = null;
            _actionState = null;
        }
        #endregion
    }
}