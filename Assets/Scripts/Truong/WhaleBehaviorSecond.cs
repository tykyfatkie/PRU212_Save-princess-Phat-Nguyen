using UnityEngine;

public class WhaleBehaviorSecond : MonoBehaviour
{
    public float floatUpY = -510.7f;        // Vị trí Y khi nổi lên
    public float floatUpDuration = 3f;      // Thời gian nổi lên (3 giây)
    public float swimDuration = 10f;        // Thời gian bơi vòng vòng trước khi nổi lên lại
    public float swimSpeed = 5f;            // Tốc độ bơi vòng vòng
    public float swimRangeX = 50f;          // Phạm vi bơi vòng vòng theo trục X
    public float swimRangeY = 20f;          // Phạm vi bơi vòng vòng theo trục Y
    public float swimMinY = -530f;          // Giới hạn Y tối thiểu khi bơi
    public WhaleBehavior firstWhale;        // Tham chiếu đến con cá voi đầu tiên

    private Vector3 initialPosition;        // Vị trí ban đầu của con cá
    private Vector3 targetPosition;         // Vị trí mục tiêu khi bơi vòng vòng
    private float timer;                    // Bộ đếm thời gian
    private bool isFloatingUp = false;      // Trạng thái nổi lên (bắt đầu là false để xen kẽ với con cá voi đầu tiên)
    private bool isSwimming = true;         // Trạng thái bơi vòng vòng (bắt đầu là true)
    private PolygonCollider2D collider;     // Collider để người chơi đứng lên

    void Start()
    {
        initialPosition = transform.position; // Lưu vị trí ban đầu
        collider = GetComponent<PolygonCollider2D>();
        if (collider == null)
        {
            Debug.LogError("Con cá voi thứ hai cần PolygonCollider2D để người chơi đứng lên!");
        }
        else
        {
            collider.isTrigger = false; // Đảm bảo collider không phải trigger để người chơi có thể đứng lên
        }

        if (firstWhale == null)
        {
            Debug.LogError("Chưa gán con cá voi đầu tiên (First Whale)! Con cá voi thứ hai không thể hoạt động.");
        }

        // Bắt đầu ở trạng thái bơi vòng vòng (xen kẽ với con cá voi đầu tiên)
        timer = 0f;
        isFloatingUp = false;
        isSwimming = true;
        PickNewTargetPosition();
    }

    void Update()
    {
        if (firstWhale == null) return;

        timer += Time.deltaTime;

        // Kiểm tra trạng thái của con cá voi đầu tiên để quyết định hành vi
        if (firstWhale.IsSwimming()) // Nếu con cá voi đầu tiên đang bơi vòng vòng
        {
            // Con cá voi thứ hai nổi lên
            if (!isFloatingUp)
            {
                isFloatingUp = true;
                isSwimming = false;
                timer = 0f;
                Debug.Log("Con cá voi thứ hai nổi lên vì con cá voi đầu tiên đang bơi vòng vòng.");
            }
        }
        else // Nếu con cá voi đầu tiên đang nổi lên
        {
            // Con cá voi thứ hai bơi vòng vòng
            if (!isSwimming)
            {
                isSwimming = true;
                isFloatingUp = false;
                timer = 0f;
                PickNewTargetPosition();
                Debug.Log("Con cá voi thứ hai bơi vòng vòng vì con cá voi đầu tiên đang nổi lên.");
            }
        }

        if (isFloatingUp)
        {
            FloatUp();
        }
        else if (isSwimming)
        {
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
            // Chuyển sang trạng thái bơi vòng vòng (nếu con cá voi đầu tiên vẫn đang bơi)
            if (!firstWhale.IsSwimming())
            {
                isFloatingUp = false;
                isSwimming = true;
                timer = 0f;
                PickNewTargetPosition();
                Debug.Log("Con cá voi thứ hai bắt đầu bơi vòng vòng.");
            }
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
            // Chuyển sang trạng thái nổi lên (nếu con cá voi đầu tiên vẫn đang nổi lên)
            if (firstWhale.IsSwimming())
            {
                isFloatingUp = true;
                isSwimming = false;
                timer = 0f;
                Debug.Log("Con cá voi thứ hai nổi lên lại.");
            }
        }
    }

    void PickNewTargetPosition()
    {
        // Chọn một vị trí ngẫu nhiên trong phạm vi bơi
        float randomX = Random.Range(initialPosition.x - swimRangeX, initialPosition.x + swimRangeX);
        float randomY = Random.Range(swimMinY, initialPosition.y);
        targetPosition = new Vector3(randomX, randomY, initialPosition.z);
        Debug.Log($"Con cá voi thứ hai chọn vị trí mục tiêu mới: {targetPosition}");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Nếu người chơi va chạm với con cá, đặt người chơi làm con của con cá để di chuyển cùng
        if (collision.gameObject.CompareTag("Player") && isFloatingUp)
        {
            collision.transform.SetParent(transform);
            Debug.Log("Người chơi đứng lên đầu con cá voi thứ hai!");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Khi người chơi rời khỏi con cá, bỏ parent
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
            Debug.Log("Người chơi rời khỏi đầu con cá voi thứ hai.");
        }
    }
}