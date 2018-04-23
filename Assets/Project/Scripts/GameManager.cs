using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
	private static GameManager _instance;
	public static GameManager singleton{ get{ return _instance; } }

	public float tileGlitchTime = 0.5f;

	public Queue<Block> upcomingBlocks;

	public float gameUpdateSpeed = 0.8f;
	public float holdDownMultiplier = 5f;

	public List<TileAnimationSet> tileSprites;

	public int boardSizeX;
	public int boardSizeY;

	public Board board;
	public BoardUI boardRendering;

	public TetrisBlock fallingBlock;

	public VectorGrid vectorMesh;

	private float _timeMultiplier = 1f;
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
		upcomingBlocks = new Queue<Block>();

		board = new Board( boardSizeX, boardSizeY, 0 );

		boardRendering.Initalize( board );
		board.DirtyAll();

		upcomingBlocks = new Queue<Block>();
		upcomingBlocks.Enqueue(
			Block.blocks[Random.Range(0, Block.blocks.Count)]
		);
		upcomingBlocks.Enqueue(
			Block.blocks[Random.Range(0, Block.blocks.Count)]
		);

		fallingBlock.Initalize( Block.blocks[1] );

		StartCoroutine( GameUpdater() );
	}

	void Update()
	{
		_timeMultiplier = 
			(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) ?
			holdDownMultiplier : 1f;
	}
		
	void GameUpdate()
	{
		if( fallingBlock.active == false )
		{
			fallingBlock.Initalize( upcomingBlocks.Dequeue() );

			upcomingBlocks.Enqueue(
				Block.blocks[Random.Range(0, Block.blocks.Count)]
			);
		}
		else
			fallingBlock.OnGameUpdate();



		/*
		for (int x = 0; x < board.width; x++) 
		{
			for (int y = 0; y < board.height; y++)
			{
				//board[x,y] = VectorExtras.SplitChance() ? 0 : 1;
			}
		} */
	}
	public void ForceGameUpdate()
	{
		_timeTillUpdate = gameUpdateSpeed;
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
			_timeTillUpdate -= (Time.deltaTime * _timeMultiplier);

			if( _timeTillUpdate <= 0f )
			{
				GameUpdate();
				_timeTillUpdate = gameUpdateSpeed;
			}

			yield return null;
		}

	}
}
