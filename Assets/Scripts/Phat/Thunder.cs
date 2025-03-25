using System.Collections;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    public float fallSpeed = 5f;
    public float destroyTime = 2f;

    void Start()
    {
        Destroy(gameObject, destroyTime);  // Tự động hủy sau thời gian nhất định
    }

    void Update()
    {
        // Rơi từ trên xuống
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra va chạm với người chơi hoặc các vật thể khác
        if (collision.gameObject.CompareTag("Player"))
        {
            // Gọi hàm nhận sát thương cho người chơi tại đây
            Debug.Log("Player hit by Thunder!");
            // Ví dụ: collision.gameObject.GetComponent<PlayerController>().TakeDamage(50);
        }
    }
}

