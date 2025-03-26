using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightFlicker : MonoBehaviour
{
    public Light2D spotLight; // Light chiếu quanh nhân vật
    public float maxRadius = 8f; // Kích thước sáng ban đầu
    public float minRadius = 2f; // Kích thước thu nhỏ tối đa
    public float flickerSpeed = 0.2f; // Tốc độ thay đổi ánh sáng

    void Start()
    {
        spotLight.pointLightOuterRadius = maxRadius; // Ban đầu sáng rộng
        StartCoroutine(FlickerLight());
    }

    IEnumerator FlickerLight()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 15f)); // Chờ random trước mỗi lần hiệu ứng

            // 🔦 **Thu nhỏ ánh sáng**
            for (float r = maxRadius; r >= minRadius; r -= 0.5f)
            {
                spotLight.pointLightOuterRadius = r;
                yield return new WaitForSeconds(flickerSpeed);
            }

            yield return new WaitForSeconds(Random.Range(2f, 4f)); // Giữ ánh sáng nhỏ trong 2 - 4 giây

            // 🔆 **Mở rộng ánh sáng lại**
            for (float r = minRadius; r <= maxRadius; r += 0.5f)
            {
                spotLight.pointLightOuterRadius = r;
                yield return new WaitForSeconds(flickerSpeed);
            }
        }
    }
}
