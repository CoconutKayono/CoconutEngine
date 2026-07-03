using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace KayanoAction.Runtime
{
    public static class ActionUnpacker
    {
        public static void Unpack(
            ActionTimeline action,
            out AnimationClip clip,
            List<ActionNotifyState> enterTrackNotifyStates,
            List<ActionNotifyState> notifyStates,
            List<ActionNotify> notifies)
        {
            clip = null;

            foreach (var track in action.GetOutputTracks())
            {
                MarkersToNotifies(track.GetMarkers(), notifies);

                if (track is AnimationTrack animTrack)
                {
                    foreach (var timelineClip in animTrack.GetClips())
                    {
                        if (clip == null)
                        {
                            clip = timelineClip.animationClip;
                        }
                    }
                }
                else if (track is EnterTrack)
                {
                    TimelineClipsToNotifyStates(track.GetClips(), enterTrackNotifyStates);
                }
                else
                {
                    TimelineClipsToNotifyStates(track.GetClips(), notifyStates);
                }
            }

            enterTrackNotifyStates.Sort((l, r) => l.start.CompareTo(r.start));
            notifyStates.Sort((l, r) => l.start.CompareTo(r.start));
            notifies.Sort((l, r) => l.time.CompareTo(r.time));
        }

        private static void MarkersToNotifies(IEnumerable<IMarker> markers, List<ActionNotify> notifies)
        {
            foreach (var marker in markers)
            {
                if (marker is ActionNotify notify)
                {
                    notifies.Add(notify);
                }
            }
        }

        private static void TimelineClipsToNotifyStates(
            IEnumerable<TimelineClip> clips,
            List<ActionNotifyState> notifyStates)
        {
            foreach (var clip in clips)
            {
                if (clip.asset is ActionNotifyState notifyState)
                {
                    notifyState.start = (float)clip.start;
                    notifyState.length = (float)clip.duration;
                    notifyStates.Add(notifyState);
                }
            }
        }
    }
}
