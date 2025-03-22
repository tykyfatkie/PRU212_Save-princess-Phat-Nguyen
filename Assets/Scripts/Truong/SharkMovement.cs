using UnityEngine;

public class SharkMovement : MonoBehaviour
{
    public float speed = 40f;               // Tốc độ di chuyển của cá mập
    public float rotationSpeed = 2f;        // Tốc độ xoay đầu
    public float changeDirectionTime = 3f;  // Thời gian thay đổi hướng
    public float maxTiltAngle = 30f;        // Góc nghiêng tối đa khi bơi lên/xuống
    public GameObject boundaryObject;       // Tham chiếu đến GameObject làm boundary
    public float minX = 154f;             // Giới hạn tọa độ X tối thiểu
    public float maxX = 630.5f;             // Giới hạn tọa độ X tối đa
    public float minY = -541.3f;            // Giới hạn tọa độ Y tối thiểu
    public float maxY = -511.2f;            // Giới hạn tọa độ Y tối đa
    public float detectionRadius = 50f;     // Bán kính quan sát để phát hiện người chơi
    public Transform player;                // Tham chiếu đến Transform của người chơi
    public float chaseSpeedMultiplier = 1.5f; // Hệ số tốc độ khi đuổi theo (1.5 lần)

    private Vector3 targetDirection;        // Hướng di chuyển mục tiêu (3D)
    private float timer;                    // Bộ đếm thời gian
    private Animator animator;              // Điều khiển hoạt ảnh
    private PolygonCollider2D boundary;     // Giới hạn di chuyển (dùng PolygonCollider2D)
    private bool isChasingPlayer = false;   // Trạng thái đuổi theo người chơi
    private float currentSpeed;             // Tốc độ hiện tại (thay đổi khi đuổi theo)

    void Start()
    {
        animator = GetComponent<Animator>();

        if (boundaryObject != null)
        {
            boundary = boundaryObject.GetComponent<PolygonCollider2D>();
            if (boundary == null)
            {
                Debug.LogError($"GameObject {boundaryObject.name} không có PolygonCollider2D! Cá mập sẽ di chuyển tự do.");
            }
            else
            {
                // Kiểm tra xem vị trí ban đầu có nằm trong boundary không
                Vector2 point = new Vector2(transform.position.x, transform.position.y);
                if (!boundary.OverlapPoint(point))
                {
                    Debug.LogWarning($"Vị trí ban đầu của {gameObject.name} (x: {transform.position.x}, y: {transform.position.y}) nằm ngoài boundary! Cá mập có thể không di chuyển đúng.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Boundary Object chưa được gán! Cá mập sẽ di chuyển tự do.");
        }

        // Tìm người chơi nếu chưa gán
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogWarning("Không tìm thấy người chơi với tag 'Player'! Cá mập sẽ không đuổi theo.");
            }
        }

        RandomizeDirection();
        currentSpeed = speed; // Khởi tạo tốc độ ban đầu

        if (animator != null)
        {
            animator.SetBool("IsSwimming", true);
        }
    }

    void Update()
    {
        // Kiểm tra xem người chơi có trong tầm quan sát không
        CheckForPlayer();

        if (isChasingPlayer)
        {
            // Đuổi theo người chơi
            ChasePlayer();
        }
        else
        {
            // Di chuyển ngẫu nhiên
            timer += Time.deltaTime;
            if (timer >= changeDirectionTime)
            {
                RandomizeDirection();
                timer = Random.Range(0f, changeDirectionTime * 0.5f);
            }
        }

        MoveShark();
        UpdateRotationAndTilt();
    }

