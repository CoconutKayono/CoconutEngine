using TEngine;

namespace GameLogic
{
    /// <summary>
    /// 音频系统 — 管理音频事件服务的生命周期
    /// </summary>
    public class AudioSystem : Singleton<AudioSystem>
    {
        private AudioEventService _audioEventService;

        protected override void OnInit()
        {
            base.OnInit();
            _audioEventService = new AudioEventService();
        }

        protected override void OnRelease()
        {
            _audioEventService.Dispose();
            _audioEventService = null;
            base.OnRelease();
        }
    }
}