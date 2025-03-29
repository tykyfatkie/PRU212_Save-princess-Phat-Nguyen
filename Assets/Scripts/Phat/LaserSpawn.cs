using System.Collections;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    public GameObject laserPrefab;  // Prefab của laser
    public Transform firePoint;     // Điểm phát ra laser
    public float spawnInterval = 5f; // Khoảng thời gian giữa các lần tạo laser
    public float destroyTime = 5f;  // Thời gian sống của laser

    private void Start()
    {
        // Bắt đầu Coroutine để tạo laser theo chu kỳ
        StartCoroutine(SpawnLaserRoutine());
    }

    // Coroutine tạo laser theo chu kỳ
    private IEnumerator SpawnLaserRoutine()
    {
        while (true)
        {
            // Tạo một laser mới tại FirePoint
            GameObject laser = Instantiate(laserPrefab, firePoint.position, Quaternion.identity);

            // Hủy laser sau destroyTime giây
            Destroy(laser, destroyTime);

            // Chờ một khoảng thời gian trước khi tạo laser tiếp theo
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

