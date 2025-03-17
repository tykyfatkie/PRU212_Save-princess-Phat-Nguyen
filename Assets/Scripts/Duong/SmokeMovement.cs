using UnityEngine;

public class SmokeMovement : MonoBehaviour
{
	public float speed = 1f;
	public float distance = 2f;

	private Vector3 startPosition;

	void Start()
	{
		startPosition = transform.position;
	}

	void Update()
	{
		float offset = Mathf.PingPong(Time.time * speed, distance * 2) - distance;
		transform.position = new Vector3(startPosition.x + offset, startPosition.y, startPosition.z);
	}
}
