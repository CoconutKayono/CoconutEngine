#if UNITY_EDITOR
using GameLogic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TEngine.Editor.ConfigUI
{
    public static class SettingDropdownRow_LastestPrefabBuilder
    {
        private const string PrefabPath = "Assets/AssetRaw/UI/SettingDropdownRow_Lastest.prefab";

        [MenuItem("TEngine/ConfigUI/Build SettingDropdownRow_Lastest Prefab")]
        public static void Build()
        {
            var spRow = LoadSprite("Assets/AssetRaw/UIRaw/Raw/UI/ConfigUI/ConfigUI_row_bg.png");
            var spDropdown = LoadSprite("Assets/AssetRaw/UIRaw/Raw/UI/ConfigUI/ConfigUI_dropdown_bg.png");
            var spChevron = LoadSprite("Assets/AssetRaw/UIRaw/Raw/UI/ConfigUI/ConfigUI_icon_chevron_down.png");

            var root = new GameObject("SettingDropdownRow_Lastest", typeof(RectTransform));
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.anchorMin = new Vector2(0, 1);
            rootRt.anchorMax = new Vector2(1, 1);
            rootRt.pivot = new Vector2(0.5f, 1f);
            rootRt.sizeDelta = new Vector2(0, 56);

            AddImage(root, spRow);
            var rootLe = root.AddComponent<LayoutElement>();
            rootLe.minHeight = 56;
            rootLe.preferredHeight = 56;
            var hlg = root.AddComponent<HorizontalLayoutGroup>();
            hlg.padding = new RectOffset(20, 20, 8, 8);
            hlg.spacing = 16;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;

            var labelGo = CreateGo("m_tmpLabel", root.transform);
            var labelLe = labelGo.AddComponent<LayoutElement>();
            labelLe.minWidth = 160;
            labelLe.preferredWidth = 240;
            labelLe.flexibleWidth = 1;
            AddTmp(labelGo, "Label", 28, TextAlignmentOptions.MidlineLeft);

            var dropGo = CreateGo("m_imgDropdown", root.transform);
            var dropLe = dropGo.AddComponent<LayoutElement>();
            dropLe.minWidth = 220;
            dropLe.preferredWidth = 280;
            dropLe.minHeight = 40;
            AddImage(dropGo, spDropdown);

            var valueGo = CreateGo("m_tmpValue", dropGo.transform);
            var valueRt = valueGo.GetComponent<RectTransform>();
            Stretch(valueRt);
            valueRt.offsetMin = new Vector2(8, 4);
            valueRt.offsetMax = new Vector2(-28, -4);
            AddTmp(valueGo, "Value", 26, TextAlignmentOptions.MidlineLeft);

            var chevronGo = CreateGo("m_imgChevron", dropGo.transform);
            var chevronRt = chevronGo.GetComponent<RectTransform>();
            chevronRt.anchorMin = new Vector2(1, 0.5f);
            chevronRt.anchorMax = new Vector2(1, 0.5f);
            chevronRt.pivot = new Vector2(1, 0.5f);
            chevronRt.anchoredPosition = new Vector2(-10, 0);
            chevronRt.sizeDelta = new Vector2(20, 20);
            var chevronImg = AddImage(chevronGo, spChevron, Image.Type.Simple);
            chevronImg.preserveAspect = true;
            chevronImg.raycastTarget = false;
            chevronGo.AddComponent<LayoutElement>().ignoreLayout = true;

            var bind = root.AddComponent<UIBindComponent>();
            bind.Clear();
            bind.className = "SettingDropdownRow_Lastest";
            bind.uiType = "UIWidget";
            bind.AddComponent(root.GetComponent<Image>());
            bind.AddComponent(labelGo.GetComponent<TextMeshProUGUI>());
            bind.AddComponent(dropGo.GetComponent<Image>());
            bind.AddComponent(valueGo.GetComponent<TextMeshProUGUI>());
            bind.AddComponent(chevronGo.GetComponent<Image>());

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
            Debug.Log("[SettingDropdownRow_LastestPrefabBuilder] Prefab saved.");
        }

        private static Sprite LoadSprite(string path) => AssetDatabase.LoadAssetAtPath<Sprite>(path);

        private static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static Image AddImage(GameObject go, Sprite sp, Image.Type type = Image.Type.Sliced)
        {
            var img = go.GetComponent<Image>() ?? go.AddComponent<Image>();
            img.sprite = sp;
            img.type = type;
            img.color = Color.white;
            return img;
        }

        private static TMP_Text AddTmp(GameObject go, string text, float size, TextAlignmentOptions align)
        {
            var tmp = go.GetComponent<TextMeshProUGUI>() ?? go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.alignment = align;
            tmp.color = Color.white;
            tmp.raycastTarget = false;
            return tmp;
        }

        private static GameObject CreateGo(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }
    }
}
#endif
