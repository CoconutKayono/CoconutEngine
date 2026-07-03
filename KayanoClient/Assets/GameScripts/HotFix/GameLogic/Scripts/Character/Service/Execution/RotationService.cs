using Fantasy;
using UnityEngine;

namespace GameLogic
{
    public class RotationService : IRotationService
    {
        #region States
        private CharacterModule _characterModule;
        #endregion
        public RotationService(CharacterModule characterModule, InputModule inputModule)
        {
            _characterModule = characterModule;
        }

        public void RotateTowards(Camera camera)
        {
            if (camera == null || _characterModule?.Owner == null)
            {
                Log.Warning("[RotationService] 依赖为空");
                return;
            }

            Vector2 input = InputModule.Instance.Input.Move;
            if (input.sqrMagnitude < 0.001f) return;

            Transform cam = camera.transform;
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 worldDir = (forward * input.y) + (right * input.x);

            var targetRotation = Quaternion.LookRotation(worldDir);
            var rotateSpeed = _characterModule.CharacterState.RotationSpeed;

            _characterModule.Owner.rotation = Quaternion.RotateTowards(
            _characterModule.Owner.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime 
            );
        }
    }
}
