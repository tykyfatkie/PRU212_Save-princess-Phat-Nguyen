using UnityEngine;

public class CameraZoom2D : MonoBehaviour
{
    [SerializeField] public Camera mainCamera;
    [SerializeField] public float zoomInSize = 5f;
    [SerializeField] public float zoomOutSize = 10f;
    [SerializeField] public float zoomSpeed = 5f; 

    
    void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("Player"))
        {
           
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, zoomInSize, zoomSpeed * Time.deltaTime);
        }
    }

   
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, zoomOutSize, zoomSpeed * Time.deltaTime);
        }
    }
}
