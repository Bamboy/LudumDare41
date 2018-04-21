using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TetrisBlock : MonoBehaviour
{
	public float holdDownTimer = 0.2f;

	private Block _template;
	public Block template{ get{ return _template; } private set{ _template = value; } }

	public int rotation;

	public bool active{ get; private set; }

	/// Position of our pivot on the game board.
	public Vector2Int position
	{
		get{ return new Vector2Int(Mathf.FloorToInt( transform.position.x ), Mathf.FloorToInt( transform.position.y )); }
		set{
			transform.position = new Vector3( value.x, value.y, 0f );
		}
	}


	public List<BoardUITile> tiles;

	void Start()
	{
		StartCoroutine( MoveDownLoop() );
	}

	public void Initalize( Block template, int rotation = 0 )
	{
		active = true;
		this.template = template;
		position = GameManager.singleton.board.blockSpawn;
		this.rotation = rotation;
		tiles = new List<BoardUITile>();

		GameObject prefab = GameManager.singleton.boardRendering.tilePrefab;
		Vector2 offset = template.rotations[rotation].pivot;
		for (int x = 0; x < template.rotations[rotation].tiles.GetLength(0); x++) 
		{
			for (int y = 0; y < template.rotations[rotation].tiles.GetLength(1); y++) 
			{
				if( template.rotations[rotation].tiles[x,y] == false ) //Skip blank blocks
					continue;

				GameObject obj = Instantiate<GameObject>( prefab );


				obj.transform.parent = this.transform;
				obj.transform.localPosition = new Vector3( x - template.rotations[rotation].pivot.x, 
					y - template.rotations[rotation].pivot.y - 1, 0f );
				BoardUITile t = obj.GetComponent<BoardUITile>();
				t.Initalize(x, y - 1, this);
				//t.token = template.token;
				tiles.Add( t );
			}
		}

	}



	public void OnGameUpdate()
	{
		if( active )
		{
			MoveDown();
		}
	}

	public void DetatchChildren()
	{
		foreach (BoardUITile tile in tiles) 
		{
			//TODO place children on board.

			Vector2Int tilePos = tile.position + Vector2Int.up;
			if( GameManager.singleton.board.IsInBounds( tilePos ) )
			{
				GameManager.singleton.board[tilePos.x, tilePos.y] = tile.token;
				Debug.Log("Placed tile " + tilePos);

			}

			Destroy( tile.gameObject );
		}
		tiles = new List<BoardUITile>();
		active = false;
	}


	/// Tries to rotate this block. Returns true if the rotation is possible.
	public bool Rotate( bool clockwise )
	{
		rotation = clockwise ? rotation + 1 : rotation - 1; //TODO
		return false;
	}

	#region input
	public void Update()
	{
		if( Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) )
		{
			MoveLeft();
		}
		else if( Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) )
		{
			MoveRight();
		}
		else
		{


		}
	}
	void MoveLeft()
	{

		GameManager.singleton.ForceGameUpdate();
	}
	void MoveRight()
	{

		GameManager.singleton.ForceGameUpdate();
	}
	void MoveDown()
	{
		bool moveDown = true;
		foreach (BoardUITile tile in tiles) 
		{
			if( tile.CanMoveDown() == false )
			{
				moveDown = false;
				break;
			}
		}

		if( moveDown )
		{
			position += new Vector2Int(0, -1);
		}
		else
		{
			DetatchChildren();
		}
	}

	private float _holdDownTimer;
	IEnumerator MoveDownLoop()
	{
		_holdDownTimer = holdDownTimer;
		while( true )
		{
			if( active == false )
			{
				yield return null;
				continue;
			}

			if( Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.PageDown) )
			{
				_holdDownTimer -= Time.deltaTime;

				if( _holdDownTimer <= 0f )
				{
					GameManager.singleton.ForceGameUpdate(); //This will also move our block down!

					_holdDownTimer = holdDownTimer;
				}
			}
			else
				_holdDownTimer = holdDownTimer;
			
			yield return null;
		}
	}

	#endregion

}
