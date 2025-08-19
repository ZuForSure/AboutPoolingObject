using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArcScrollDriver : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    public ArcLayoutGroup arc;
    [Tooltip("?? nh?y: pixel d?ch chu?t -> ?? cung")]
    public float sensitivity = 1f; // 1 = dùng công th?c radian chu?n phía d??i

    [Tooltip("Quán tính sau khi th? kéo")]
    public bool useInertia = true;
    public float deceleration = 800f; // deg/s^2

    float velocityDegPerSec = 0f;
    float lastFrameTime;

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        if (useInertia && Mathf.Abs(velocityDegPerSec) > 0.01f)
        {
            arc.SetOffsetDegrees(arc.angleOffsetDeg + velocityDegPerSec * dt);

            // gi?m t?c
            float sign = Mathf.Sign(velocityDegPerSec);
            velocityDegPerSec -= sign * deceleration * dt;
            if (Mathf.Sign(velocityDegPerSec) != sign) velocityDegPerSec = 0f;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        velocityDegPerSec = 0f;
        lastFrameTime = Time.unscaledTime;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (arc == null) return;

        // Quãng ???ng kéo theo ti?p tuy?n ~ pixel trên màn hình.
        // Quy ??i: s (pixel) ~ r*theta  => theta (rad) = s / r
        // deg = (s / r) * Rad2Deg.
        float pixels = eventData.delta.x; // ??n gi?n: l?y delta.x. Có th? dot v?i h??ng ti?p tuy?n n?u mu?n.
        float degDelta = (pixels / Mathf.Max(1f, arc.radius)) * Mathf.Rad2Deg * sensitivity;

        // chi?u cung ?ã tính ? ArcLayoutGroup.clockwise (offset c?ng thêm hay tr? ?i ??u ?úng tr?c quan)
        arc.SetOffsetDegrees(arc.angleOffsetDeg + degDelta);

        // velocity ??c l??ng cho quán tính
        float now = Time.unscaledTime;
        float dt = Mathf.Max(0.0001f, now - lastFrameTime);
        velocityDegPerSec = degDelta / dt;
        lastFrameTime = now;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // gi? velocityDegPerSec cho inertia
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (arc == null) return;
        float wheel = eventData.scrollDelta.y; // bánh xe chu?t
        float degDelta = wheel * 10f;          // ch?nh cho v?a tay
        arc.SetOffsetDegrees(arc.angleOffsetDeg + degDelta);
        velocityDegPerSec = 0f;
    }
}
