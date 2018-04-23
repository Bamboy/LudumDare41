using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public delegate void PositionDelegate(int x, int y);

[Serializable]
public class Board
{
	public static readonly Vector2Int OUTOFBOUNDS = new Vector2Int(int.MinValue, int.MinValue);
	public static readonly Vector2Int ABOVE = new Vector2Int( 0,-1);
	public static readonly Vector2Int BELOW = new Vector2Int( 0, 1);
	public static readonly Vector2Int LEFT  = new Vector2Int(-1, 0);
	public static readonly Vector2Int RIGHT = new Vector2Int( 1, 0);

	/// The tiles on our board. The int represents which player owns that space. Zero is nobody
	[ShowInInspector]
	public int[,] tiles;

	/// Tiles that need to be updated in the UI.
	public bool[,] dirtyTiles; 

	/// Will be true if any tiles are dirty.
	public bool isDirty = false;

	/// Gets the width.
	public int width{ get{ return tiles.GetLength(0); } }
	/// Gets the height.
	public int height{ get{ return tiles.GetLength(1); } }

	public Vector2Int blockSpawn;

	private int _highestPoint;
	public int highestPoint{ get{ return _highestPoint; } }

	/// Create a new board with the given size.
	public Board( int width, int height, int fillTokens = 0 )
	{
		tiles = new int[width, height];

		blockSpawn = new Vector2Int( width / 2, height - 1 );

		for (int x = 0; x < width; x++) 
		{
			for (int y = 0; y < height; y++) 
			{
				tiles[x, y] = fillTokens;
			}
		}

		DirtyAll();
		ResolveDirty();
	}

	/// Creates a copy of the given board
	public static Board Copy( Board other )
	{
		Board result = new Board( other.width, other.height );
		for (int x = 0; x < result.width; x++) 
		{
			for (int y = 0; y < result.height; y++) 
			{
				result[x, y] = other[x, y];
			}
		}
			
		return result;
	}
		
	public int this[int x, int y] 
	{
		get{ return tiles[x, y]; }
		set
		{
			if( tiles[x, y] != value )
			{
				tiles[x, y] = value;
				isDirty = true;
				dirtyTiles[x, y] = true;
			}
		}
	}


	/// Marks all blocks to update
	public void DirtyAll()
	{
		dirtyTiles = new bool[width, height];
		for (int x = 0; x < width; x++) 
		{
			for (int y = 0; y < height; y++) 
			{
				dirtyTiles[x, y] = true;
			}
		}
		isDirty = true;
	}
	public void ClearDirty()
	{
		dirtyTiles = new bool[width, height];
		for (int x = 0; x < width; x++) 
		{
			for (int y = 0; y < height; y++) 
			{
				dirtyTiles[x, y] = false;
			}
		}
		isDirty = false;
	}
	public void ResolveDirty()
	{
		if( isDirty )
		{
			
			if( onResolveDirty != null )
			{
				for (int x = 0; x < width; x++) 
				{
					for (int y = 0; y < height; y++)
					{
						if( dirtyTiles[x, y] ) //Is this tile marked to update?
						{
							onResolveDirty(x, y);
						}
						//tiles[x, y].Set( tracking[x, y] );
					}
				}
			}
				
			if( onAfterChanged != null )
				onAfterChanged();

			ClearDirty();
		}
	}
	public int GetHighestPoint()
	{
		int highest = 0; 
		for (int y = 0; y < height; y++) 
		{
			for (int x = 0; x < width; x++) 
			{
				if( tiles[x,y] != 0 )
				{
					highest = y;
					break;
				}
			}
		}
		_highestPoint = highest;
		return highest;
	}

	public void DropEmptyRows()
	{
		int[,] newTiles = new int[width, height];


		bool[] emptyRows = new bool[height];
		for (int y = 0; y < height; y++) 
		{
			emptyRows[y] = IsRowEmpty(y);
		}
			
		int validRowIndex = 0;
		for (int y = 0; y < height; y++) 
		{
			if( emptyRows[y] == false )
			{
				for (int x = 0; x < width; x++) 
				{

					newTiles[x, validRowIndex] = tiles[x, y];

				}

				validRowIndex++;
			}

		}

		tiles = newTiles;
		DirtyAll();

		/*

		int highest = GetHighestPoint();

		int breakout = 300;

		for (int y = 0; y < highest; y++) 
		{
			breakout--;

			if( breakout <= 0 )
			{
				Debug.LogError("Infinite loop in dropemptyrows");
				break;
			}

			if( IsRowEmpty( y ) )
			{
				ShiftDownward( y );
				y -= 1;
			}
		}*/
	}

