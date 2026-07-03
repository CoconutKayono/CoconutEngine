using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 移动方向服务 — 负责计算世界空间方向并更新 ChStateModel
    /// </summary>
    public class MoveDirectionService
    {
        #region States
        private CharacterStore _characterStore;
        private ChStateModel _state;
        #endregion

        #region Constructor
        public MoveDirectionService(CharacterStore characterStore)
        {
            _characterStore = characterStore;
            _state = characterStore.ChState;
        }
        #endregion

        #region Actions

        /// <summary>
        /// 更新移动方向状态
        /// </summary>
        public void UpdateDirection(Vector2 input)
        {
            if (_state == null) return;

            Vector2 worldDir = MoveHelper.GetWorldDirection(input);

            _state.InputDirection = worldDir;
            _state.IsMoving = worldDir.sqrMagnitude > 0.01f;

            if (_state.IsMoving)
            {
                _state.LastInputDirection = _state.MoveDirection;
                _state.MoveDirection = worldDir;
            }
        }

        #endregion

        #region Dispose
        public void Dispose()
        {
            _characterStore = null;
            _state = null;
        }
        #endregion
    }
}