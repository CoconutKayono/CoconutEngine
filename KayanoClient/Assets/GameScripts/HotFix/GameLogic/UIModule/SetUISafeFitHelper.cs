using UnityEngine;

namespace GameLogic
{
    public class SetUISafeFitHelper
    {
        public bool LiuHaiFit { get; set; } = false;
        public float TopSpacing { get; set; } = 0;
        public bool BottomFit { get; set; } = false;
        public float BottomSpacing { get; set; } = 0;

        private readonly RectTransform m_curFitRect;

        public SetUISafeFitHelper(
            RectTransform fitRect,
            bool liuHaiFit = true,
            float topSpacing = 0,
            bool bottomFit = true,
            float bottomSpacing = 0)
        {
            LiuHaiFit = liuHaiFit;
            TopSpacing = topSpacing;
            BottomFit = bottomFit;
            BottomSpacing = bottomSpacing;
            m_curFitRect = fitRect;
        }

        public SetUISafeFitHelper()
        {
        }

        public void SetUIFit()
        {
            if (m_curFitRect == null)
            {
                return;
            }

            Vector3 offsetMax = new Vector2(0f, 0f);
            Vector3 offsetMin = new Vector2(0f, 0f);
            Rect[] cutouts = Screen.cutouts;

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    TopSpacing = 70;
                    BottomSpacing = 80;
                    break;

                case RuntimePlatform.Android:
                    break;

                case RuntimePlatform.IPhonePlayer:
                    var phoneType = SystemInfo.deviceModel;
                    TopSpacing = 70;
                    BottomSpacing = 80;

                    if (phoneType == "iPhone12,1" || phoneType == "iPhone11,8")
                    {
                        TopSpacing = 30;
                        BottomSpacing = 70;
                    }

                    break;
            }

            if (LiuHaiFit)
            {
                if (cutouts != null && cutouts.Length > 0 && Application.platform != RuntimePlatform.IPhonePlayer)
                {
                    offsetMax = new Vector3(m_curFitRect.offsetMax.x, cutouts[0].height);
                }
                else if (Screen.safeArea.yMax > 0 && Screen.height - Screen.safeArea.yMax > 0)
                {
                    offsetMax = new Vector3(
                        Screen.width - Screen.safeArea.xMax,
                        Screen.height - (Screen.safeArea.yMax + TopSpacing));
                }

                m_curFitRect.offsetMax = new Vector2(offsetMax.x, -offsetMax.y);
            }
            else
            {
                m_curFitRect.offsetMax = offsetMax;
            }

            if (BottomFit)
            {
                if (Screen.safeArea.y > 0)
                {
                    offsetMin = new Vector2(Screen.safeArea.x, Mathf.Abs(Screen.safeArea.y - BottomSpacing));
                }

                if (Mathf.Abs(offsetMin.y) > 0)
                {
                    m_curFitRect.offsetMin = new Vector2(m_curFitRect.offsetMin.x, Mathf.Abs(offsetMin.y));
                }
                else
                {
                    m_curFitRect.offsetMin = offsetMin;
                }
            }
            else
            {
                m_curFitRect.offsetMin = offsetMin;
            }
        }

        public void SetUINotFit(RectTransform rect)
        {
            if (m_curFitRect == null || rect == null)
            {
                return;
            }

            Vector3 localPos = rect.localPosition;
            float compensationY = -m_curFitRect.offsetMax.y;
            rect.localPosition = new Vector3(localPos.x, localPos.y + compensationY, localPos.z);
        }

        public void SetUINotFit(RectTransform rect, RectTransform refRect)
        {
            if (rect == null || refRect == null)
            {
                return;
            }

            Vector3 localPos = rect.localPosition;
            float compensationY = -refRect.offsetMax.y;
            rect.localPosition = new Vector3(localPos.x, localPos.y + compensationY, localPos.z);
        }
    }
}
