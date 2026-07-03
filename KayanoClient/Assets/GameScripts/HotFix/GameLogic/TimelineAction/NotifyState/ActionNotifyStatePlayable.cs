using UnityEngine;
using UnityEngine.Playables;

namespace KayanoAction.Runtime
{
    public sealed class ActionNotifyStatePlayable : PlayableBehaviour
    {
        private ActionNotifyState _state;
        private GameObject _owner;
        private GameObject _previewParticle;

        public void Bind(ActionNotifyState state, GameObject owner)
        {
            _state = state;
            _owner = owner;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return;
            }

            PreviewBegin();
#endif
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return;
            }

            if (_state is BoxNotifyState box)
            {
                BoxNotifyStateScenePreview.SetActive(box, _owner, false);
            }

            PreviewEnd();
#endif
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
#if UNITY_EDITOR
            if (Application.isPlaying || _state is not BoxNotifyState box)
            {
                return;
            }

            var active = info.weight > 0f && info.effectiveWeight > 0f;
            BoxNotifyStateScenePreview.SetActive(box, _owner, active);
#endif
        }

#if UNITY_EDITOR
        private void PreviewBegin()
        {
            switch (_state)
            {
                case PlayAudioNotifyState audio:
                    ActionTimelinePreviewRuntime.PreviewAudio(audio.EffectiveClips, audio.dontPlayProbability, _owner);
                    break;
                case PlayVoiceNotifyState voice:
                    ActionTimelinePreviewRuntime.PreviewVoice(voice.EffectiveClips, voice.dontPlayProbability, _owner);
                    break;
                case PlayParticleNotifyState particle:
                    _previewParticle = ActionTimelinePreviewRuntime.PreviewParticleBegin(
                        particle.prefab,
                        particle.localPosition,
                        particle.localRotation,
                        particle.localScale,
                        _owner);
                    break;
            }
        }

        private void PreviewEnd()
        {
            if (_previewParticle == null)
            {
                return;
            }

            ActionTimelinePreviewRuntime.PreviewParticleEnd(_previewParticle);
            _previewParticle = null;
        }
#endif
    }
}
