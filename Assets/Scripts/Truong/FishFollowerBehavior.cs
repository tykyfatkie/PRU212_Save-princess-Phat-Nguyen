using UnityEngine;

public class FishFollowerBehavior : MonoBehaviour
{
    public Transform leader;            // Tham chiếu đến con cá dẫn đầu
    public float followSpeed = 8f;      // Tốc độ di chuyển của con cá theo sau
    public float cohesionDistance = 10f; // Khoảng cách để giữ gần nhau (cohesion)
    public float separationDistance = 2f; // Khoảng cách để tránh va chạm (separation)
    public float alignmentWeight = 1f;   // Trọng số để di chuyển cùng hướng (alignment)
    public float cohesionWeight = 2f;    // Trọng số để giữ gần nhau (tăng lên để bám sát hơn)
    public float separationWeight = 1f;  // Trọng số để tránh va chạm
    public float waveAmplitude = 1f;     // Biên độ uốn lượn (giảm xuống để dao động nhẹ hơn)
    public float waveFrequency = 1f;     // Tần số uốn lượn
    public float yFollowStrength = 5f;   // Sức mạnh để giữ Y của các con cá con gần với Y của con cá dẫn đầu

    private Vector3 velocity;           // Vận tốc hiện tại của con cá
    private FishFollowerBehavior[] flock; // Danh sách các con cá trong bầy
    private FishLeaderBehavior leaderBehavior; // Tham chiếu đến script của con cá dẫn đầu

    void Start()
    {
        if (leader == null)
        {
            Debug.LogError("Chưa gán con cá dẫn đầu (Leader)! Con cá này không thể di chuyển.");
            return;
        }

        // Lấy script FishLeaderBehavior từ con cá dẫn đầu
        leaderBehavior = leader.GetComponent<FishLeaderBehavior>();
        if (leaderBehavior == null)
        {
            Debug.LogError("Con cá dẫn đầu không có script FishLeaderBehavior! Con cá này không thể đồng bộ chuyển động.");
            return;
        }

        // Tìm tất cả các con cá trong bầy (các GameObject có script FishFollowerBehavior)
        flock = FindObjectsOfType<FishFollowerBehavior>();

        // Debug kích thước của con cá
        Debug.Log($"Kích thước của {gameObject.name}: Scale = {transform.localScale}, SpriteRenderer bounds = {GetComponent<SpriteRenderer>().bounds.size}");
    }

    void Update()
    {
        if (leader == null || leaderBehavior == null) return;

        // Tính các lực flocking
        Vector3 cohesion = CalculateCohesion();
        Vector3 separation = CalculateSeparation();
        Vector3 alignment = CalculateAlignment();

        // Kết hợp các lực
        Vector3 desiredVelocity = (cohesion * cohesionWeight + separation * separationWeight + alignment * alignmentWeight).normalized * followSpeed;

        // Cập nhật vận tốc
        velocity = Vector3.Lerp(velocity, desiredVelocity, Time.deltaTime * 2f);

        // Tính vị trí mới
        Vector3 newPosition = transform.position + velocity * Time.deltaTime;

        // Đồng bộ chuyển động uốn lượn với con cá dẫn đầu
        float waveTimer = leaderBehavior.GetWaveTimer();
        float waveOffset = Mathf.Sin(waveTimer * waveFrequency) * waveAmplitude;
        float targetY = leader.position.y + waveOffset;

        // Kéo vị trí Y của con cá con về gần Y của con cá dẫn đầu
        newPosition.y = Mathf.Lerp(newPosition.y, targetY, yFollowStrength * Time.deltaTime);

        // Cập nhật vị trí
        transform.position = newPosition;

        // Xoay con cá theo hướng di chuyển
        if (velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Hướng trái
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1); // Hướng phải
        }
    }

    Vector3 CalculateCohesion()
    {
        // Tính trung tâm của bầy (bao gồm cả con cá dẫn đầu)
        Vector3 center = leader.position;
        int count = 1; // Đếm con cá dẫn đầu

        foreach (FishFollowerBehavior fish in flock)
        {
            if (fish != this && Vector3.Distance(transform.position, fish.transform.position) < cohesionDistance)
            {
                center += fish.transform.position;
                count++;
            }
        }

        center /= count;
        return (center - transform.position).normalized;
    }

    Vector3 CalculateSeparation()
    {
        // Tránh va chạm với các con cá khác
        Vector3 separation = Vector3.zero;

        foreach (FishFollowerBehavior fish in flock)
        {
            if (fish != this)
            {
                float distance = Vector3.Distance(transform.position, fish.transform.position);
                if (distance < separationDistance && distance > 0)
                {
                    separation += (transform.position - fish.transform.position).normalized / distance;
                }
            }
        }

        return separation.normalized;
    }

    Vector3 CalculateAlignment()
    {
        // Di chuyển cùng hướng với con cá dẫn đầu
        Vector3 leaderVelocity = (leader.position - transform.position).normalized;
        return leaderVelocity;
    }
}