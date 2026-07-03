#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace KayanoAction.Runtime
{
    /// <summary>
    /// Timeline 预览时登记当前帧激活的 BoxNotifyState，供 Scene 视图绘制线框。
    /// </summary>
    public static class BoxNotifyStateScenePreview
    {
        public readonly struct Entry
        {
            public Entry(BoxNotifyState box, GameObject owner)
            {
                Box = box;
                Owner = owner;
            }

            public BoxNotifyState Box { get; }

            public GameObject Owner { get; }
        }

        private static readonly Dictionary<int, Entry> Active = new(8);

        public static void SetActive(BoxNotifyState box, GameObject owner, bool active)
        {
            if (box == null)
            {
                return;
            }

            var id = box.GetInstanceID();
            if (active && owner != null)
            {
                Active[id] = new Entry(box, owner);
                return;
            }

            Active.Remove(id);
        }

        public static void Clear()
        {
            Active.Clear();
        }

        public static IEnumerable<Entry> GetActiveEntries()
        {
            return Active.Values;
        }
    }
}
#endif
