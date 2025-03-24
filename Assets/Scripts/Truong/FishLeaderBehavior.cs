using UnityEngine;

public class FishLeaderBehavior : MonoBehaviour
{
    public float startX = 100f;         // Điểm X bắt đầu
    public float endX = 600f;           // Điểm X kết thúc
    public float speed = 10f;           // Tốc độ di chuyển của con cá dẫn đầu
    public float swimY = -520f;         // Vị trí Y trung tâm khi bơi
    public float waveAmplitude = 5f;    // Biên độ uốn lượn theo trục Y
    public float waveFrequency = 1f;    // Tần số uốn lượn

    private Vector3 targetPosition;     // Vị trí mục tiêu hiện tại
    private bool movingToEnd = true;    // Trạng thái: true nếu đang di chuyển đến endX, false nếu quay lại startX
    private float waveTimer;            // Bộ đếm thời gian cho chuyển động uốn lượn

    void Start()
    {
        // Đặt vị trí ban đầu
        transform.position = new Vector3(startX, swimY, transform.position.z);
        targetPosition = new Vector3(endX, swimY, transform.position.z);

        // Debug kích thước của con cá
        Debug.Log($"Kích thước của {gameObject.name}: Scale = {transform.localScale}, SpriteRenderer bounds = {GetComponent<SpriteRenderer>().bounds.size}");
    }

    void Update()
    {
        // Tăng bộ đếm thời gian cho chuyển động uốn lượn
        waveTimer += Time.deltaTime;

        // Tính vị trí X mới
        float newX = transform.position.x + (movingToEnd ? speed : -speed) * Time.deltaTime;

        // Tính vị trí Y với chuyển động uốn lượn
        float waveOffset = Mathf.Sin(waveTimer * waveFrequency) * waveAmplitude;
        float newY = swimY + waveOffset;

        // Cập nhật vị trí
        transform.position = new Vector3(newX, newY, transform.position.z);

        // Xoay con cá theo hướng di chuyển
        if (movingToEnd)
        {
            transform.localScale = new Vector3(1, 1, 1); // Hướng phải
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // Hướng trái
        }

        // Kiểm tra xem đã đến điểm X mục tiêu chưa
        if (movingToEnd && transform.position.x >= endX)
        {
            movingToEnd = false;
            Debug.Log("Con cá dẫn đầu đến endX, quay lại startX.");
        }
        else if (!movingToEnd && transform.position.x <= startX)
        {
            movingToEnd = true;
            Debug.Log("Con cá dẫn đầu đến startX, quay lại endX.");
        }
    }

    // Phương thức để các con cá con truy cập waveTimer và đồng bộ chuyển động uốn lượn
    public float GetWaveTimer()
    {
        return waveTimer;
    }
}