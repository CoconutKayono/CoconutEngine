#if UNITY_EDITOR

using KayanoAction.Runtime;
using UnityEditor;
using UnityEngine;

namespace TEngine.Editor.TimelineAction
{
    /// <summary>
    /// 碰撞盒预览绘制工具
    /// </summary>
    [InitializeOnLoad]
    internal static class BoxNotifyStateScenePreviewDrawer
    {
        static BoxNotifyStateScenePreviewDrawer()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.playModeStateChanged += _ =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    BoxNotifyStateScenePreview.Clear();
                }
            };
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (Application.isPlaying)
            {
                return;
            }

            foreach (var entry in BoxNotifyStateScenePreview.GetActiveEntries())
            {
                if (entry.Box == null || entry.Owner == null)
                {
                    continue;
                }

                BoxNotifyStateDrawUtility.DrawWire(entry.Box, entry.Owner.transform);
            }
        }
    }

    internal static class BoxNotifyStateDrawUtility
    {
        public static void DrawWire(BoxNotifyState box, Transform parent)
        {
            if (box == null || parent == null)
            {
                return;
            }

            var color = box.boxType == EBoxType.HitBox
                ? new Color(1f, 0.25f, 0.25f, 0.95f)
                : new Color(0.25f, 0.85f, 1f, 0.95f);

            using (new HandlesColorScope(color))
            {
                switch (box.boxShape)
                {
                    case EBoxShape.Sphere:
                    {
                        var worldCenter = parent.TransformPoint(box.center);
                        Handles.DrawWireDisc(worldCenter, Vector3.up, box.radius);
                        Handles.DrawWireDisc(worldCenter, Vector3.right, box.radius);
                        Handles.DrawWireDisc(worldCenter, Vector3.forward, box.radius);
                        break;
                    }

                    case EBoxShape.Box:
                    {
                        var prevMatrix = Handles.matrix;
                        Handles.matrix = parent.localToWorldMatrix;
                        Handles.DrawWireCube(box.center, box.size);
                        Handles.matrix = prevMatrix;
                        break;
                    }
                }
            }

            var labelPos = parent.TransformPoint(box.center);
            Handles.Label(labelPos, $"{box.boxType} hitId={box.hitId}");
        }

        private readonly struct HandlesColorScope : System.IDisposable
        {
            private readonly Color _previous;

            public HandlesColorScope(Color color)
            {
                _previous = Handles.color;
                Handles.color = color;
            }

            public void Dispose()
            {
                Handles.color = _previous;
            }
        }
    }
}

#endif
