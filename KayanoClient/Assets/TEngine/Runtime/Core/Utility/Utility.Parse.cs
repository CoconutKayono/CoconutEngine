using UnityEngine;

namespace TEngine
{
    /// <summary>
    /// TEngine 框架通用工具集。
    /// 提供数值归一化、反归一化、范围映射等基础数学工具方法。
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// 步进值转换工具类。
        /// 提供整数值与归一化浮点数之间的双向转换功能。
        /// </summary>
        /// <remarks>
        /// 适用场景：音量控制、进度条、分步滑块、等级映射等需要将离散整数值与连续浮点数互转的场景。
        /// </remarks>
        public static partial class Step
        {
            /// <summary>
            /// 将整数值归一化为 [0, 1] 范围的浮点数。
            /// </summary>
            /// <param name="value">待归一化的整数值，会被限制在 [0, max] 范围内。</param>
            /// <param name="max">最大值（分母），必须大于 0。</param>
            /// <returns>归一化后的浮点数，范围 [0, 1]。</returns>
            /// <example>
            /// <code>
            /// float result = Utility.Step.ToFloat(7, 10); // 返回 0.7f
            /// </code>
            /// </example>
            public static float ToFloat(int value, int max)
            {
                return Mathf.Clamp(value, 0, max) / (float)max;
            }

            /// <summary>
            /// 将整数值从 [min, max] 范围映射到 [0, 1] 范围的浮点数。
            /// </summary>
            /// <param name="value">待映射的整数值，会被限制在 [min, max] 范围内。</param>
            /// <param name="min">最小值。</param>
            /// <param name="max">最大值，必须大于 min。</param>
            /// <returns>映射后的浮点数，范围 [0, 1]。</returns>
            /// <example>
            /// <code>
            /// float result = Utility.Step.ToFloat(5, 2, 10); // 返回 (5-2)/(10-2) = 0.375f
            /// </code>
            /// </example>
            public static float ToFloat(int value, int min, int max)
            {
                return (Mathf.Clamp(value, min, max) - min) / (float)(max - min);
            }

            /// <summary>
            /// 将 [0, 1] 范围的浮点数转换为整数值（四舍五入）。
            /// </summary>
            /// <param name="normalized">归一化的浮点数，范围 [0, 1]。</param>
            /// <param name="max">最大值。</param>
            /// <returns>转换后的整数值，范围 [0, max]。</returns>
            /// <example>
            /// <code>
            /// int result = Utility.Step.ToInt(0.7f, 10); // 返回 7
            /// </code>
            /// </example>
            public static int ToInt(float normalized, int max)
            {
                return Mathf.Clamp(Mathf.RoundToInt(normalized * max), 0, max);
            }

            /// <summary>
            /// 将 [0, 1] 范围的浮点数映射到 [min, max] 范围的整数值（四舍五入）。
            /// </summary>
            /// <param name="normalized">归一化的浮点数，范围 [0, 1]。</param>
            /// <param name="min">目标最小值。</param>
            /// <param name="max">目标最大值，必须大于 min。</param>
            /// <returns>映射后的整数值，范围 [min, max]。</returns>
            /// <example>
            /// <code>
            /// int result = Utility.Step.ToInt(0.375f, 2, 10); // 返回 5
            /// </code>
            /// </example>
            public static int ToInt(float normalized, int min, int max)
            {
                return Mathf.Clamp(Mathf.RoundToInt(normalized * (max - min) + min), min, max);
            }
        }
    }
}