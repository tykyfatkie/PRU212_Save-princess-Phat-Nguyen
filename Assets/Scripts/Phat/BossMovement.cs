using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform[] waypoints; // Các điểm dừng của boss

    private int currentWaypointIndex = 0;

    void Update()
    {
        MoveBoss();
    }

    void MoveBoss()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Chuyển sang điểm dừng tiếp theo
        }
    }
}
