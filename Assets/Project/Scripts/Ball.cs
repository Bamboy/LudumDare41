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
		
	public AudioClip wallImpact;
	public AudioClip blockImpact;

	public int hits;

	public float targetSpeed = 2f;
	public float accelToTargetSpeed = 0.01f;

	public float upperSpeed = 25f;
	public float deccelToUpperSpeed = 1f;


	[SerializeField]
	private Vector2 velocity;

	private Rigidbody2D rbody;
	private CircleCollider2D col;

	private AudioSource player;
	void Start()
	{
		col = GetComponent<CircleCollider2D>();
		rbody = GetComponent<Rigidbody2D>();
		player = GetComponent<AudioSource>();

		Physics2D.showColliderAABB = true;

		rbody.AddForce(velocity, ForceMode2D.Impulse);
	}

	private int wallBounces = 0;
	void OnCollisionEnter2D( Collision2D impact )
	{
		BoardUITile tile = impact.gameObject.GetComponent<BoardUITile>();
		if( tile != null )
		{
			wallBounces = 0;
			if( tile.owner == null )
			{
				player.PlayOneShot( blockImpact );
				GameManager.singleton.board[tile.x,tile.y] = 0;

				GameObject glitchedTile = Instantiate<GameObject>( GameManager.singleton.boardRendering.tilePrefab, tile.transform.position, Quaternion.identity, tile.transform );

				BoardUITile visual = glitchedTile.GetComponent<BoardUITile>();
				visual.Initalize( tile.token, true, GameManager.singleton.tileGlitchTime );

				GameManager.singleton.ballHitsCombo++;
				return;
			}
			else
			{
				//We hit a falling tetris piece
			}
		}
		else
		{
			//We hit a wall or something?

			wallBounces++;
			if( wallBounces > 2 )
			{
				GameManager.singleton.FinishCombo();
			}
			
			player.PlayOneShot( wallImpact );
		}
	}

	void FixedUpdate()
	{

		Vector2 additionalForce = Vector2.zero;

		if( rbody.velocity.x < targetSpeed ) //Prevent getting stuck on a single axis.
			additionalForce.x = Mathf.Sign(rbody.velocity.x) * accelToTargetSpeed;
		else if( rbody.velocity.x > upperSpeed )
			additionalForce.x = -Mathf.Sign(rbody.velocity.x) * deccelToUpperSpeed;

		if( rbody.velocity.y < targetSpeed )
			additionalForce.y = Mathf.Sign(rbody.velocity.y) * accelToTargetSpeed;
		else if( rbody.velocity.x > upperSpeed )
			additionalForce.y = -Mathf.Sign(rbody.velocity.y) * deccelToUpperSpeed;

		if( additionalForce != Vector2.zero )
			rbody.AddForce( additionalForce );
		
	}

}
