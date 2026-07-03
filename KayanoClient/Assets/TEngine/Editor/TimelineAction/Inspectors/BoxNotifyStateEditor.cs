#if UNITY_EDITOR

using System.Collections.Generic;
using KayanoAction.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TEngine.Editor.TimelineAction
{
    [CustomEditor(typeof(BoxNotifyState))]
    internal sealed class BoxNotifyStateEditor : UnityEditor.Editor
    {
        private enum EditMode
        {
            None = -1,
            Center = 0,
            Size = 1,
        }

        private static readonly List<BoxNotifyStateEditor> Instances = new(4);

        private BoxNotifyState _target;
        private EditMode _editMode = EditMode.None;
        private bool _showInScene = true;

        private SerializedProperty _center;
        private SerializedProperty _radius;
        private SerializedProperty _size;
        private SerializedProperty _boxShape;

        private void OnEnable()
        {
            _target = (BoxNotifyState)target;
            _center = serializedObject.FindProperty(nameof(BoxNotifyState.center));
            _radius = serializedObject.FindProperty(nameof(BoxNotifyState.radius));
            _size = serializedObject.FindProperty(nameof(BoxNotifyState.size));
            _boxShape = serializedObject.FindProperty(nameof(BoxNotifyState.boxShape));

            if (!Instances.Contains(this))
            {
                Instances.Add(this);
                SceneView.duringSceneGui += OnSceneGUI;
            }
        }

        private void OnDisable()
        {
            Instances.Remove(this);
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(ActionNotifyInspectorElements.BuildDefaultInspector(serializedObject));
            root.Add(ActionNotifyInspectorElements.BuildHelpBox(
                "Scene：红色=HitBox，青色=DodgeBox。Timeline 预览进入 Clip 区间也会显示线框。"));

            var showToggle = new Toggle("Scene 显示此 Clip") { value = _showInScene };
            showToggle.RegisterValueChangedCallback(evt =>
            {
                _showInScene = evt.newValue;
                SceneView.RepaintAll();
            });
            root.Add(showToggle);

            root.Add(ActionNotifyInspectorElements.BuildButtonRow(
                ("编辑中心", () => ToggleEditMode(EditMode.Center)),
                ("编辑尺寸", () => ToggleEditMode(EditMode.Size))));

            root.Add(new Button(SoloShowCurrent) { text = "只显示当前 Clip 线框", style = { marginTop = 4f } });

            return root;
        }

        private void ToggleEditMode(EditMode mode)
        {
            _editMode = _editMode == mode ? EditMode.None : mode;
            SceneView.RepaintAll();
        }

        private void SoloShowCurrent()
        {
            _showInScene = true;
            for (var i = 0; i < Instances.Count; i++)
            {
                if (Instances[i] != this)
                {
                    Instances[i]._showInScene = false;
                }
            }

            SceneView.RepaintAll();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!_showInScene || _target == null)
            {
                return;
            }

            var parent = ResolvePreviewTransform();
            if (parent == null)
            {
                Handles.BeginGUI();
                GUILayout.Label("未找到 Timeline 绑定角色：请选中 PlayableDirector 或角色后重试。");
                Handles.EndGUI();
                return;
            }

            serializedObject.Update();
            BoxNotifyStateDrawUtility.DrawWire(_target, parent);
            DrawHandles(parent);
            serializedObject.ApplyModifiedProperties();
        }

        private Transform ResolvePreviewTransform()
        {
            if (_target.owner != null)
            {
                return _target.owner.transform;
            }

            var owner = TimelineActionClipUtility.ResolvePreviewOwner(_target);
            return owner != null ? owner.transform : null;
        }

        private void DrawHandles(Transform parent)
        {
            if (_editMode == EditMode.Center)
            {
                var worldPos = parent.TransformPoint(_center.vector3Value);
                var newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
                var newLocalPos = parent.InverseTransformPoint(newWorldPos);
                if (newLocalPos != _center.vector3Value)
                {
                    _center.vector3Value = newLocalPos;
                }

                return;
            }

            if (_editMode != EditMode.Size)
            {
                return;
            }

            var centerWorld = parent.TransformPoint(_center.vector3Value);
            var shape = (EBoxShape)_boxShape.enumValueIndex;
            if (shape == EBoxShape.Sphere)
            {
                var newRadius = Handles.RadiusHandle(Quaternion.identity, centerWorld, _radius.floatValue, true);
                if (!Mathf.Approximately(newRadius, _radius.floatValue))
                {
                    _radius.floatValue = newRadius;
                }
            }
            else if (shape == EBoxShape.Box)
            {
                var newSize = Handles.ScaleHandle(_size.vector3Value, centerWorld, parent.rotation);
                if (newSize != _size.vector3Value)
                {
                    _size.vector3Value = newSize;
                }
            }
        }
    }
}

#endif
