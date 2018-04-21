using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinEvaluator 
{
	private int winLength;
	private Board board;

	public WinEvaluator( Board board, int winLength )
	{
		this.winLength = winLength;
		this.board = board;
	}

	/// Check for empty spaces.
	public bool SpacesAvailable()
	{
		bool emptyExists = false;
		for (int x = 0; x < board.width; x++) 
		{
			for (int y = 0; y < board.height; y++) 
			{
				if( board[x, y] == 0 )
				{
					emptyExists = true;
					break;
				}
			}
			if( emptyExists )
				break;
		}
		return emptyExists;
	}

	/// Evaluate the board and returns the game victory state.
	public Result Evaluate()
	{
		//Go through each space that is occupied
		for (int x = 0; x < board.width; x++) 
		{
			for (int y = 0; y < board.height; y++) 
			{
				if( board[x, y] == 0 )
					continue;

				List<Vector2Int> chain = FindChain(x, y);
				if( chain != null && chain.Count >= winLength )
				{
					Result win = new Result();
					win.state = GameState.Win;
					win.winningPlayer = board[x,y];
					win.chain = chain;
					return win;
				}
			}
		}

		//Noone has won yet. Check if any more moves can be made.
		if( SpacesAvailable() )
			return new Result();
		else
			return new Result(){ state = GameState.Draw };
	}
		
	List<Vector2Int> FindChain( int startX, int startY )
	{
		int token = board[startX, startY];
		List<Vector2Int> chain = new List<Vector2Int>();

		//Iterate over every direction.
		//Doing this for every tile on the board means we will eventually check the same tile multiple times. (bad)
		for(int i = 0; i < 8; i++) 
		{
			chain = FindChainLength(startX, startY, (Direction)i, token);
			if( chain.Count >= winLength )
			{
				return chain;
			}
		}
		return null;
	}
		
	/// Returns a list of equal-type tiles in the given direction.
	List<Vector2Int> FindChainLength( int x, int y, Direction dir, int token )
	{
		List<Vector2Int> chain = new List<Vector2Int>(){ new Vector2Int(x, y) };

		// Endlessly find similar tiles in the same direction until 
		// we go out of bounds or we reach a tile that is different.
		while( true )
		{
			Vector2Int nextPos;
			if(HasIdenticalInDirection( chain[chain.Count - 1], dir, token, out nextPos ))
				chain.Add( nextPos );
			else
				break;
		}
		return chain;
	}


	/// Checks if the same token exists in a given direction
	bool HasIdenticalInDirection( Vector2Int pos, Direction dir, int token, out Vector2Int newPos )
	{
		newPos = Board.GetInDirection( board, pos, dir );
		if( newPos == Board.OUTOFBOUNDS )
			return false;
		else
			return board[newPos.x, newPos.y] == token;
	}

	public struct Result
	{
		public GameState state;
		public int winningPlayer;
		public List<Vector2Int> chain;
	}
}
	
public enum GameState
{
	None,
	Draw,
	Win
}