using GameConfig.Main;
using UnityEngine;

namespace GameLogic
{
    [System.Serializable]
    public class ChStateModel
    {
        #region 移动状态
        public bool IsMoving { get; set; }
        public bool IsSprinting { get; set; }
        public Vector2 MoveDirection { get; set; }
        public Vector2 InputDirection { get; set; }
        public Vector2 LastInputDirection { get; set; }
        #endregion
    }
}