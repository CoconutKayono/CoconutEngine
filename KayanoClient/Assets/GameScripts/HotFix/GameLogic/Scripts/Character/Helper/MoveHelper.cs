using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 移动辅助工具
    /// </summary>
    public static class MoveHelper
    {
        /// <summary>
        /// 将原始输入方向（屏幕空间）转换为世界空间方向
        /// </summary>
        public static Vector2 GetWorldDirection(Vector2 input)
        {
            if (input.sqrMagnitude < 0.001f) return Vector2.zero;

            var camera = Camera.main;
            if (camera == null)
            {
                Log.Warning("[MoveHelper] Camera.main 为空");
                return Vector2.zero;
            }

            Transform cam = camera.transform;
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 world = (forward * input.y + right * input.x).normalized;
            return new Vector2(world.x, world.z);
        }
    }
}