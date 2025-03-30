using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderPool : MonoBehaviour
{
    public GameObject thunderPrefab;  // Prefab của Thunder
    public int poolSize = 5;  // Số lượng tối đa của Thunder trong pool
    private Queue<GameObject> thunderPool = new Queue<GameObject>();

    void Start()
    {
        // Tạo một pool với các đối tượng Thunder
        for (int i = 0; i < poolSize; i++)
        {
            GameObject thunder = Instantiate(thunderPrefab);
            thunder.SetActive(false);  // Đảm bảo tất cả các Thunder trong pool đều ẩn
            thunderPool.Enqueue(thunder);
        }
    }

    // Lấy một đối tượng Thunder từ pool
    public GameObject GetThunder()
    {
        if (thunderPool.Count > 0)
        {
            GameObject thunder = thunderPool.Dequeue();
            thunder.SetActive(true);  // Kích hoạt đối tượng Thunder
            Debug.Log("Thunder activated at position: " + thunder.transform.position);  // Kiểm tra vị trí
            return thunder;
        }
        else
        {
            // Nếu pool đã hết, tạo mới Thunder
            GameObject thunder = Instantiate(thunderPrefab);
            thunder.SetActive(true);  // Kích hoạt đối tượng mới
            return thunder;
        }
    }


    // Trả lại Thunder vào pool
    public void ReturnThunderToPool(GameObject thunder)
    {
        thunder.SetActive(false);  // Tắt đối tượng Thunder
        thunderPool.Enqueue(thunder);  // Đưa lại vào pool
    }
}
