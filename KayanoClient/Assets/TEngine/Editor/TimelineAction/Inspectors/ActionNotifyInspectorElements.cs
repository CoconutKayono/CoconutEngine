#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TEngine.Editor.TimelineAction
{
    internal static class ActionNotifyInspectorElements
    {
        public static VisualElement BuildDefaultInspector(
            SerializedObject serializedObject,
            params string[] skipPropertyNames)
        {
            var container = new VisualElement();
            var iterator = serializedObject.GetIterator();
            if (!iterator.NextVisible(true))
            {
                return container;
            }

            do
            {
                if (ShouldSkipProperty(iterator.name, skipPropertyNames))
                {
                    continue;
                }

                var field = new PropertyField(iterator.Copy());
                if (iterator.name == "m_Script")
                {
                    field.SetEnabled(false);
                }

                container.Add(field);
            }
            while (iterator.NextVisible(false));

            return container;
        }

        private static bool ShouldSkipProperty(string propertyName, string[] skipPropertyNames)
        {
            if (skipPropertyNames == null || skipPropertyNames.Length == 0)
            {
                return false;
            }

            for (var i = 0; i < skipPropertyNames.Length; i++)
            {
                if (propertyName == skipPropertyNames[i])
                {
                    return true;
                }
            }

            return false;
        }

        public static VisualElement BuildButtonRow(params (string label, Action onClick)[] buttons)
        {
            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    marginTop = 8f,
                },
            };

            for (var i = 0; i < buttons.Length; i++)
            {
                var (label, onClick) = buttons[i];
                var button = new Button(onClick) { text = label };
                if (i > 0)
                {
                    button.style.marginLeft = 4f;
                }

                button.style.flexGrow = 1f;
                row.Add(button);
            }

            return row;
        }

        public static HelpBox BuildHelpBox(string message, HelpBoxMessageType type = HelpBoxMessageType.Info)
        {
            return new HelpBox(message, type)
            {
                style = { marginTop = 8f },
            };
        }
    }
}

#endif
