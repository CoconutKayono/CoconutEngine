using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class RootMotionService : IRootMotionService
    {
        #region States
        private CharacterModule _characterModule;
        #endregion

        #region Constructor
        public RootMotionService(CharacterModule characterModule)
        {
            _characterModule = characterModule;
        }
        #endregion

        #region Actions

        public void OnAnimatorMove()
        {
            if (_characterModule == null)
            {
                Log.Warning("[RootMotionService] CharacterModule 为空，跳过根运动");
                return;
            }

            var animator = _characterModule.Animator;
            var ownerTransform = _characterModule.Owner;

            if (animator == null || ownerTransform == null)
            {
                Log.Warning($"[RootMotionService] Animator 或 Owner 为空，跳过根运动。InstanceId: {_characterModule.InstanceId}");
                return;
            }

            ownerTransform.position += animator.deltaPosition;
            ownerTransform.rotation *= animator.deltaRotation;
        }

        public void Dispose()
        {
            _characterModule = null;
        }

        #endregion
    }
}