using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Sirenix.OdinInspector;

public class BoardUITile : MonoBehaviour 
{
	public BoxCollider2D collision;
	private TileAnimator render;

	public TextMeshPro text;

	public TetrisBlock owner;

	public int x;
	public int y;
	[ReadOnly]public int token = -int.MaxValue;

	/// Position of this tile in the world
	public Vector2Int position
	{
		get{ return new Vector2Int(Mathf.FloorToInt( transform.position.x ), Mathf.FloorToInt( transform.position.y )); }
		set{
			transform.position = new Vector3( value.x, value.y, 0f );
		}
	}

	void Awake()
	{
		render = GetComponent<TileAnimator>();
	}

	private bool _initalized = false;
	public void Initalize(int x, int y)
	{
		_initalized = true;
		this.x = x;
		this.y = y;

		text.text = string.Format("[{0}, {1}]", x, y);
	}
	public void Initalize(int x, int y, TetrisBlock owner)
	{
		if( text != null )
			Destroy( text.gameObject );

		_initalized = true;

		this.token = owner.template.token;
		this.owner = owner;

		collision.enabled = true;

		render.SetTiles( GameManager.singleton.tileSprites[this.token] );
	}

	public void Vaporize()
	{
		if( _initalized == false )
			return;

		if( token < 0 )
			token = 0;

		collision.enabled = false;
		render.SetTiles( GameManager.singleton.tileSprites[token], true, GameManager.singleton.tileGlitchTime );

		//StartCoroutine( GlitchThenRemove() );
	}

	#region Movement
	public bool CanMoveDown()
	{
		if( this.position.y < 0 )
			return false;

		RaycastHit2D[] data = Physics2D.RaycastAll( transform.position, Vector2.down, 1f );
		DebugExtension.DebugArrow( transform.position, Vector3.down, Color.blue );

		bool move = true;
		foreach (RaycastHit2D hit in data) 
		{
			if( hit.transform == this.transform )
				continue;

			Ball ball = hit.collider.gameObject.GetComponent<Ball>();
			Paddle paddle = hit.collider.gameObject.GetComponent<Paddle>();


			if( ball != null )
			{
				Vector2 dirFromCenter = VectorExtras.Direction(VectorExtras.V2FromV3(transform.position - Vector3.down), VectorExtras.V2FromV3(ball.transform.position));
				ball.GetComponent<Rigidbody2D>().AddForce( dirFromCenter * 10f, ForceMode2D.Impulse );
			}

			if( ball != null || paddle != null )
				continue;


			BoardUITile otherTile = hit.collider.gameObject.GetComponent<BoardUITile>();



			if( owner.tiles.Contains( otherTile ) ) //Ignore other tiles that are part of this tetris block
			{
				continue;
			}
			else
			{
				move = false;
				break;
			}
		}

		return move;
	}
	public bool CanMoveLeft()
	{
		if( this.position.x <= 0 )
			return false;

		RaycastHit2D[] data = Physics2D.RaycastAll( transform.position, Vector2.left, 1f );
		DebugExtension.DebugArrow( transform.position, Vector3.down, Color.cyan );

		bool move = true;
		foreach (RaycastHit2D hit in data) 
		{
			if( hit.transform == this.transform )
				continue;

			BoardUITile otherTile = hit.collider.gameObject.GetComponent<BoardUITile>();

			if( owner.tiles.Contains( otherTile ) ) //Ignore other tiles that are part of this tetris block
				continue;
			else
			{
				move = false;
				break;
			}
		}

		return move;
	}
	public bool CanMoveRight()
	{
		if( this.position.x >= GameManager.singleton.board.tiles.GetLength(0) - 1 )
			return false;

		RaycastHit2D[] data = Physics2D.RaycastAll( transform.position, Vector2.right, 1f );
		DebugExtension.DebugArrow( transform.position, Vector3.down, Color.cyan );

		bool move = true;
		foreach (RaycastHit2D hit in data) 
		{
			if( hit.transform == this.transform )
				continue;

			BoardUITile otherTile = hit.collider.gameObject.GetComponent<BoardUITile>();

			if( owner.tiles.Contains( otherTile ) ) //Ignore other tiles that are part of this tetris block
				continue;
			else
			{
				move = false;
				break;
			}
		}

		return move;
	}

	#endregion

	public void Set( int token, bool forceCollisionEnabled = false )
	{
		if( token < 0 )
			token = 0;

		if( this.token == token )
			return;

		if( forceCollisionEnabled )
			collision.enabled = true;
		else
			collision.enabled = token != 0;

		render.SetTiles( GameManager.singleton.tileSprites[token] );
		this.token = token;
	}
}
