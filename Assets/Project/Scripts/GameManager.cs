using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    private static GameManager _instance;
    public static GameManager singleton { get { return _instance; } }

    public float tileGlitchTime = 0.5f;

    public Queue<Block> upcomingBlocks;

    public float gameUpdateSpeed = 0.8f;
    public float holdDownMultiplier = 5f;
    public float holdUpMultiplier = 0.5f;

    public List<TileAnimationSet> tileSprites;

    public int boardSizeX;
    public int boardSizeY;

    public Board board;
    public BoardUI boardRendering;

    public TetrisBlock fallingBlock;

    public VectorGrid vectorMesh;

    public AudioSource lowMusic, highMusic;

	private bool _gameOver = false;
	public bool isGameOver
	{
		get{ return _gameOver; }
		set{ 
			_gameOver = value;

			GameOverUI.singleton.UpdateUI();

			Time.timeScale = value ? 0f : 1f;
		}
	}

    #region Score
    private int _score;
    public int score
    {
        get { return _score; }
        set {
            _score = value;
            Scoreboard.singleton.UpdateUI();
        }
    }
    private float _scoreMult = 1f;
    /// <summary>
    /// Gets or sets the score multiplier.
    /// </summary>
    /// <value>The score multiplier.</value>
    public float scoreMultiplier
    {
        get { return _scoreMult; }
        set {
            _scoreMult = value;
            Scoreboard.singleton.UpdateUI();
        }
    }

    private float _scoreCombo = 0f;
    /// <summary>
    /// Pending score that has not been multiplied by our multiplier
    /// </summary>
    public float scoreCombo
    {
        get { return _scoreCombo; }
        set {
            _scoreCombo = value;
            Scoreboard.singleton.UpdateUI();
        }
    }

    public void ClearedRows( int rows )
    {
        int s = 100 * (rows ^ 2);
        scoreCombo += (float)s;

        scoreMultiplier += (float)rows * 2f;

        rowsClearedCombo += rows;
    }

    public void BallHitBlock()
    {
        ballHitsCombo++;
        scoreCombo += 3f;
        scoreMultiplier += 0.2f;
    }

    private int _ballhitcombo;
    public int ballHitsCombo
    {
        get { return _ballhitcombo; }
        set { _ballhitcombo = value; Scoreboard.singleton.UpdateUI(); }
    }
    private int _rowsClearedCombo;
    public int rowsClearedCombo
    {
        get { return _rowsClearedCombo; }
        set { _rowsClearedCombo = value; Scoreboard.singleton.UpdateUI(); }
    }

    public void FinishCombo()
    {
        scoreCombo += ((float)ballHitsCombo * 1.2f) + ((float)rowsClearedCombo * 5f);
        GameManager.singleton.score += Mathf.FloorToInt(scoreCombo * scoreMultiplier);

        ballHitsCombo = 0;
        rowsClearedCombo = 0;

        scoreCombo = 0f;
        scoreMultiplier = 1f;
    }
    #endregion

    private float _timeMultiplier = 1f;
    void Awake()
    {
        if ( _instance == null ) {
            _instance = this;
        } else {
            Destroy( this.gameObject );
            return;
        }


    }

    void Start()
    {
        Block.BuildBlocks();
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
		upcomingBlocks.Enqueue(
			Block.blocks[Random.Range(0, Block.blocks.Count)]
		);

		fallingBlock.Initalize( Block.blocks[Random.Range(0, Block.blocks.Count)] );

        StartCoroutine( GameUpdater() );
    }

    void Update()
    {

        if ( Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) )
        { _timeMultiplier = holdDownMultiplier; }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        { _timeMultiplier = holdUpMultiplier; }
        else
        { _timeMultiplier = 1f; }

        // Alter the music mix based on the board's highest point
        float mix = board.highestPoint * 1f / board.height;
        lowMusic.volume = Mathf.Lerp(lowMusic.volume, Mathf.Sqrt(mix), Time.deltaTime);
        highMusic.volume = Mathf.Lerp(highMusic.volume, Mathf.Sqrt(1f - mix), Time.deltaTime);
    }

    void GameUpdate()
    {
		if( isGameOver )
			return;
		
		if ( fallingBlock.active == false ) 
		{
            fallingBlock.Initalize( upcomingBlocks.Dequeue() );

            upcomingBlocks.Enqueue(
                Block.blocks[Random.Range(0, Block.blocks.Count)]
            );

			UpcomingBlocks.singleton.UpdateUI();
        } 
		else
        { 
			fallingBlock.OnGameUpdate(); 
		}
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
        while ( true ) {
            _timeTillUpdate -= (Time.deltaTime * _timeMultiplier);

            if ( _timeTillUpdate <= 0f ) {
                GameUpdate();
                _timeTillUpdate = gameUpdateSpeed;
            }

            yield return null;
        }

    }
}
