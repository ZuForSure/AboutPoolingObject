using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class InstantiateAndDestroy : MonoBehaviour
{
    public GameObject prefab;
    public float lifeTime = 2f;
    public int spawnPerSecond = 50;

    private float timer = 0f;
    private int totalCreated = 0;

    private void Start()
    {
        this.prefab = GameObject.Find("Enemy");
    }

    void Update() => this.SpawnObject();
    
    protected virtual void SpawnObject()
    {
        this.timer += Time.deltaTime;

        // Tạo liên tục nhiều object mỗi giây
        if (this.timer >= 1f / spawnPerSecond)
        {
            this.timer = 0f;

            GameObject obj = Instantiate(this.prefab);

            // Tự hủy sau vài giây
            Destroy(obj, this.lifeTime);

            this.totalCreated++;

            this.ShowProfiler();
        }
    }

    protected virtual void ShowProfiler()
    {
        // Tổng bộ nhớ đã được Unity cấp phát và đang sử dụng
        long totalAlloc = Profiler.GetTotalAllocatedMemoryLong();

        // Bộ nhớ đã được "đặt trước" (reserved) nhưng chưa chắc đã dùng hết
        long totalReserved = Profiler.GetTotalReservedMemoryLong();

        // Lượng bộ nhớ C# (Managed Heap) đang thực sự sử dụng (các biến, object, script...)
        // Đây là vùng bị ảnh hưởng trực tiếp bởi GC. Nếu số này lên cao và sau đó giảm đột ngột -> GC vừa chạy
        long monoUsed = Profiler.GetMonoUsedSizeLong();

        // Tổng dung lượng Mono Heap (C#) mà Unity đã cấp phát. Đây là kích thước tối đa vùng nhớ C# hiện tại
        long monoHeap = Profiler.GetMonoHeapSizeLong();


        // Kết quả Test (12th Gen Intel, Core i5-12500H, 16 CPUs, ~3.1GHz, RAM 16Gb)
        // monoUsed tăng đến ~580 sau đó đột ngột giảm ~520 -> GC vừa chạy
        Debug.Log(
            $"[#{totalCreated}] " +
            $"Allocated: {FormatBytes(totalAlloc)}, " +
            $"Reserved: {FormatBytes(totalReserved)}, " +
            $"Mono Used: {FormatBytes(monoUsed)}, " +
            $"Mono Heap: {FormatBytes(monoHeap)}");
    }

    string FormatBytes(long bytes)
    {
        return $"{(bytes / (1024f * 1024f)):F2} MB";
    }
}
