using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CircleCollider2D))]
public class Ball : MonoBehaviour 
{
	private static Ball _instance;
	public static Ball singleton{ get{ return _instance; } }

	public float speed
    {
        get { return rbody.velocity.magnitude; }
        set {
            targetSpeed = value;
        }
    }
	public int historyLength = 10;
	[ShowInInspector]public Stack<Vector3> posHistory;


	public BoxCollider2D gameArea;

    public AudioClip wallImpact;
    public AudioClip blockImpact;
    public AudioClip paddleImpact;

    public int hits;

    public float targetSpeed = 2f;
    public float accelToTargetSpeed = 0.01f;

    public float upperSpeed = 25f;
    public float deccelToUpperSpeed = 1f;

    public Color addColor = new Color(1, 0, 0.3f, 1);
    public Color removeColor = new Color(0, 1, 1, 1);

    bool adding = false;

    int numAdded = 0;

    [SerializeField]
	private float startVelocity = 2f;

    private Rigidbody2D rbody;
    [HideInInspector]public CircleCollider2D col;

    private AudioSource player;
	private Vector3 startPosition;

	void Awake()
	{
		if( _instance == null )
			_instance = this;
	}
	void Start()
    {
		startPosition = transform.position;
		col = GetComponent<CircleCollider2D>();
        rbody = GetComponent<Rigidbody2D>();
        player = GetComponent<AudioSource>();

        Physics2D.showColliderAABB = true;


		Reset();

    }

    void SetAdding(bool isAdding)
    {
        adding = isAdding;
        TrailRenderer trail = GetComponentInChildren<TrailRenderer>();
        trail.startColor = adding ? addColor : removeColor;
        trail.endColor = trail.startColor;
        //rbody.velocity = rbody.velocity + Random.insideUnitCircle;
		rbody.AddForce( Random.insideUnitCircle );
    }

    private int wallBounces = 0;
    void OnCollisionEnter2D( Collision2D impact )
    {
        BoardUITile tile = impact.gameObject.GetComponent<BoardUITile>();
		if ( tile != null ) 
		{
            wallBounces = 0;
            if ( tile.owner == null ) 
			{
                var board = GameManager.singleton.board;

                player.PlayOneShot( blockImpact );
                if (adding) 
				{
					if( canPlaceBlock == false )
						return;

					float dx = rbody.position.x - tile.x;
                    float dy = rbody.position.y + 1 - tile.y;

                    int xPos = tile.x;
                    int yPos = tile.y;
                    if (Mathf.Abs(dx) > Mathf.Abs(dy)) 
					{
                        xPos += dx > 0 ? 1 : -1;
                    } 
					else 
					{
                        yPos += dy > 0 ? 1 : -1;
                    }

                   // rbody.position = rbody.position + new Vector2(dx, dy);
					rbody.MovePosition( rbody.position + new Vector2(dx, dy) );
                    //rbody.velocity = rbody.velocity + Random.insideUnitCircle;
					rbody.AddForce( Random.insideUnitCircle );

                    if (board.IsInBounds(xPos, yPos)) 
					{
                        board[xPos, yPos] = Random.Range(1, 7);
						GameManager.singleton.boardRendering.tiles[xPos, yPos].DelayCollisionWithBall();
						StartCoroutine(PreventPlaceBlocks());
                    }

                    if (++numAdded >= 5) 
					{
                        SetAdding(false);
                        numAdded = 0;
                    }
                }
				else 
				{
                    board[tile.x, tile.y] = 0;

                    GameObject glitchedTile = Instantiate<GameObject>( GameManager.singleton.boardRendering.tilePrefab,
                                              tile.transform.position, Quaternion.identity, tile.transform );

                    BoardUITile visual = glitchedTile.GetComponent<BoardUITile>();
                    visual.Initalize( tile.token, true, GameManager.singleton.tileGlitchTime );

                    GameManager.singleton.ballHitsCombo++;
                }

                return;
            } 
			else 
			{
                //We hit a falling tetris piece
                player.PlayOneShot( paddleImpact );

				SetAdding( !adding );
            }
        } 
		else 
		{
            //We hit a wall or something?

            wallBounces++;
            if ( wallBounces > 2 ) 
			{
                GameManager.singleton.FinishCombo();
            }

            player.PlayOneShot( wallImpact );
        }


		GameManager.singleton.vectorMesh.AddGridForce(
			new Vector3(transform.position.x, transform.position.y, 5f), 
			0.1f,
			0.8f, Color.clear, false);


    }

	private int _resetCounter;
	private int resetCountdown = 10;
    void FixedUpdate()
    {
		posHistory.Push( transform.position );
		if( posHistory.Count > historyLength )
			posHistory.Pop();

		float distance = HistoryDistance().magnitude;
		if( distance < 1.5f )
		{
			Reset();
		}

		if( rbody.velocity == Vector2.zero )
		{
			_resetCounter--;
			if( _resetCounter <= 0 )
			{
				Reset();
				return;
			}
		}
		else if( !gameArea.bounds.Contains( transform.position ) )
		{
			Reset();
			return;
		}
		else
			_resetCounter = resetCountdown;
			
        Vector2 additionalForce = Vector2.zero;

        if ( rbody.velocity.x < targetSpeed ) //Prevent getting stuck on a single axis.
        { additionalForce.x = Mathf.Sign(rbody.velocity.x) * accelToTargetSpeed; }
        else if ( rbody.velocity.x > upperSpeed )
        { additionalForce.x = -Mathf.Sign(rbody.velocity.x) * deccelToUpperSpeed; }

        if ( rbody.velocity.y < targetSpeed )
        { additionalForce.y = Mathf.Sign(rbody.velocity.y) * accelToTargetSpeed; }
        else if ( rbody.velocity.x > upperSpeed )
        { additionalForce.y = -Mathf.Sign(rbody.velocity.y) * deccelToUpperSpeed; }

        if ( additionalForce != Vector2.zero )
        { rbody.AddForce( additionalForce ); }

    }

	public void Reset()
	{
		posHistory = new Stack<Vector3>();
		StopCoroutine(PreventPlaceBlocks());
		canPlaceBlock = true;
		_resetCounter = resetCountdown;
		wallBounces = 0;
		rbody.MovePosition( startPosition );

		rbody.AddForce(Random.insideUnitCircle.normalized * startVelocity, ForceMode2D.Impulse);
	}

	public void OnDrawGizmos()
	{
		if( !Application.isPlaying || posHistory.Count < 3 )
			return;

		List<Vector3> d = new List<Vector3>( posHistory );

		Gizmos.color = Color.yellow;
		for (int i = 0; i < posHistory.Count; i++) 
		{
			DebugExtension.DrawPoint( d[i] );
		}
	}

	public float blockPlaceDelay = 0.3f;
	private bool canPlaceBlock = true;
	IEnumerator PreventPlaceBlocks()
	{
		canPlaceBlock = false;
		yield return new WaitForSeconds( blockPlaceDelay );
		canPlaceBlock = true;
	}

	Vector3 HistoryDistance()
	{
		//float result = 0f;
		if( posHistory.Count < historyLength )
			return Vector3.one * Mathf.Infinity;

		List<Vector3> d = new List<Vector3>( posHistory );

		Vector3 relDist = d[0];
		for (int i = 1; i < posHistory.Count; i++) 
		{
			relDist += d[i];
			//result += Vector3.Distance( posHistory[i-1], posHistory[i] );
		}
		//return result;
		return relDist;
	}
}
