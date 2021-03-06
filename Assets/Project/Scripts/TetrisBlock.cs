﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TetrisBlock : MonoBehaviour {
    private Block _template;
    public Block template { get { return _template; } private set { _template = value; } }

    private int _rotation;
    public int rotation
    {
        get { return _rotation; }
        set {
            _rotation = (int)Mathf.Repeat((float)value, (float)template.rotations.Count);
        }
    }

    public float blockPlaceVectorgridForce = 1f;
    public float blockPlaceVectorgridRadius = 0.5f;

    public float rowClearVectorgridForce = 3f;
    public float rowClearVectorgridRadius = 3f;
    public Color rowClearVectorgridColor = Color.white;

    public bool active { get; private set; }

    /// Position of our pivot on the game board.
    public Vector2Int position
    {
        get { return new Vector2Int(Mathf.FloorToInt( transform.position.x ), Mathf.FloorToInt( transform.position.y )); }
        set {
            transform.position = new Vector3( value.x, value.y, 0f );
        }
    }


    public List<BoardUITile> tiles;

    private AudioSource player;
    public AudioClip blockMoveDeny;
    public AudioClip blockRotate;
    public AudioClip blockMove;
    public AudioClip blockPlace;


    public AudioClip rowCleared;
    void Start()
    {
        player = GetComponent<AudioSource>();
    }

    public void Initalize( Block template, int rotation = 0 )
    {
        active = true;
        this.template = template;
        position = GameManager.singleton.board.blockSpawn;
        this.rotation = rotation;
        transform.rotation = Quaternion.identity;
        tiles = new List<BoardUITile>();

        GameObject prefab = GameManager.singleton.boardRendering.tilePrefab;
        Vector2 offset = template.rotations[rotation].pivot;
        for (int x = 0; x < template.rotations[rotation].tiles.GetLength(0); x++) {
            for (int y = 0; y < template.rotations[rotation].tiles.GetLength(1); y++) {
                if ( template.rotations[rotation].tiles[x, y] == false ) //Skip blank blocks
                { continue; }

                GameObject obj = Instantiate<GameObject>( prefab );

                obj.transform.parent = this.transform;
                obj.transform.localPosition = new Vector3(
                    x - template.rotations[rotation].pivot.x,
                    y - template.rotations[rotation].pivot.y, 0f );
                BoardUITile t = obj.GetComponent<BoardUITile>();
                t.Initalize(x, y - 1, this);



                //t.token = template.token;
                tiles.Add( t );
            }
        }
    }

    public void OnGameUpdate()
    {
        if ( active == false )
        { return; }

        MoveDown();

    }

    public void DetatchChildren()
    {
        if ( active == false )
        { return; }

        if ( position.y == GameManager.singleton.board.blockSpawn.y ) {
            GameManager.singleton.isGameOver = true;
            active = false;
            return;
        }

        Board board = GameManager.singleton.board;

        Color forceColor = tiles[0].GetComponent<TileAnimator>().tileSet.blockColor;
        foreach (BoardUITile tile in tiles) {
            Vector2Int tilePos = tile.position + Vector2Int.up;
            if ( board.IsInBounds( tilePos ) ) {
                GameManager.singleton.vectorMesh.AddGridForce(new Vector3(tile.transform.position.x, tile.transform.position.y, 5f),
                        blockPlaceVectorgridForce,
                        blockPlaceVectorgridRadius, forceColor, true);

                board[tilePos.x, tilePos.y] = tile.token;
            }
        }

        //Check for row clears!
        List<int> clearedRows = new List<int>();
        for (int y = 0; y < board.tiles.GetLength(1); y++) { //Go by height first, then by width
            bool doClearRow = true;
            for (int x = 0; x < board.tiles.GetLength(0); x++) {
                if ( board[x, y] == 0 ) { //Is this space empty?
                    doClearRow = false;
                    break;
                }
            }

            if ( doClearRow )
            { clearedRows.Add( y ); }
        }

        if ( clearedRows.Count == 0 ) {
            player.PlayOneShot( blockPlace );
        } else {
            GameManager.singleton.ClearedRows( clearedRows.Count );
            foreach (int row in clearedRows) {
                for (int x = 0; x < board.tiles.GetLength(0); x++) {
                    GameManager.singleton.vectorMesh.AddGridForce(new Vector3(x, row, 5f), rowClearVectorgridForce * clearedRows.Count,
                            rowClearVectorgridRadius * clearedRows.Count, rowClearVectorgridColor, true);

                    board[x, row] = 0;
                }
            }

            board.DropEmptyRows();

            player.PlayOneShot( rowCleared );
        }

        foreach (BoardUITile tile in tiles) {
            Destroy( tile.gameObject );
        }

        GameManager.singleton.board.ResolveDirty();

        tiles = new List<BoardUITile>();
        active = false;
    }


    /// Tries to rotate this block. Returns true if the rotation is possible.
    public bool Rotate( bool clockwise )
    {
        if ( active == false )
        { return false; }

        int newRot = (int)Mathf.Repeat(rotation + (clockwise ? 1f : -1f), (float)template.rotations.Count);

        bool canRotate = true;
        //Check for colliding spaces before rotating
        for (int x = 0; x < template.rotations[newRot].tiles.GetLength(0); x++) {
            for (int y = 0; y < template.rotations[newRot].tiles.GetLength(1); y++) {
                if ( template.rotations[newRot].tiles[x, y] == false )
                { continue; }

                //Move each tile to its new rotated position.
                //This assumes that each rotation keeps the same number of tiles.

                int px = x - template.rotations[newRot].pivot.x + position.x;
                int py = y - template.rotations[newRot].pivot.y + position.y;

                Collider2D[] intersections = Physics2D.OverlapCircleAll( new Vector2(px, py), 0.4f );
                DebugExtension.DebugWireSphere(new Vector2(px, py), 0.4f);

                foreach (Collider2D col in intersections) { //TODO - check if we are out of bounds!
                    BoardUITile colTile = col.gameObject.GetComponent<BoardUITile>();
                    if ( colTile == null )
                    { return false; } //We hit some other collider that isn't a tile. Don't rotate.

                    if ( tiles.Contains( colTile ) )
                    { continue; }
                    else {
                        return false;
                    }
                }
            }
        }

        //Rotate our existing tiles
        int i = 0;
        for (int x = 0; x < template.rotations[newRot].tiles.GetLength(0); x++) {
            for (int y = 0; y < template.rotations[newRot].tiles.GetLength(1); y++) {
                if ( template.rotations[newRot].tiles[x, y] == false )
                { continue; }

                //Move each tile to its new rotated position.
                //This assumes that each rotation keeps the same number of tiles.
                tiles[i].transform.localPosition = new Vector3(
                    x - template.rotations[newRot].pivot.x,
                    y - template.rotations[newRot].pivot.y, 0f );

                i++;
            }
        }

        rotation = newRot;
        player.PlayOneShot( blockRotate );

        return true;
    }

    #region input
    public void Update()
    {
        if ( active == false )
        { return; }

        if ( Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ) {
            MoveLeft();
        } else if ( Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) ) {
            MoveRight();
        }

        if ( Input.GetKeyDown(KeyCode.Q) ) {
            bool didRot = Rotate( false );
            if ( didRot == false ) {
                Debug.Log("Blocked!");
                player.PlayOneShot( blockMoveDeny );
            }
        } else if ( Input.GetKeyDown(KeyCode.E) ) {
            bool didRot = Rotate( true );
            if ( didRot == false ) {
                Debug.Log("Blocked!");
                player.PlayOneShot( blockMoveDeny );
            }
        }
    }

    void MoveLeft()
    {
        if ( active == false || Time.timeScale <= 0.01f )
        { return; }

        bool moveLeft = true;
        foreach (BoardUITile tile in tiles) {
            if ( tile.CanMoveLeft() == false ) {
                moveLeft = false;
                break;
            }
        }
        if ( moveLeft ) {
            position += new Vector2Int(-1, 0);
            player.PlayOneShot( blockMove );
        }
    }
    void MoveRight()
    {
        if ( active == false || Time.timeScale <= 0.01f )
        { return; }

        bool moveRight = true;
        foreach (BoardUITile tile in tiles) {
            if ( tile.CanMoveRight() == false ) {
                moveRight = false;
                break;
            }
        }
        if ( moveRight ) {
            position += new Vector2Int(1, 0);
            player.PlayOneShot( blockMove );
        }
    }
    void MoveDown()
    {
        if ( active == false )
        { return; }

        bool moveDown = true;
        foreach (BoardUITile tile in tiles) {
            if ( tile.CanMoveDown() == false ) {
                moveDown = false;
                break;
            }
        }

        if ( moveDown ) {
            GameManager.singleton.ResetTimer();
            position += new Vector2Int(0, -1);
        } else {
            DetatchChildren();
        }
    }

    #endregion

}
