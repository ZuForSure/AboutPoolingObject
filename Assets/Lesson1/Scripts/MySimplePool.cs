using com.cyborgAssets.inspectorButtonPro;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MySimplePool : MonoBehaviour
{
    public GameObject prefab;
    public int listSize = 10;

    // Quản lý các prefab cần pool
    public List<GameObject> poolList = new ();

    // Quản lý các prefab đang hoạt động trong game
    public List<GameObject> activeList = new ();

    void Start() => this.InitGameObject();

    // Khởi tạo pool với số lượng ban đầu
    protected virtual void InitGameObject()
    {
        for (int i = 0; i < listSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            this.poolList.Add(obj);
        }

        Debug.Log("Khởi tạo số lượng ban đầu: " + this.poolList.Count);
    }
    
    [ProButton]
    public void GetObject()
    {
        if (this.poolList.Count > 0)
        {
            // Lấy Object nếu đã có trong PoolList
            GameObject obj = this.poolList[poolList.Count - 1];
            obj.SetActive(true);

            this.poolList.Remove(obj);
            this.activeList.Add(obj);

            Debug.Log(obj.name + " đã được lấy từ Pool");
        }
        else
        {
            // Pool rỗng -> tạo mới
            GameObject obj = Instantiate(prefab);
            obj.SetActive(true);

            this.activeList.Add(obj);

            Debug.Log(obj.name + " không có trong Pool, đã được tạo mới");
        }
    }

    [ProButton]
    public void ReturnObjectToPool()
    {
        // Đưa Object vào lại PoolList khi không cần dùng nữa
        if (this.activeList.Count <= 0)
        {
            Debug.Log("Không có Object nào đang active");
            return;
        }

        GameObject obj = this.activeList[0];
        obj.SetActive(false);   

        poolList.Add(obj);
        activeList.Remove(obj);

        Debug.Log(obj.name + " đã được thêm vào Pool");
    }
}
