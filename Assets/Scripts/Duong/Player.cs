using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	public float runSpeed = 10f;
	public float jumpPower = 5f;
	bool isFacingRight = true;
	public Vector2 boxSize;
	public float castDistance;
	public LayerMask groundLayer;

	Vector2 moveInput;
	Rigidbody2D myRigidbody;
	Animator animator;

	void Start()
	{
		myRigidbody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		Run();
		FlipSprite();
		animator.SetFloat("yVelocity", myRigidbody.linearVelocity.y);
		if (isGrounded() && myRigidbody.linearVelocity.y == 0)
		{
			animator.SetBool("isJumping", false);
		}
	}

	void OnMove(InputValue value)
	{
		moveInput = value.Get<Vector2>();
		//Debug.Log(moveInput);
	}

	void OnJump(InputValue value)
	{

		if (value.isPressed && isGrounded())
		{
			Vector2 playerVelocity = new Vector2(myRigidbody.linearVelocity.x, jumpPower);
			myRigidbody.linearVelocity = playerVelocity;
			animator.SetBool("isJumping", true);
		}
	}
	void Run()
	{
		Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.linearVelocity.y);
		myRigidbody.linearVelocity = playerVelocity;
		animator.SetFloat("xVelocity", Math.Abs(playerVelocity.x));
	}

	void FlipSprite()
	{
		bool rightToLeft = isFacingRight && moveInput.x < 0f;
		bool leftToRight = !isFacingRight && moveInput.x > 0f;

		if (rightToLeft || leftToRight)
		{
			isFacingRight = !isFacingRight;
			Vector2 ls = transform.localScale;
			ls.x *= -1f;
			transform.localScale = ls;
		}
	}

	public bool isGrounded()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, castDistance, groundLayer);
		return hit.collider != null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + Vector3.down * castDistance);
	}

}
