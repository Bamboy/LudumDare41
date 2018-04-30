using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
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

    private bool _gameOver = false;
    public bool isGameOver
    {
        get { return _gameOver; }
        set {
            _gameOver = value;
            GameOverUI.singleton.SetMenuState( MenuState.GameOver );

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
        AddBlocks();

        fallingBlock.Initalize( upcomingBlocks.Dequeue() );

        StartCoroutine( GameUpdater() );
    }

    void Update()
    {

        if ( isGameOver == false ) {
            if ( Application.isFocused == false && GameOverUI.singleton.shownMenu == MenuState.None ) {
                GameOverUI.singleton.SetMenuState( MenuState.Pause ); //Pause the game!
            } else if ( Input.GetKeyDown(KeyCode.Escape) ) {
                GameOverUI.singleton.SetMenuState( GameOverUI.singleton.shownMenu == MenuState.None ? MenuState.Pause :
                                                   MenuState.None );
            }
        }



        if ( Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) )
        { _timeMultiplier = holdDownMultiplier; }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        { _timeMultiplier = holdUpMultiplier; }
        else
        { _timeMultiplier = 1f; }


    }

    void AddBlocks()
    {
        // implements the 7-bag piece queueing mechanism
        List<Block> bag = new List<Block>();
        for (int i = 0; i < Block.blocks.Count; i++) {
            bag.Add(Block.blocks[i]);
        }
        while (bag.Count > 0) {
            int n = Random.Range(0, bag.Count);
            upcomingBlocks.Enqueue(bag[n]);
            bag.RemoveAt(n);
        }
    }

    void GameUpdate()
    {
        if ( isGameOver )
        { return; }

        if ( fallingBlock.active == false ) {
            fallingBlock.Initalize( upcomingBlocks.Dequeue() );

            if (upcomingBlocks.Count < Block.blocks.Count) {
                AddBlocks();
            }

            UpcomingBlocks.singleton.UpdateUI();
        } else {
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

    public void ResetTimer()
    {
        _timeTillUpdate = gameUpdateSpeed;
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
