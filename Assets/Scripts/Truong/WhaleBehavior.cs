using UnityEngine;

public class WhaleBehavior : MonoBehaviour
{
    public float floatUpY = -510.7f;        // Vị trí Y khi nổi lên
    public float floatUpDuration = 3f;      // Thời gian nổi lên (3 giây)
    public float swimDuration = 10f;        // Thời gian bơi vòng vòng trước khi nổi lên lại
    public float swimSpeed = 5f;            // Tốc độ bơi vòng vòng
    public float swimRangeX = 50f;          // Phạm vi bơi vòng vòng theo trục X
    public float swimRangeY = 20f;          // Phạm vi bơi vòng vòng theo trục Y
    public float swimMinY = -530f;          // Giới hạn Y tối thiểu khi bơi

    private Vector3 initialPosition;        // Vị trí ban đầu của con cá
    private Vector3 targetPosition;         // Vị trí mục tiêu khi bơi vòng vòng
    private float timer;                    // Bộ đếm thời gian
    private bool isFloatingUp = true;       // Trạng thái nổi lên
    private bool isSwimming = false;        // Trạng thái bơi vòng vòng
    private PolygonCollider2D collider;     // Collider để người chơi đứng lên

    void Start()
    {
        initialPosition = transform.position; // Lưu vị trí ban đầu
        collider = GetComponent<PolygonCollider2D>();
        if (collider == null)
        {
            Debug.LogError("Con cá cần PolygonCollider2D để người chơi đứng lên!");
        }
        else
        {
            collider.isTrigger = false; // Đảm bảo collider không phải trigger để người chơi có thể đứng lên
        }

        // Bắt đầu ở trạng thái nổi lên
        timer = 0f;
        isFloatingUp = true;
        isSwimming = false;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (isFloatingUp)
        {
            // Nổi lên vị trí floatUpY trong floatUpDuration (3 giây)
            FloatUp();
        }
        else if (isSwimming)
        {
            // Bơi vòng vòng trong swimDuration
            SwimAround();
        }
    }

    void FloatUp()
    {
        // Di chuyển con cá lên vị trí floatUpY
        Vector3 targetPos = new Vector3(transform.position.x, floatUpY, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, swimSpeed * Time.deltaTime);

        // Kiểm tra xem đã nổi lên đủ lâu chưa
        if (timer >= floatUpDuration)
        {
            // Chuyển sang trạng thái bơi vòng vòng
            isFloatingUp = false;
            isSwimming = true;
            timer = 0f;
            PickNewTargetPosition();
            Debug.Log("Con cá voi đầu tiên bắt đầu bơi vòng vòng.");
        }
    }

    void SwimAround()
    {
        // Di chuyển đến vị trí mục tiêu
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, swimSpeed * Time.deltaTime);

        // Giới hạn Y tối thiểu khi bơi
        if (transform.position.y < swimMinY)
        {
            transform.position = new Vector3(transform.position.x, swimMinY, transform.position.z);
            PickNewTargetPosition();
        }

        // Nếu đến vị trí mục tiêu, chọn vị trí mới
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            PickNewTargetPosition();
        }

        // Kiểm tra xem đã bơi đủ lâu chưa
        if (timer >= swimDuration)
        {
            // Chuyển sang trạng thái nổi lên
            isFloatingUp = true;
            isSwimming = false;
            timer = 0f;
            Debug.Log("Con cá voi đầu tiên nổi lên lại.");
        }
    }

    void PickNewTargetPosition()
    {
        // Chọn một vị trí ngẫu nhiên trong phạm vi bơi
        float randomX = Random.Range(initialPosition.x - swimRangeX, initialPosition.x + swimRangeX);
        float randomY = Random.Range(swimMinY, initialPosition.y);
        targetPosition = new Vector3(randomX, randomY, initialPosition.z);
        Debug.Log($"Con cá voi đầu tiên chọn vị trí mục tiêu mới: {targetPosition}");
    }

    public bool IsSwimming()
    {
        return isSwimming;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Nếu người chơi va chạm với con cá, đặt người chơi làm con của con cá để di chuyển cùng
        if (collision.gameObject.CompareTag("Player") && isFloatingUp)
        {
            collision.transform.SetParent(transform);
            Debug.Log("Người chơi đứng lên đầu con cá voi đầu tiên!");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Khi người chơi rời khỏi con cá, bỏ parent
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
            Debug.Log("Người chơi rời khỏi đầu con cá voi đầu tiên.");
        }
    }
}