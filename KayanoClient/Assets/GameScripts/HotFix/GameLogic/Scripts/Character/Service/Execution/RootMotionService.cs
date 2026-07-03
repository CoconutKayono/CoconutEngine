using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class RootMotionService : IRootMotionService
    {
        #region States
        private CharacterStore _characterStore;
        #endregion

        #region Constructor
        public RootMotionService(CharacterStore characterStore)
        {
            _characterStore = characterStore;
        }
        #endregion

        #region Actions

        public void OnAnimatorMove()
        {
            if (_characterStore == null)
            {
                Log.Warning("[RootMotionService] CharacterStore 为空，跳过根运动");
                return;
            }

            var animator = _characterStore.Animator;
            var ownerTransform = _characterStore.Owner;

            if (animator == null || ownerTransform == null)
            {
                Log.Warning($"[RootMotionService] Animator 或 Owner 为空，跳过根运动。InstanceId: {_characterStore.InstanceId}");
                return;
            }

            ownerTransform.position += animator.deltaPosition;
            ownerTransform.rotation *= animator.deltaRotation;
        }

        public void Dispose()
        {
            _characterStore = null;
        }

        #endregion
    }
}