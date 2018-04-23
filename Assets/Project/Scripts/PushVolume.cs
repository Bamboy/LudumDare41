using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushVolume : MonoBehaviour 
{

	public Vector2 pushDirection = Vector2.right;

	public float pushForce = 0.02f;

	public float pushDelay = 0.5f;

	private float _timeLeft;
	private Rigidbody2D ballRB;
	private Collider2D ballCollider;
	void OnTriggerEnter2D( Collider2D other )
	{
		if( other.GetComponent<Ball>() != null )
		{
			ballCollider = other;
			ballRB = other.GetComponent<Rigidbody2D>();
			_timeLeft = pushDelay;
		}
	}



	void OnTriggerStay2D( Collider2D other )
	{
		if( ballRB != null && other == ballCollider )
		{
			_timeLeft -= Time.deltaTime;

			if( _timeLeft <= 0f )
			{
				ballRB.AddForce( pushDirection * pushForce );
			}
		}
	}
	void OnTriggerExit2D( Collider2D other )
	{
		ballCollider = null;
		ballRB = null;
		_timeLeft = pushDelay;
	}
}
