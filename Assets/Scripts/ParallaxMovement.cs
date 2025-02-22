using UnityEngine;

public class ParallaxMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float length, startPos;
    public GameObject Camera;
    public float speed;
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float tmp = (Camera.transform.position.x * (1 - speed));
        float distance = (Camera.transform.position.x * speed);

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if(tmp > startPos + length) startPos += length;
        else if (tmp < startPos - length) startPos -= length;
    }
}
