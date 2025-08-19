using System.Collections.Generic;
using UnityEngine;

/*[ExecuteAlways]*/ // ExecuteAlways để có thể xem trước trong Editor, không chỉ khi Play
public class ArcScroller : MonoBehaviour
{
    [Header("Arc")]
    [Tooltip("Nếu <= 0 và có RectTransform -> tự tính theo kích thước rect (bán kính = min(width, height)/2 - arcPadding).")]
    public float radius = -1f;
    public float arcPadding = 10f;          // chỉ dùng khi auto-radius từ RectTransform
    public float startAngleDeg = 0f;
    [Tooltip("Khoảng cách giữa các item (đơn vị: độ). Nếu <= 0, sẽ tính tự động theo số lượng item và góc cung.")]
    public float spacingDeg = 15f;
    [Tooltip("Chiều xoay của vòng cung: true = ngược chiều kim đồng hồ, false = cùng chiều kim đồng hồ.")]
    public bool clockwise = true;
    [Range(-360f, 360f)]
    public float angleOffsetDeg = 0f;

    [Header("Items")]
    public Transform itemPrefab;             // có thể là prefab Sprite/3D/RectTransform
    public int autoItemCount = 10;
    [Tooltip("Tự động instantiate itemPrefab khi Play? Nếu không, sẽ collect các con hiện có trong transform.")]
    public bool autoInstantiateOnPlay = true;
    [Tooltip("Có xoay item theo tiếp tuyến (tangent) không? Nếu có, sẽ xoay quanh trục Z của parent.")]
    public bool alignToTangent = false;
    [Tooltip("Có giữ item luôn thẳng đứng (upright) không? Nếu có, sẽ xoay quanh trục Z của parent.")]
    public bool keepUpright = true;

    [Tooltip("Runtime list (được reset khi thoát play).")]
    public List<Transform> items = new();

    [Header("Interaction")]
    [Tooltip("Có cho phép kéo (drag) không?")]
    public bool enableDrag = true;
    //[Tooltip("Có cho phép cuộn bằng chuột (wheel) không?")]
    //public bool enableWheel = true;
    public float wheelDegStep = 10f;
    public float dragSensitivity = 1f;       // pixel -> độ

    [Header("Inertia & Snap")]
    [Tooltip("Khi không kéo, có áp dụng quán tính (inertia) không?")]
    public bool useInertia = true;
    public float deceleration = 800f;        // deg/s^2
    [Tooltip("Khi không kéo, có tự động snap về vị trí gần nhất không?")]
    public bool snapToNearest = false;
    public float snapSpeed = 600f;           // deg/s

    [Header("Clamp (optional)")]
    [Tooltip("Có giới hạn góc offset không? Nếu có, sẽ clamp angleOffsetDeg trong khoảng minAngleDeg -> maxAngleDeg.")]
    public bool clampAngle = false;
    public float minAngleDeg = -180f;
    public float maxAngleDeg = 180f;

    // runtime
    [SerializeField] float velocityDegPerSec = 0f;
    private Vector2 lastMouseScreen;
    private Vector2 startMouseScreen;
    private bool dragging;

    Camera cam;
    private RectTransform rt; // nếu có RectTransform -> dùng
    private readonly List<Transform> runtimeSpawned = new();

    // ==== Lifecycle ====
    void Awake()
    {
        rt = GetComponent<RectTransform>();
        cam = Camera.main;
        //cam = Camera.main;
        // Ở Edit mode: luôn clear danh sách để không “giữ” state runtime
        if (!Application.isPlaying)
            items.Clear();
    }


    void Start()
    {
        if (Application.isPlaying)
        {
            // Chỉ auto-spawn khi Play
            if (autoInstantiateOnPlay && itemPrefab)
                SpawnRuntimeItems();
            else if (items.Count == 0)
                CollectChildrenAsItems(); // nếu đã có con thủ công trong play
        }
        else
        {
            // Edit mode: không spawn
            items.Clear();
        }

        RepositionAll();
    }

    void Update()
    {
        HandleInput();
        ApplyInertiaAndSnap();
        RepositionAll();
    }

