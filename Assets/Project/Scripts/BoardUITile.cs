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

	private int x;
	private int y;
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

	public void Initalize(int x, int y)
	{
		this.x = x;
		this.y = y;

		text.text = string.Format("[{0}, {1}]", x, y);
	}
	public void Initalize(int x, int y, TetrisBlock owner)
	{
		if( text != null )
			Destroy( text.gameObject );

		this.token = owner.template.token;
		this.owner = owner;

		render.SetTiles( GameManager.singleton.tileSprites[this.token] );
	}

	void OnCollisionEnter2D( Collision2D col )
	{
		//TODO 


	}


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

	public void Set( int token )
	{
		if( token < 0 )
			token = 0;

		if( this.token == token )
		{
			Debug.Log("No change");
			return;
		}

		collision.enabled = token != 0;

		render.SetTiles( GameManager.singleton.tileSprites[token] );
		this.token = token;
	}
}
