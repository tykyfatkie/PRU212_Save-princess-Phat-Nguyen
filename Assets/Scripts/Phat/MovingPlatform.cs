using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 30f;
    private Vector3 target;
    private Transform player;
    private bool isPlayerOnPlatform = false;

    void Start()
    {
        target = pointA.position;
    }

    void Update()
    {
        // Chỉ di chuyển platform khi player đã lên
        if (isPlayerOnPlatform)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.1f)
            {
                if (target == pointA.position)
                {
                    target = pointB.position;
                }
                else if (target == pointB.position)
                {
                    target = pointA.position;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {         
            player = collision.transform;
            isPlayerOnPlatform = true;
            
            //player.SetParent(transform);         
            //Vector3 originalScale = player.localScale;
            //player.localScale = originalScale;
        }
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        // Khi player rời khỏi platform
    //        player.SetParent(null);
    //        isPlayerOnPlatform = false;
    //    }
    //}
}
