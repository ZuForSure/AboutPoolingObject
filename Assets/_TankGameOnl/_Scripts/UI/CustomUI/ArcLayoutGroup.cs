using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class ArcLayoutGroup : LayoutGroup
    {
        [Header("Arc")]
        public float radius = 200f;
        public float startAngleDeg = 0f;     // góc kh?i ??u c?a item ??u tiên
        public float spacingDeg = 15f;       // kho?ng cách góc gi?a các item
        public bool clockwise = true;
        [Range(-2160f, 2160f)]
        public float angleOffsetDeg = 0f;    // offset ?? “scroll” theo cung

        [Header("Rotation")]
        public bool alignToTangent = false;  // xoay item theo ti?p tuy?n cung
        public bool keepUpright = true;      // n?u true, h?n ch? l?t ng??c khi xoay theo ti?p tuy?n

        // ====== LayoutGroup API ======
        public override void CalculateLayoutInputHorizontal()
        {
            // R?T QUAN TR?NG: g?i base ?? ?i?n rectChildren + clear tracker
            base.CalculateLayoutInputHorizontal();

            // Layout này không “yêu c?u” kích th??c t?i thi?u/?a thích ??c bi?t (?? 0)
            SetLayoutInputForAxis(0f, 0f, 0f, 0); // axis 0 (width)
        }

        public override void CalculateLayoutInputVertical()
        {
            SetLayoutInputForAxis(0f, 0f, 0f, 1); // axis 1 (height)
        }

        public override void SetLayoutHorizontal()
        {
            PositionChildren();
        }

        public override void SetLayoutVertical()
        {
            PositionChildren();
        }

        // API ti?n ích cho script khác (VD: driver kéo cung)
        public void SetOffsetDegrees(float deg)
        {
            angleOffsetDeg = deg;
            SetDirty();
        }

        // ====== Core positioning ======
        void PositionChildren()
        {
            if (rectTransform == null) return;

            // Tâm là (0,0) khi ??t anchor Content ? center; ta set anchor con v? center ?? ??n gi?n hóa
            int activeIndex = 0;
            float step = clockwise ? -spacingDeg : spacingDeg;

            // Chu?n b? tracker ?? Unity bi?t các thu?c tính nào b? “driven” b?i layout này
            // (CalculateLayoutInputHorizontal ?ã Clear m?t l?n)
            for (int i = 0; i < rectChildren.Count; i++)
            {
                var child = rectChildren[i];
                if (!child || !child.gameObject.activeInHierarchy) continue;

                float angle = startAngleDeg + angleOffsetDeg + activeIndex * step;
                float rad = angle * Mathf.Deg2Rad;
                Vector2 localPos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

                // Drive anchors & anchoredPosition (??a anchor con v? center)
                m_Tracker.Add(this, child,
                    DrivenTransformProperties.AnchorMin |
                    DrivenTransformProperties.AnchorMax |
                    DrivenTransformProperties.AnchoredPositionX |
                    DrivenTransformProperties.AnchoredPositionY |
                    DrivenTransformProperties.Rotation
                );

                child.anchorMin = new Vector2(0.5f, 0.5f);
                child.anchorMax = new Vector2(0.5f, 0.5f);
                child.anchoredPosition = localPos;

                // Xoay theo ti?p tuy?n n?u c?n
                if (alignToTangent)
                {
                    float tangentDeg = angle + (clockwise ? -90f : 90f);
                    float z = keepUpright ? Mathf.DeltaAngle(0f, tangentDeg) : tangentDeg;
                    child.localRotation = Quaternion.Euler(0f, 0f, z);
                }
                else
                {
                    child.localRotation = Quaternion.identity;
                }

                activeIndex++;
            }
        }

        // ====== Hooks ?? t? làm “dirty” ?úng chu?n LayoutGroup ======
        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (rectTransform != null && transform.parent == null || transform.parent != null && transform.parent.GetComponent(typeof(ILayoutGroup)) == null)
                SetDirty();
            else
                SetDirty(); // an toàn: luôn c?p nh?t
        }

        protected override void OnTransformChildrenChanged()
        {
            base.OnTransformChildrenChanged();
            SetDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SetDirty();
        }
#endif
    }
}
