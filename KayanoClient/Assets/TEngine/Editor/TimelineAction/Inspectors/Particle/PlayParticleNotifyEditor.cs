#if UNITY_EDITOR

using KayanoAction.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TEngine.Editor.TimelineAction
{
    /// <summary>
    /// PlayParticleNotify（Timeline Marker）的自定义 Inspector 编辑器
    /// 提供粒子特效的预览功能，方便策划/美术在编辑器中查看 Marker 触发的特效效果
    /// </summary>
    [CustomEditor(typeof(PlayParticleNotify))]
    public sealed class PlayParticleNotifyEditor : UnityEditor.Editor
    {
        private GameObject _previewInstance;  // 当前预览的粒子实例
        private Button _stopButton;           // 停止预览按钮的引用

        /// <summary>
        /// 编辑器禁用时自动清理预览实例，避免场景残留
        /// </summary>
        private void OnDisable()
        {
            StopPreview();
        }

        /// <summary>
        /// 构建自定义 Inspector UI（使用 UI Toolkit）
        /// </summary>
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            // 添加默认的属性字段（prefab、localPosition、localRotation 等）
            root.Add(ActionNotifyInspectorElements.BuildDefaultInspector(serializedObject));

            // ============================================
            // 预览控制按钮
            // ============================================

            // 预览播放按钮
            var previewButton = new Button(PreviewSelected) { text = "预览播放" };
            previewButton.style.flexGrow = 1f;

            // 停止预览按钮（初始禁用）
            _stopButton = new Button(StopPreview) { text = "停止预览" };
            _stopButton.style.flexGrow = 1f;
            _stopButton.style.marginLeft = 4f;
            _stopButton.SetEnabled(false);

            // 将按钮放入水平行布局
            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    marginTop = 8f,
                },
            };
            row.Add(previewButton);
            row.Add(_stopButton);
            root.Add(row);

            root.Add(ActionNotifyInspectorElements.BuildHelpBox(
                "点击「预览播放」在选中对象上生成特效。"));

            return root;
        }

        private void PreviewSelected()
        {
            StopPreview();

            var notify = (PlayParticleNotify)target;
            var owner = Selection.activeGameObject;

            _previewInstance = ActionTimelinePreviewRuntime.PreviewParticleBegin(
                notify.prefab,           // 粒子预制体
                notify.localPosition,    // 本地位置偏移
                notify.localRotation,    // 本地旋转
                notify.localScale,       // 本地缩放
                owner);                  // 父对象

            // 预览成功则启用停止按钮
            _stopButton?.SetEnabled(_previewInstance != null);
        }

        /// <summary>
        /// 停止当前预览，销毁场景中的粒子实例
        /// </summary>
        private void StopPreview()
        {
            if (_previewInstance == null)
            {
                return;
            }

            // 通知运行时停止预览并销毁实例
            ActionTimelinePreviewRuntime.PreviewParticleEnd(_previewInstance);
            _previewInstance = null;

            // 禁用停止按钮
            _stopButton?.SetEnabled(false);
        }
    }
}

#endif