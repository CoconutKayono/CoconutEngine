using UnityEngine;

namespace KayanoAction.Runtime
{
    /// <summary>
    /// 音频片段长度工具类
    /// 用于从 AudioClip 字段解析主音频时长，支持轨道拖放、Inspector 与 Clip 长度同步
    /// </summary>
    public static class AudioClipLengthUtility
    {
        /// <summary>
        /// 获取有效的音频片段数组
        /// </summary>
        /// <param name="clip">单个音频片段</param>
        /// <param name="clips">音频片段数组</param>
        /// <returns>
        /// 优先返回 clips 数组（若非空）；
        /// 若 clips 为空但 clip 不为空，返回包含 clip 的单元素数组；
        /// 若都为空，返回 null
        /// </returns>
        public static AudioClip[] GetEffectiveClips(AudioClip clip, AudioClip[] clips)
        {
            if (clips != null && clips.Length > 0)
            {
                return clips;
            }

            if (clip != null)
            {
                return new[] { clip };
            }

            return null;
        }

        /// <summary>
        /// 尝试获取主音频片段的时长（通过 clip 和 clips 参数）
        /// </summary>
        /// <param name="clip">单个音频片段</param>
        /// <param name="clips">音频片段数组</param>
        /// <param name="length">输出音频时长（秒）</param>
        /// <returns>是否成功获取到有效的音频时长</returns>
        public static bool TryGetPrimaryLength(AudioClip clip, AudioClip[] clips, out float length)
        {
            return TryGetPrimaryLength(GetEffectiveClips(clip, clips), out length);
        }

        /// <summary>
        /// 尝试获取主音频片段的时长（通过音频片段数组）
        /// </summary>
        /// <param name="clips">音频片段数组</param>
        /// <param name="length">输出音频时长（秒）</param>
        /// <returns>是否成功获取到有效的音频时长</returns>
        public static bool TryGetPrimaryLength(AudioClip[] clips, out float length)
        {
            length = 0f;

            if (clips == null)
            {
                return false;
            }

            for (var i = 0; i < clips.Length; i++)
            {
                var audioClip = clips[i];
                if (audioClip == null)
                {
                    continue;
                }

                length = audioClip.length;
                return length > 0f;
            }

            return false;
        }

        /// <summary>
        /// 同步 clip 与 clips：clips 数组为权威数据源（支持多 clip 随机）；
        /// clip 仅用于 Timeline 拖放兼容与主时长读取。
        /// </summary>
        /// <param name="clip">单个音频片段（引用传递）</param>
        /// <param name="clips">音频片段数组（引用传递）</param>
        public static void SyncPrimaryClip(ref AudioClip clip, ref AudioClip[] clips)
        {
            if (clips != null)
            {
                if (clips.Length == 0)
                {
                    clip = null;
                    clips = null;
                    return;
                }

                AudioClip primary = null;
                for (var i = 0; i < clips.Length; i++)
                {
                    if (clips[i] != null)
                    {
                        primary = clips[i];
                        break;
                    }
                }

                clip = primary;
                return;
            }

            if (clip != null)
            {
                clips = new[] { clip };
            }
        }

        /// <summary>
        /// 将单个 AudioClip 同时写入 clip 与 clips（Timeline 拖放、From Audio Clip 共用）。
        /// </summary>
        public static void ApplySourceClip(AudioClip source, ref AudioClip clip, ref AudioClip[] clips)
        {
            if (source == null)
            {
                return;
            }

            clip = source;
            clips = new[] { source };
        }
    }
}




