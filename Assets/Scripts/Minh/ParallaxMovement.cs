using UnityEngine;

public class ParallaxMovement : MonoBehaviour
{
    private float lengthX, lengthY, startPosX, startPosY;
    public GameObject Camera;
    public float speedX = 0.5f; 
    public float speedY = 0.5f; 

    void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        if (sprite != null)
        {
            lengthX = sprite.bounds.size.x;
            lengthY = sprite.bounds.size.y;
        }
    }

    void Update()
    {
        float tmpX = (Camera.transform.position.x * (1 - speedX));
        float tmpY = (Camera.transform.position.y * (1 - speedY));

        float distanceX = (Camera.transform.position.x * speedX);
        float distanceY = (Camera.transform.position.y * speedY);

        transform.position = new Vector3(startPosX + distanceX, startPosY + distanceY, transform.position.z);

        if (tmpX > startPosX + lengthX) startPosX += lengthX;
        else if (tmpX < startPosX - lengthX) startPosX -= lengthX;

        if (tmpY > startPosY + lengthY) startPosY += lengthY;
        else if (tmpY < startPosY - lengthY) startPosY -= lengthY;
    }
}