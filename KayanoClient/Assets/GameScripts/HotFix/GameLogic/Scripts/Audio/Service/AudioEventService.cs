using NUnit.Framework;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class AudioEventService
    {
        #region Constructor
        public AudioEventService()
        {
            GameEvent.AddEventListener<PlayAudioEvent>(IActionTimelineEvents_Event.OnPlayAudio, OnPlayAudio);
            GameEvent.AddEventListener<PlayVoiceEvent>(IActionTimelineEvents_Event.OnPlayVoice, OnPlayVoice);
        }
        #endregion

        /// <summary>
        /// 播放音频（事件驱动）
        /// </summary>
        /// <remarks>
        /// 从路径列表中随机选取一个音频进行播放，并支持“不播放”概率。
        /// 当前为 2D 播放方式，未对位置和空间音效做处理。
        /// Todo:改进为支持 3D 空间音效，可根据事件上下文传入播放位置、空间混合比等参数。
        /// </remarks>
        /// <param name="evt">音频播放事件数据</param>
        private void OnPlayAudio(PlayAudioEvent evt)
        {
            if (evt.Paths == null || evt.Paths.Length == 0)
            {
                Log.Warning($"[Audio] 播放音频失败：路径列表为空");
                return;
            }

            if (Random.Range(0f, 1f) < evt.DontPlayProbability)
            {
                return;
            }

            int index = Random.Range(0, evt.Paths.Length);
            string selectedPath = evt.Paths[index];

            GameModule.Audio.Play(evt.audioType, selectedPath);
        }

        /// <summary>
        /// 播放语音（事件驱动）
        /// </summary>
        /// <remarks>
        /// 从路径列表中随机选取一条语音进行播放，并支持“不播放”概率。
        /// 当前为 2D 播放方式，未对位置和空间音效做处理。
        /// Todo:改进为支持 3D 空间音效，可根据事件上下文传入播放位置、空间混合比等参数。
        /// </remarks>
        /// <param name="evt">语音播放事件数据</param>
        private void OnPlayVoice(PlayVoiceEvent evt)
        {
            if (evt.Paths == null || evt.Paths.Length == 0)
            {
                Log.Warning($"[Audio] 播放语音失败：路径列表为空");
                return;
            }

            if (Random.Range(0f, 1f) < evt.DontPlayProbability)
            {
                return;
            }

            int index = Random.Range(0, evt.Paths.Length);
            string selectedPath = evt.Paths[index];

            GameModule.Audio.Play(evt.audioType, selectedPath);
        }

        public void Dispose()
        {
            GameEvent.RemoveEventListener<PlayAudioEvent>(IActionTimelineEvents_Event.OnPlayAudio, OnPlayAudio);
            GameEvent.RemoveEventListener<PlayVoiceEvent>(IActionTimelineEvents_Event.OnPlayVoice, OnPlayVoice);
        }
    }
}