    // ==== Input ====
    void HandleInput()
    {
        //if (!enableDrag && !enableWheel) return;

        //// Wheel
        //if (enableWheel && Application.isFocused)
        //{
        //    float w = Input.mouseScrollDelta.y;
        //    if (Mathf.Abs(w) > 0.0001f)
        //    {
        //        //velocityDegPerSec = 0f;
        //        AddOffset(w * wheelDegStep);
        //    }
        //}

        if (!enableDrag) return;

        if (Input.GetMouseButtonDown(0))
        {
            velocityDegPerSec = 0f;
            startMouseScreen = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            dragging = true;
            lastMouseScreen = Input.mousePosition;

        }

        if (dragging)
        {
            dragging = false;
            Vector2 delta = lastMouseScreen - startMouseScreen;
            float pixels = delta.x; // đơn giản: kéo ngang => di chuyển theo cung
            Debug.Log($"Dragging: {dragging} - delta: {delta} - pixels: {pixels}");
            float degDelta = (pixels / radius) * Mathf.Rad2Deg * dragSensitivity;
            AddOffset(degDelta);

            float dt = Mathf.Max(Time.deltaTime, 0.1f); // khoảng thời gian (tính bằng giây) giữa 2 frame, không bị ảnh hưởng bởi Time.timeScal

            // Cách 1 : float maxVelocity = 720f; // giới hạn ±720 độ/giây
            //velocityDegPerSec = Mathf.Clamp(degDelta / dt, -maxVelocity, maxVelocity);
            // cách 2 : giảm dragSensitivity => nhở hơn 1
            // Cách 3 : Mathf.Max(Time.unscaledDeltaTime, 0.1f)

            velocityDegPerSec = degDelta / dt; // vận tốc hiện tại (độ/giây)
        }
    }

    void ApplyInertiaAndSnap()
    {
        float dt = Time.deltaTime;

        if (useInertia && !dragging && Mathf.Abs(velocityDegPerSec) > 0.01f)
        {
            AddOffset(velocityDegPerSec * dt);

            float sign = Mathf.Sign(velocityDegPerSec);
            velocityDegPerSec -= sign * deceleration * dt;
            velocityDegPerSec = Mathf.MoveTowards(velocityDegPerSec, 0f, deceleration * dt);
        }
        else if (!dragging && snapToNearest && Mathf.Abs(velocityDegPerSec) < 0.01f)
        {
            float step = clockwise ? -spacingDeg : spacingDeg;
            if (!Mathf.Approximately(step, 0f))
            {
                // Snap sao cho item đầu tiên rơi vào góc tham chiếu (0°)
                float curFirst = startAngleDeg + angleOffsetDeg;
                int nNearest = Mathf.RoundToInt(-curFirst / step);
                float desiredFirst = -nNearest * step;
                float desiredOffset = desiredFirst - startAngleDeg;

                float diff = Mathf.DeltaAngle(angleOffsetDeg, desiredOffset);

                float maxMove = snapSpeed * dt;
                float move = Mathf.Clamp(diff, -maxMove, maxMove);
                AddOffset(move);

                if (Mathf.Abs(diff) < 0.1f)
                    angleOffsetDeg = desiredOffset;
            }
        }
    }

    void AddOffset(float degDelta)
    {
        angleOffsetDeg += degDelta;

        // Wrap về [-360, 360]
        angleOffsetDeg = Mathf.DeltaAngle(0f, angleOffsetDeg);

        if (clampAngle)
            angleOffsetDeg = Mathf.Clamp(angleOffsetDeg, minAngleDeg, maxAngleDeg);
    }

    // ==== Positioning ====
    void RepositionAll()
    {
        if (items == null || items.Count == 0) return;

        // Tính bán kính thực sự (local) theo RectTransform nếu có
        float rLocal = ComputeLocalRadius();

        float step = clockwise ? -spacingDeg : spacingDeg;
        for (int i = 0; i < items.Count; i++)
        {
            var t = items[i];
            if (!t) continue;

            float angle = startAngleDeg + angleOffsetDeg + i * step;
            t.name = $"Item_{i} {angle}°";
            float rad = angle * Mathf.Deg2Rad; // chuyển từ độ sang radian

            // local 2D theo trục parent
            Vector2 posLocal2D = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * rLocal; // toạ độ offset theo bán kính.

            if (rt) // Parent có RectTransform -> tính theo trục local của Rect
            {
                // Tâm là rect.center (độc lập pivot)
                Vector2 center = rt.rect.center; //điểm giữa rect → chính là tâm vòng cung.
                Vector2 local2D = center + posLocal2D; // vị trí item nằm đúng trên vòng tròn, cân đối trong khung, không lệch dù pivot thay đổi.

                // Nếu con là RectTransform: đặt anchoredPosition (anchor 0.5/0.5 cho dễ)
                var childRT = t as RectTransform;
                if (childRT)
                {
                    childRT.anchorMin = childRT.anchorMax = new Vector2(0.5f, 0.5f);
                    childRT.anchoredPosition = local2D;
                    // xoay theo tiếp tuyến quanh Z của parent
                    if (alignToTangent)
                    {
                        float tangentDeg = angle + (clockwise ? -90f : 90f);
                        RotaionEulerZ(tangentDeg, childRT, null); // xoay quanh trục Z của parent
                    }
                    else childRT.localRotation = Quaternion.identity;
                }
                else
                {
                    // Con là Transform thường: đặt localPosition trong không gian parent
                    Vector3 local3D = new(local2D.x, local2D.y, 0f);
                    t.localPosition = local3D;

                    if (alignToTangent)
                    {
                        float tangentDeg = angle + (clockwise ? -90f : 90f);
                        RotaionEulerZ(tangentDeg, null, t); // xoay quanh trục Z của parent
                    }
                    else t.localRotation = Quaternion.identity;
                }
            }
            else
            {
                // Không có RectTransform: dùng Transform như cũ (world hoặc local tuỳ parent)
                Vector3 local3D = (Vector3)posLocal2D;
                t.localPosition = local3D;

                if (alignToTangent)
                {
                    float tangentDeg = angle + (clockwise ? -90f : 90f);
                    RotaionEulerZ(tangentDeg, null, t); // xoay quanh trục Z của parent
                }
                else t.localRotation = Quaternion.identity;
            }
        }
    }

