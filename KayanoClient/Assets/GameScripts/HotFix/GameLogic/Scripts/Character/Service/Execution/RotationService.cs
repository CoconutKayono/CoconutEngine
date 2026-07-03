using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class RotationService : IRotationService
    {
        #region States
        private CharacterStore _characterStore;
        private float _rotationSpeed = 360f;
        #endregion

        #region Constructor
        public RotationService(CharacterStore characterStore)
        {
            _characterStore = characterStore;
            _rotationSpeed = characterStore.RotationSpeed;
        }
        #endregion

        #region Actions

        public void RotateTowards()
        {
            if (_characterStore?.Owner == null)
            {
                Log.Warning("[RotationService] 依赖为空，跳过旋转");
                return;
            }

            var currentAction = _characterStore.ChActionState.CurrentAction;
            if (currentAction == null || !currentAction.enableRotation)
            {
                return;
            }

            var state = _characterStore.ChState;
            Vector2 moveDir = state.MoveDirection;
            if (moveDir.sqrMagnitude < 0.001f) return;

            var owner = _characterStore.Owner;
            Vector3 worldDir = new Vector3(moveDir.x, 0, moveDir.y);
            var targetRotation = Quaternion.LookRotation(worldDir);

            owner.rotation = Quaternion.RotateTowards(
                owner.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }

        #endregion

        #region Dispose
        public void Dispose()
        {
            _characterStore = null;
        }
        #endregion
    }
}