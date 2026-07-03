#if UNITY_EDITOR
using UnityEngine;

namespace KayanoAction.Runtime
{
    /// <summary>Timeline 编辑器预览（音频/粒子）；仅 Editor 下调试 Timeline 时执行。</summary>
    public static class ActionTimelinePreviewRuntime
    {
        private static GameObject _particleRoot;
        private static GameObject _activeAudioPreview;

        public static void PreviewAudio(AudioClip[] clips, float dontPlayProbability, GameObject owner)
        {
            if (clips == null || clips.Length == 0)
            {
                return;
            }

            if (dontPlayProbability > 0f && Random.value < dontPlayProbability)
            {
                return;
            }

            var clip = clips[0];
            for (var i = 0; i < clips.Length; i++)
            {
                if (clips[i] != null)
                {
                    clip = clips[i];
                    break;
                }
            }

            if (clip == null)
            {
                return;
            }

            var position = owner != null ? owner.transform.position : Vector3.zero;
            if (Application.isPlaying)
            {
                AudioSource.PlayClipAtPoint(clip, position);
                return;
            }

            StopActiveAudioPreview();

            var go = new GameObject("[TimelinePreviewAudio]");
            go.hideFlags = HideFlags.HideAndDontSave;
            go.transform.position = position;

            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.playOnAwake = false;

            go.AddComponent<TimelinePreviewAudioPlayer>();
            _activeAudioPreview = go;
        }

        public static void PreviewVoice(AudioClip[] clips, float dontPlayProbability, GameObject owner)
        {
            PreviewAudio(clips, dontPlayProbability, owner);
        }

        private static void StopActiveAudioPreview()
        {
            if (_activeAudioPreview == null)
            {
                return;
            }

            Object.DestroyImmediate(_activeAudioPreview);
            _activeAudioPreview = null;
        }

        public static GameObject PreviewParticleBegin(
            GameObject prefab,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale,
            GameObject owner)
        {
            if (prefab == null)
            {
                return null;
            }

            EnsureParticleRoot();
            var parent = owner != null ? owner.transform : _particleRoot.transform;
            var instance = Object.Instantiate(prefab, parent);
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = localRotation;
            instance.transform.localScale = localScale;
            return instance;
        }

        public static void PreviewParticleEnd(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            Object.DestroyImmediate(instance);
        }

        public static void ClearAllPreviewParticles()
        {
            if (_particleRoot == null)
            {
                return;
            }

            for (var i = _particleRoot.transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(_particleRoot.transform.GetChild(i).gameObject);
            }
        }

        private static void EnsureParticleRoot()
        {
            if (_particleRoot != null)
            {
                return;
            }

            _particleRoot = new GameObject("[TimelinePreviewParticle]");
            _particleRoot.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    [ExecuteAlways]
    internal sealed class TimelinePreviewAudioPlayer : MonoBehaviour
    {
        private AudioSource _source;
        private double _endTime;

        private void OnEnable()
        {
            _source = GetComponent<AudioSource>();
            if (_source == null || _source.clip == null || Application.isPlaying)
            {
                return;
            }

            _source.Play();
            var duration = _source.clip.length / Mathf.Max(_source.pitch, 0.01f);
            _endTime = AudioSettings.dspTime + duration + 0.05;
        }

        private void Update()
        {
            if (Application.isPlaying || _source == null)
            {
                return;
            }

            if (AudioSettings.dspTime >= _endTime)
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}
#endif