    void RotaionEulerZ(float tangentDeg, RectTransform rect = null, Transform tranform = null)
    {

        float z = keepUpright ? Mathf.DeltaAngle(0f, tangentDeg) : tangentDeg;
        if (rect)
            rect.localRotation = Quaternion.Euler(0, 0, z);
        else if (tranform)
            tranform.localRotation = Quaternion.Euler(0, 0, z);
    }

    float ComputeLocalRadius() // tính bán kính local theo RectTransform nếu có
    {
        // Nếu có RectTransform và radius <= 0 -> auto theo rect (nửa cạnh ngắn trừ padding)
        if (rt && radius <= 0f)
        {
            float halfShort = Mathf.Min(rt.rect.width, rt.rect.height) * 0.5f;
            return Mathf.Max(0f, halfShort - arcPadding);
        }
        // Nếu có RectTransform và radius > 0 -> dùng radius như “đơn vị local” của rect
        if (rt) return radius;

        // Không có RectTransform -> dùng radius như trước; nếu <=0 thì fallback 5
        return (radius > 0f) ? radius : 5f;
    }


    // ==== Utilities ====
    void SpawnRuntimeItems()
    {
        items.Clear();
        runtimeSpawned.Clear();

        for (int i = 0; i < autoItemCount; i++)
        {
            var inst = Instantiate(itemPrefab, transform);
            items.Add(inst);
            runtimeSpawned.Add(inst);
        }
    }

    void CollectChildrenAsItems()
    {
        items.Clear();
        for (int i = 0; i < transform.childCount; i++)
            items.Add(transform.GetChild(i));
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Vẽ cung tham chiếu theo RectTransform nếu có
        Gizmos.color = Color.yellow;

        if (rt)
        {
            float rLocal = ComputeLocalRadius();
            Vector3 cW = rt.TransformPoint(new Vector3(rt.rect.center.x, rt.rect.center.y, 0f));

            const int seg = 64;
            Vector3 prev = rt.TransformPoint(new Vector3(rt.rect.center.x + rLocal, rt.rect.center.y, 0f));
            for (int i = 1; i <= seg; i++)
            {
                float ang = (i / (float)seg) * Mathf.PI * 2f;
                Vector3 pLocal = new(rt.rect.center.x + rLocal * Mathf.Cos(ang),
                                             rt.rect.center.y + rLocal * Mathf.Sin(ang), 0f);
                Vector3 pW = rt.TransformPoint(pLocal);
                Gizmos.DrawLine(prev, pW);
                prev = pW;
            }

            Gizmos.color = Color.cyan;
            Vector3 rightW = rt.TransformPoint(new Vector3(rt.rect.center.x + rLocal, rt.rect.center.y, 0f));
            Gizmos.DrawLine(cW, rightW);
        }
        else
        {
            float r = (radius > 0f) ? radius : 5f;
            Vector3 c = transform.position;
            const int seg = 64;
            Vector3 prev = c + transform.right * r;
            for (int i = 1; i <= seg; i++)
            {
                float ang = (i / (float)seg) * Mathf.PI * 2f;
                Vector3 p = c + (transform.right * Mathf.Cos(ang) + transform.up * Mathf.Sin(ang)) * r;
                Gizmos.DrawLine(prev, p);
                prev = p;
            }
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(c, c + transform.right * (r + 0.5f));
        }
    }
#endif
}
