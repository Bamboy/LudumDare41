using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Ball : MonoBehaviour 
{
	public float speed
	{
		get{ return rbody.velocity.magnitude; }
		set{
			targetSpeed = value;
		}
	}

	public AudioClip paddleImpact;
	public AudioClip wallImpact;
	public AudioClip blockImpact;

	public bool customPhysics = false;

	public float targetSpeed = 2f;

	[SerializeField]
	private Vector2 velocity;

	private Rigidbody2D rbody;
	private CircleCollider2D col;

	[SerializeField]
	private float _collisionSkin = -0.02f;
	private AudioSource player;
	void Start()
	{
		col = GetComponent<CircleCollider2D>();
		rbody = GetComponent<Rigidbody2D>();
		player = GetComponent<AudioSource>();
		velocity = (Vector2.right + Vector2.down) * targetSpeed;

		Physics2D.showColliderAABB = true;

		rbody.AddForce(velocity, ForceMode2D.Impulse);

	}


	void OnCollisionEnter2D( Collision2D impact )
	{
		BoardUITile tile = impact.gameObject.GetComponent<BoardUITile>();
		if( tile != null && tile.owner == null )
		{
			player.PlayOneShot( blockImpact );
			GameManager.singleton.board[tile.x,tile.y] = 0; //This should trigger the vaporize glitch effect
			return;
		}

		Paddle pad = impact.gameObject.GetComponent<Paddle>();
		if( pad != null )
		{
			player.PlayOneShot( paddleImpact );
			return;
		}

		player.PlayOneShot( wallImpact );

	}

	void FixedUpdate()
	{
		if( customPhysics)
		{
			CustomPhysics();
		}
		else
		{

			if( rbody.velocity.magnitude < targetSpeed )
			{
				rbody.velocity = rbody.velocity.normalized * targetSpeed;
			}

		}
	}
	void CustomPhysics()
	{



		float time = Time.deltaTime;

		Vector2 pos = new Vector2(transform.position.x, transform.position.y);

		Vector2 newPos;
		Vector2 newVelocity = velocity;

		newPos.x = pos.x + (velocity.x * time) + (time * (time+1f)) / 2f;
		newPos.y = pos.y + (velocity.y * time) + (time * (time+1f)) / 2f;


		Vector2 movement = newPos - pos;


		RaycastHit2D[] data = Physics2D.CircleCastAll( pos, col.radius, movement, movement.magnitude );

		DebugExtension.DebugArrow(VectorExtras.V3FromV2(pos), VectorExtras.V3FromV2(movement), Color.green);

		foreach (RaycastHit2D hit in data) 
		{
			if( hit.collider == col )
				continue; //Ignore our own collider

			DebugExtension.DebugPoint( VectorExtras.V3FromV2(hit.point) );
			DebugExtension.DebugArrow( VectorExtras.V3FromV2(hit.point), VectorExtras.V3FromV2(hit.normal), Color.white );

			//Physics bouncing
			Vector2 velReflect = Vector2.Reflect( velocity, hit.normal );

			float distToCollision = Vector2.Distance( pos, hit.centroid );
			float remainingDist = Vector2.Distance( pos, newPos ) - distToCollision; //movement.magnitude?


			//Debug.Log(string.Format("CollisionDist: {0}, Remaining: {1}, TotalADD: {2}, TotalTarget: {3}", distToCollision, remainingDist, distToCollision + remainingDist, movement.magnitude));

			Vector2 remainingMove = movement.normalized * remainingDist;

			Vector2 reflection = Vector2.Reflect( remainingMove, hit.normal );


			newVelocity = velReflect;
			newPos = hit.centroid + reflection;

			DebugExtension.DebugArrow(VectorExtras.V3FromV2(hit.centroid), VectorExtras.V3FromV2(reflection), Color.red);
			//Debug.Break();

			break;
		}

		velocity = newVelocity;
		pos = newPos;
		transform.position = new Vector3( newPos.x, newPos.y, 0f );
	}


}