    void CheckForPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRadius)
        {
            // Người chơi trong tầm quan sát, chuyển sang trạng thái đuổi theo
            if (!isChasingPlayer)
            {
                isChasingPlayer = true;
                currentSpeed = speed * chaseSpeedMultiplier; // Tăng tốc độ khi đuổi theo
                Debug.Log($"{gameObject.name} phát hiện người chơi và bắt đầu đuổi theo!");
            }
        }
        else
        {
            // Người chơi ngoài tầm quan sát, trở lại trạng thái di chuyển ngẫu nhiên
            if (isChasingPlayer)
            {
                isChasingPlayer = false;
                currentSpeed = speed; // Trở lại tốc độ ban đầu
                RandomizeDirection();
                Debug.Log($"{gameObject.name} mất dấu người chơi, trở lại di chuyển ngẫu nhiên.");
            }
        }
    }

    void ChasePlayer()
    {
        // Hướng về phía người chơi
        targetDirection = (player.position - transform.position).normalized;
    }

    void MoveShark()
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition + targetDirection * currentSpeed * Time.deltaTime;

        // Giới hạn tọa độ X trong khoảng minX (136.9) đến maxX (630.5)
        if (newPosition.x < minX)
        {
            newPosition.x = minX;
            targetDirection.x = 0;
            targetDirection = targetDirection.normalized;
            Debug.Log("Đạt giới hạn X tối thiểu, đổi hướng: " + targetDirection);
        }
        else if (newPosition.x > maxX)
        {
            newPosition.x = maxX;
            targetDirection.x = 0;
            targetDirection = targetDirection.normalized;
            Debug.Log("Đạt giới hạn X tối đa, đổi hướng: " + targetDirection);
        }

        // Giới hạn tọa độ Y trong khoảng minY (-541.3) đến maxY (-511.2)
        if (newPosition.y > maxY)
        {
            newPosition.y = maxY;
            targetDirection.y = 0;
            targetDirection = targetDirection.normalized;
            Debug.Log("Đạt giới hạn Y tối đa, đổi hướng: " + targetDirection);
        }
        else if (newPosition.y < minY)
        {
            newPosition.y = minY;
            targetDirection.y = 0;
            targetDirection = targetDirection.normalized;
            Debug.Log("Đạt giới hạn Y tối thiểu, đổi hướng: " + targetDirection);
        }

        // Kiểm tra boundary
        if (boundary != null && !IsInsideBoundary(newPosition))
        {
            targetDirection = -targetDirection;
            newPosition = currentPosition + targetDirection * currentSpeed * Time.deltaTime;
            Debug.Log("Chạm biên, đổi hướng: " + targetDirection);
        }

        // Debug vị trí hiện tại và vị trí mới
        Debug.Log($"Current Position: {currentPosition}, New Position: {newPosition}, Target Direction: {targetDirection}");

        transform.position = newPosition;
    }

    void UpdateRotationAndTilt()
    {
        if (targetDirection == Vector3.zero) return;

        // Xoay cá mập theo hướng di chuyển (3D)
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Nghiêng đầu khi bơi lên/xuống
        float verticalSpeed = targetDirection.y;
        float tiltAngle = Mathf.Clamp(verticalSpeed * maxTiltAngle, -maxTiltAngle, maxTiltAngle);
        Vector3 currentEuler = transform.localEulerAngles;
        float targetTilt = Mathf.LerpAngle(currentEuler.x, -tiltAngle, rotationSpeed * Time.deltaTime);
        transform.localEulerAngles = new Vector3(targetTilt, currentEuler.y, currentEuler.z);
    }

    void RandomizeDirection()
    {
        // Tạo hướng ngẫu nhiên trong không gian 3D với phạm vi rộng
        float randomX = Random.Range(-10f, 10f);
        float randomY = Random.Range(-6f, 6f);
        float randomZ = Random.Range(-10f, 10f);

        targetDirection = new Vector3(randomX, randomY, randomZ).normalized;

        if (animator != null)
        {
            animator.SetFloat("SwimSpeed", currentSpeed);
        }

        Debug.Log("New Direction: " + targetDirection);
    }

    bool IsInsideBoundary(Vector3 position)
    {
        if (boundary == null) return true;

        Vector2 point = new Vector2(position.x, position.y);
        return boundary.OverlapPoint(point);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + targetDirection * 2);

        if (boundary != null)
        {
            Gizmos.color = Color.red;
            Vector2[] points = boundary.points;
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 p1 = points[i] + (Vector2)boundary.transform.position;
                Vector2 p2 = points[(i + 1) % points.Length] + (Vector2)boundary.transform.position;
                Gizmos.DrawLine(p1, p2);
            }
        }

        // Vẽ đường giới hạn X và Y
        Gizmos.color = Color.yellow;
        // Giới hạn X
        Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(minX, maxY, 0));
        Gizmos.DrawLine(new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0));
        // Giới hạn Y
        Gizmos.DrawLine(new Vector3(minX, maxY, 0), new Vector3(maxX, maxY, 0));
        Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(maxX, minY, 0));

        // Vẽ bán kính quan sát
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}