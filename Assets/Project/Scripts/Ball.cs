using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Ball : MonoBehaviour 
{
	public float speed = 0.5f;

	//public float 

	private CircleCollider2D col;
	void Update()
	{

		Vector2 orign;

		RaycastHit2D[] data = Physics2D.RaycastAll( new Vector2(), new Vector2(), 1f );

		

	}

	void OnCollisionEnter2D( Collision2D impact )
	{



	}
}
