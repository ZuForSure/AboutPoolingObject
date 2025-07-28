using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;


public enum State
{
    Default,
    Pool,
    LeanPool,

}
public class LessonTwo : MonoBehaviour
{
    // LESSON 2 : CÁCH TRIỂN KHAI
    // - Qua Lesson 2 này mình sẽ nói về các ưu , khuyết điểm của các đồng thời cũng sẽ triển khai về mặt viết code với 3 dạng :
    // + Tạo object bằng instantie .
    // + Tạo object bằng pool cơ bản.
    // + Tạo object bằng package LeanPool.

    // ---------------- Các thuộc tính để tạo 1 pool thông thường -------------------------------
    
    // Để dễ Demo mình sẽ tạo 1 biến enum State để dễ chuyển qua lại giữa các dạng
    [SerializeField] private State state;
    // Biến để gắn prefabs 

    [SerializeField] private GameObject pref;
    // Khai báo số lượng trong Pool
    [SerializeField] private int poolCount;

    // Các Tranform để các object được tạo sẽ làm child của các tranform này ( cái này chỉ để gọn màn hình )
    [SerializeField] private Transform poolParent;
    [SerializeField] private Transform defaultParent;
    [SerializeField] private Transform leanPoolParent;

    // Biến thời gian để tạo bộ đếm thời gian tạo giữa các obj
    [SerializeField] private float time;

    // Khoảng cách thời gian tạo giữa các obj
    [SerializeField] private float duration;

    // 1 collection kiểu Queue ( first in - first out ) để lưu các obj được tạo ra để tái sử dụng
    private Queue<GameObject> pool = new();

    public State State => state;

    // Một hàm đếm thời gian cơ bản
    private bool CheckTime()
    {
        time += Time.deltaTime;
        if (time > duration)
        {
            time = 0;
            return true;
        }
        return false;
    }

    // ===> Tạo object bằng Instantiate <======

    // - Đầu tiên mình sẽ nói về việc triển khai code tạo đối tượng bằng Instantiate mặc định => mở Region "Default" để xem code => Nhớ đóng lại để dễ nhìn code .

    // + Ưu điểm : Code nhanh , hầu như chỉ cần 1 dòng là xong , không triển khai code nhiều , dễ hiểu .

    // + Khuyết Điểm : Không có đối tượng nào quản lý , khó tái sử dụng , phải dùng vòng lặp để duyệt nếu muốn tái sử dụng , dễ bị lỗi logic.
    //                 Nếu không có số lượng cụ thể , đều kiện ngừng hoặc muốn destroy và sinh ra lại từ đầu.
    //                  => Phải cấp bộ nhớ cho các đối tượng được sinh ra liên tục dễ bị overheap 
    //                  => Khi đối tượng bị destroy hoặc bộ nhớ heap tràn , GC Collector buộc phải dọn dẹp xử lý thường xuyên dễ dẫn đến game bị giật lag do GC collector phải xử lý




    // ===> Tạo object bằng pool cơ bản <======

    // - Tiếp theo mình sẽ nói về việc triển khai code tạo 1 pool cơ bản để tối ưu việc tái sử dụng đối tượng tránh tạo và hủy liên tục

    // => mở Region "Pool" để xem code => Nhớ đóng lại để dễ nhìn code .

    // + Ưu điểm : Chỉ cần tạo đối tượng 1 lần và có thể dùng lại chính đối tượng đó hạn chế việc tạo mới và destroy đối tượng liên tục ,
    // tối ưu được việc cấp phát bộ nhớ khi tạo đối tượng liên tục và dọn dẹp đối tượng khi bị destroy

    // + Khuyết Điểm : Mỗi lần triển khai phải tự code , phải code khá nhiều , phải quản lý được việc reset đối tượng khi đưa về pool để tránh lỗi logic , muốn thêm chức năng khác phải tự code thêm

    // ===>  Tạo object bằng package LeanPool <======

    // - Tiếp theo mình sẽ nói về việc dùng package có sẵn để sử dụng pool cho project.

    // => mở Region "LeanPool" để xem code => Nhớ đóng lại để dễ nhìn code .

    // + Ưu điểm : Giống pool tự triển khai có thể tái sử dụng các đối tượng đã tạo , sử dụng đơn giản khá giống với Tạo object bằng Instantiate , đa dạng chức năng để sử dụng

    // + Khuyết Điểm : Phải đọc Document để hiểu toàn bộ code và chức năng của package để dễ cho việc sử dụng cũng như dễ debug nếu có lỗi phát sinh.
    // Là tool bên ngoài nên phải follow theo , không tự kiểm soát hay tự quản lý được hoặc không tự mở rộng được .


    #region Các hàm của  Monobehavior
    private void Start()
    {
        InitPool();
    }
    private void Update()
    {
        switch (state)
        {
            case State.Default:
                DefaultCreateObj();
                break;
            case State.Pool:
                {
                    PoolCreateObj();
                }

                break;
            case State.LeanPool:
                {
                    LeanPoolCreateObj();
                }

                break;
        }

    }
    #endregion

    #region Pool
    private void InitPool()
    {
        for (int i = 0; i < poolCount; i++)
        {
            GameObject go = Instantiate(pref, poolParent);
            go.SetActive(false);
            pool.Enqueue(go);
        }

    }

    private GameObject GetObj()
    {
        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        obj.transform.SetParent(null);
        return obj;
    }

    public void ReturnPool(GameObject go)
    {
        go.transform.position = Vector3.zero;
        go.transform.SetParent(poolParent, false);
        go.SetActive(false);
        pool.Enqueue(go);
    }
    private void PoolCreateObj()
    {
        if (CheckTime())
        {
            GetObj();
        }
    }


    #endregion

    #region Default

    private void DefaultCreateObj()
    {
        if (CheckTime())
        {
            GameObject obj = Instantiate(pref, defaultParent);
            Destroy(obj, 4f);
        }
    }

    #endregion

    #region LeanPool

    private void LeanPoolCreateObj()
    {
        if (CheckTime())
        {
            LeanPool.Spawn(pref, leanPoolParent);
        }

    }

    #endregion

    
}
