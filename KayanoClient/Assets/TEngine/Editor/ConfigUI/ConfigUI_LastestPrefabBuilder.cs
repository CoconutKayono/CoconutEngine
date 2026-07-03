#if UNITY_EDITOR
using GameLogic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TEngine.Editor.ConfigUI
{
    public static class ConfigUI_LastestPrefabBuilder
    {
        private const string PrefabPath = "Assets/AssetRaw/UI/WindowSetting.prefab";

        [MenuItem("TEngine/ConfigUI/Build WindowSetting Prefab Shell")]
        public static void Build()
        {
            var spBg = LoadSprite("Assets/AssetRaw/UIRaw/Raw/BackGround/ConfigUI_bg_simple_2560x1440.png");
            var spPanelSidebar = LoadSprite("Assets/AssetRaw/UIRaw/Raw/UI/ConfigUI/ConfigUI_panel_sidebar.png");
            var spPanelContent = LoadSprite("Assets/AssetRaw/UIRaw/Raw/UI/ConfigUI/ConfigUI_panel_content.png");
            var spNavNormal = LoadSprite("Assets/AssetRaw/UIRaw/Raw/UI/ConfigUI/ConfigUI_nav_item_normal.png");
            var spBtnBack = LoadSprite("Assets/AssetRaw/UIRaw/Raw/UI/ConfigUI/ConfigUI_btn_back.png");
            var spCapsule = LoadSprite("Assets/AssetRaw/UIRaw/Raw/UI/ConfigUI/ConfigUI_capsule_location.png");
            var spHome = LoadSprite("Assets/AssetRaw/UIRaw/Raw/UI/ConfigUI/ConfigUI_icon_home.png");
            var spResetBg = LoadSprite("Assets/AssetRaw/UIRaw/Raw/UI/ConfigUI/ConfigUI_btn_reset_bg.png");
            var spResetIcon = LoadSprite("Assets/AssetRaw/UIRaw/Raw/UI/ConfigUI/ConfigUI_icon_reset.png");

            var instance = PrefabUtility.LoadPrefabContents(PrefabPath);
            var root = instance;

            for (var i = root.transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(root.transform.GetChild(i).gameObject);
            }

            var bgGo = CreateGo("m_imgBg", root.transform);
            Stretch(bgGo.GetComponent<RectTransform>());
            AddImage(bgGo, spBg, Image.Type.Simple);

            var header = CreateGo("m_goHeader", root.transform);
            var headerRt = header.GetComponent<RectTransform>();
            headerRt.anchorMin = new Vector2(0, 1);
            headerRt.anchorMax = new Vector2(1, 1);
            headerRt.pivot = new Vector2(0.5f, 1f);
            headerRt.sizeDelta = new Vector2(0, 88);
            var hlg = header.AddComponent<HorizontalLayoutGroup>();
            hlg.padding = new RectOffset(32, 32, 16, 8);
            hlg.spacing = 12;

            var backGo = CreateGo("m_btnBack", header.transform);
            var backLe = backGo.AddComponent<LayoutElement>();
            backLe.minWidth = 48;
            backLe.minHeight = 48;
            AddImage(backGo, spBtnBack, Image.Type.Simple).preserveAspect = true;
            backGo.AddComponent<Button>().targetGraphic = backGo.GetComponent<Image>();

            var locGo = CreateGo("m_goLocation", header.transform);
            locGo.AddComponent<LayoutElement>().preferredWidth = 200;
            AddImage(locGo, spCapsule);
            var locH = locGo.AddComponent<HorizontalLayoutGroup>();
            locH.spacing = 8;
            locH.padding = new RectOffset(12, 16, 6, 6);
            var homeGo = CreateGo("m_iconHome", locGo.transform);
            AddImage(homeGo, spHome, Image.Type.Simple).preserveAspect = true;
            homeGo.AddComponent<LayoutElement>().minWidth = 28;
            AddTmp(CreateGo("m_txtLocation", locGo.transform), "街区", 26, TextAlignmentOptions.MidlineLeft);

            var body = CreateGo("m_goBody", root.transform);
            var bodyRt = body.GetComponent<RectTransform>();
            Stretch(bodyRt);
            bodyRt.offsetMin = new Vector2(32, 48);
            bodyRt.offsetMax = new Vector2(-32, -96);
            var bodyH = body.AddComponent<HorizontalLayoutGroup>();
            bodyH.spacing = 24;

            var navWrap = CreateGo("m_goNavWrap", body.transform);
            navWrap.AddComponent<LayoutElement>().preferredWidth = 240;
            var sidebarPanel = CreateGo("m_panelSidebar", navWrap.transform);
            Stretch(sidebarPanel.GetComponent<RectTransform>());
            AddImage(sidebarPanel, spPanelSidebar);
            var nav = CreateGo("m_goNav", sidebarPanel.transform);
            StretchPad(nav.GetComponent<RectTransform>(), 12);
            var navV = nav.AddComponent<VerticalLayoutGroup>();
            navV.spacing = 8;

            string[] navLabels = { "画面", "键鼠", "声音", "系统" };
            for (var i = 0; i < navLabels.Length; i++)
            {
                var btnGo = CreateGo("m_btnNav" + i, nav.transform);
                btnGo.AddComponent<LayoutElement>().minHeight = 52;
                AddImage(btnGo, spNavNormal);
                var btn = btnGo.AddComponent<Button>();
                btn.targetGraphic = btnGo.GetComponent<Image>();
                var txtGo = CreateGo("Text", btnGo.transform);
                Stretch(txtGo.GetComponent<RectTransform>());
                var t = AddTmp(txtGo, navLabels[i], 26, TextAlignmentOptions.Center);
                t.color = Color.white;
            }

            var contentWrap = CreateGo("m_goContentWrap", body.transform);
            contentWrap.AddComponent<LayoutElement>().flexibleWidth = 1;
            var contentPanel = CreateGo("m_panelContent", contentWrap.transform);
            Stretch(contentPanel.GetComponent<RectTransform>());
            AddImage(contentPanel, spPanelContent);
            var inner = CreateGo("m_goContent", contentPanel.transform);
            StretchPad(inner.GetComponent<RectTransform>(), 20);

            var titleGo = CreateGo("m_txtPageTitle", inner.transform);
            var titleRt = titleGo.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0, 1);
            titleRt.anchorMax = new Vector2(1, 1);
            titleRt.pivot = new Vector2(0.5f, 1);
            titleRt.sizeDelta = new Vector2(0, 48);
            AddTmp(titleGo, "画面", 32, TextAlignmentOptions.TopLeft);

            var pageHost = CreateGo("m_goPageContainer", inner.transform);
            var pageHostRt = pageHost.GetComponent<RectTransform>();
            Stretch(pageHostRt);
            pageHostRt.offsetMax = new Vector2(0, -56);

            Transform displayList;
            var pageDisplay = CreateScrollPage(pageHost.transform, "Page_Display", "m_goDisplayList", out displayList);
            Transform rebindList;
            var pageControls = CreateScrollPage(pageHost.transform, "Page_Controls", "m_goRebindList", out rebindList);
            pageControls.SetActive(false);
            var pageAudio = CreateGo("Page_Audio", pageHost.transform);
            Stretch(pageAudio.GetComponent<RectTransform>());
            pageAudio.SetActive(false);
            var pageSystem = CreateGo("Page_System", pageHost.transform);
            Stretch(pageSystem.GetComponent<RectTransform>());
            pageSystem.SetActive(false);

            var resetWrap = CreateGo("m_btnResetWrap", root.transform);
            var resetRt = resetWrap.GetComponent<RectTransform>();
            resetRt.anchorMin = new Vector2(1, 0);
            resetRt.anchorMax = new Vector2(1, 0);
            resetRt.pivot = new Vector2(1, 0);
            resetRt.anchoredPosition = new Vector2(-40, 24);
            resetRt.sizeDelta = new Vector2(200, 48);
            var resetH = resetWrap.AddComponent<HorizontalLayoutGroup>();
            resetH.spacing = 8;
            var resetIconGo = CreateGo("m_iconReset", resetWrap.transform);
            AddImage(resetIconGo, spResetIcon, Image.Type.Simple).preserveAspect = true;
            resetIconGo.AddComponent<LayoutElement>().minWidth = 36;
            var resetBtnGo = CreateGo("m_btnReset", resetWrap.transform);
            resetBtnGo.AddComponent<LayoutElement>().preferredWidth = 160;
            AddImage(resetBtnGo, spResetBg);
            var resetBtn = resetBtnGo.AddComponent<Button>();
            resetBtn.targetGraphic = resetBtnGo.GetComponent<Image>();
            var resetTxtGo = CreateGo("Text", resetBtnGo.transform);
            Stretch(resetTxtGo.GetComponent<RectTransform>());
            AddTmp(resetTxtGo, "恢复默认", 24, TextAlignmentOptions.Center);

            var bind = root.GetComponent<UIBindComponent>() ?? root.AddComponent<UIBindComponent>();
            bind.Clear();
            bind.className = "WindowSetting";
            bind.uiType = "UIWindow";

            AddBind(bind, bgGo.GetComponent<Image>());
            AddBind(bind, backGo.GetComponent<Button>());
            AddBind(bind, titleGo.GetComponent<TMP_Text>());
            for (var i = 0; i < 4; i++)
            {
                AddBind(bind, nav.transform.Find("m_btnNav" + i).GetComponent<Button>());
            }

            AddBind(bind, pageDisplay.GetComponent<RectTransform>());
            AddBind(bind, pageControls.GetComponent<RectTransform>());
            AddBind(bind, pageAudio.GetComponent<RectTransform>());
            AddBind(bind, pageSystem.GetComponent<RectTransform>());
            AddBind(bind, displayList.GetComponent<RectTransform>());
            AddBind(bind, rebindList.GetComponent<RectTransform>());
            AddBind(bind, resetBtn);

            PrefabUtility.SaveAsPrefabAsset(instance, PrefabPath);
            PrefabUtility.UnloadPrefabContents(instance);
            AssetDatabase.SaveAssets();
            Debug.Log("[ConfigUI_LastestPrefabBuilder] Shell prefab saved.");
        }

        private static Sprite LoadSprite(string path) => AssetDatabase.LoadAssetAtPath<Sprite>(path);

        private static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static void StretchPad(RectTransform rt, float pad)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(pad, pad);
            rt.offsetMax = new Vector2(-pad, -pad);
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
            return tmp;
        }

        private static GameObject CreateGo(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static GameObject CreateScrollPage(
            Transform parent,
            string pageName,
            string listName,
            out Transform listTransform)
        {
            var page = CreateGo(pageName, parent);
            Stretch(page.GetComponent<RectTransform>());
            var scrollGo = CreateGo("m_scrollContent", page.transform);
            Stretch(scrollGo.GetComponent<RectTransform>());
            var sr = scrollGo.AddComponent<ScrollRect>();
            sr.horizontal = false;
            sr.vertical = true;
            sr.movementType = ScrollRect.MovementType.Elastic;
            sr.scrollSensitivity = 24f;

            var viewport = CreateGo("Viewport", scrollGo.transform);
            Stretch(viewport.GetComponent<RectTransform>());
            var vpImg = viewport.AddComponent<Image>();
            vpImg.color = new Color(0, 0, 0, 0);
            viewport.AddComponent<Mask>().showMaskGraphic = false;

            var content = CreateGo(listName, viewport.transform);
            var crt = content.GetComponent<RectTransform>();
            crt.anchorMin = new Vector2(0, 1);
            crt.anchorMax = new Vector2(1, 1);
            crt.pivot = new Vector2(0.5f, 1);
            crt.anchoredPosition = Vector2.zero;
            crt.sizeDelta = Vector2.zero;

            var vlg = content.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 10;
            vlg.padding = new RectOffset(8, 8, 8, 8);
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;

            var fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            sr.viewport = viewport.GetComponent<RectTransform>();
            sr.content = crt;
            listTransform = content.transform;
            return page;
        }

        private static void AddBind(UIBindComponent bind, Component c) => bind.AddComponent(c);
    }
}
#endif
