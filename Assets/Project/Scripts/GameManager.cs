using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
	private static GameManager _instance;
	public static GameManager singleton{ get{ return _instance; } }

	public float gameUpdateSpeed = 0.8f;

	public List<Sprite> tileSprites;

	public int boardSizeX;
	public int boardSizeY;

	public Board board;
	public BoardUI boardRendering;

	public TetrisBlock fallingBlock;

	void Awake()
	{
		if( _instance == null )
		{
			_instance = this;
		}
		else
		{
			Destroy( this.gameObject );
			return;
		}
	}

	void Start()
	{
		Block.BuildBlocks();

		board = new Board( boardSizeX, boardSizeY );

		boardRendering.Initalize( board );


		fallingBlock.Initalize( Block.blocks[0] );

		StartCoroutine( GameUpdater() );
	}



	void GameUpdate()
	{
		fallingBlock.OnGameUpdate();

		for (int x = 0; x < board.width; x++) 
		{
			for (int y = 0; y < board.height; y++)
			{
				//board[x,y] = VectorExtras.SplitChance() ? 0 : 1;
			}
		}
	}

	void LateUpdate()
	{
		board.ResolveDirty();
	}

	private float _timeTillUpdate;
	IEnumerator GameUpdater()
	{
		_timeTillUpdate = gameUpdateSpeed;
		while( true )
		{
			_timeTillUpdate -= Time.deltaTime;

			if( _timeTillUpdate <= 0f )
			{
				GameUpdate();
				_timeTillUpdate = gameUpdateSpeed;
			}

			yield return null;
		}

	}
}