	bool IsRowEmpty( int y )
	{
		for (int x = 0; x < width; x++) 
		{
			if( tiles[x,y] != 0 )
				return false;
		}
		return true;
	}

	void ShiftDownward( int aboveRow )
	{
		Board copy = Copy( this );
		for (int y = aboveRow; y < height - 1; y++) 
		{
			for (int x = 0; x < width; x++) 
			{
				copy[x,y] = tiles[x, y + 1];
			}
		}
		this.tiles = copy.tiles;
		DirtyAll();
	}

	/// Action to take when we go through dirty tiles.
	public PositionDelegate onResolveDirty;
	/// Optional action to take right after the board changes.
	public Action onAfterChanged;

	#region Position helpers

	public static Vector2Int GetInDirection( Board b, Vector2Int pos, Direction dir ){ return GetInDirection(b, pos.x, pos.y, dir); }
	public static Vector2Int GetInDirection( Board b, int x, int y, Direction dir )
	{
		switch( dir )
		{
		case Direction.Above:
			return b.GetAbove( x, y );

		case Direction.Below:
			return b.GetBelow( x, y );

		case Direction.Left:
			return b.GetLeft( x, y );

		case Direction.Right:
			return b.GetRight( x, y );

		case Direction.AboveLeft:
			return b.GetAboveLeft( x, y );

		case Direction.AboveRight:
			return b.GetAboveRight( x, y );

		case Direction.BelowLeft:
			return b.GetBelowLeft( x, y );

		case Direction.BelowRight:
			return b.GetBelowLeft( x, y );

		default:
			return OUTOFBOUNDS;
		}
	}

	/// Returns the tile that is above the given position. 
	/// Returns -1, -1 if out of bounds.
	public Vector2Int GetAbove( int x, int y )
	{
		return OOBFilter( new Vector2Int(x, y) + ABOVE );
	}
	/// Returns the tile that is below the given position. 
	/// Returns -1, -1 if out of bounds.
	public Vector2Int GetBelow( int x, int y )
	{
		return OOBFilter( new Vector2Int(x, y) + BELOW );
	}
	/// Returns the tile that is to the left of the given position. 
	/// Returns -1, -1 if out of bounds.
	public Vector2Int GetLeft( int x, int y )
	{
		return OOBFilter( new Vector2Int(x, y) + LEFT );
	}
	/// Returns the tile that is to the right of the given position. 
	/// Returns -1, -1 if out of bounds.
	public Vector2Int GetRight( int x, int y )
	{
		return OOBFilter( new Vector2Int(x, y) + RIGHT );
	}

	/// Returns the tile that is diagonal up-left of the given position. 
	/// Returns -1, -1 if out of bounds.
	public Vector2Int GetAboveLeft( int x, int y )
	{
		return OOBFilter( new Vector2Int(x, y) + ABOVE + LEFT );
	}
	/// Returns the tile that is diagonal up-right of the given position. 
	/// Returns -1, -1 if out of bounds.
	public Vector2Int GetAboveRight( int x, int y )
	{
		return OOBFilter( new Vector2Int(x, y) + ABOVE + RIGHT );
	}
	/// Returns the tile that is diagonal down-left of the given position. 
	/// Returns -1, -1 if out of bounds.
	public Vector2Int GetBelowLeft( int x, int y )
	{
		return OOBFilter( new Vector2Int(x, y) + BELOW + LEFT );
	}
	/// Returns the tile that is diagonal down-left of the given position. 
	/// Returns -1, -1 if out of bounds.
	public Vector2Int GetBelowRight( int x, int y )
	{
		return OOBFilter( new Vector2Int(x, y) + BELOW + RIGHT );
	}

	/// Converts a position to an error state if it is out of bounds.
	private Vector2Int OOBFilter( Vector2Int pos ){ return IsInBounds( pos ) ? pos : OUTOFBOUNDS; }

	/// Returns true if the position is within our board size.
	public bool IsInBounds( Vector2Int pos ){ return IsInBounds( pos.x, pos.y ); }
	/// Returns true if the position is within our board size.
	public bool IsInBounds( int x, int y )
	{
		if( x < 0 || x >= width || y < 0 || y >= height )
			return false;
		else
			return true;
	}
	#endregion
}

public enum Direction
{
	None = -1,

	Above,
	Below,
	Left,
	Right,

	AboveLeft,
	AboveRight,
	BelowLeft,
	BelowRight
}
