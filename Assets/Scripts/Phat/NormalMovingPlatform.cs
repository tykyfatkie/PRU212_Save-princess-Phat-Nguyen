using UnityEngine;

public class NormalMovingPlatform : MonoBehaviour
{
    public float speed = 4f; // Speed of movement
    public float height = 20f; // Distance to move up and down

    private Vector3 startPosition;
    private bool movingUp = true;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float movement = speed * Time.deltaTime;
        if (movingUp)
        {
            transform.position += new Vector3(0, movement, 0);
            if (transform.position.y >= startPosition.y + height)
            {
                movingUp = false;
            }
        }
        else
        {
            transform.position -= new Vector3(0, movement, 0);
            if (transform.position.y <= startPosition.y)
            {
                movingUp = true;
            }
        }
    }
}