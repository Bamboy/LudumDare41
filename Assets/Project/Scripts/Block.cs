using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// A block of tiles that are glued together.
/// </summary>
[System.Serializable]
public struct Block
{
	public static bool T = true;
	public static bool F = false;
	public static List<Block> blocks;


	public List<BlockOrientation> rotations;


	/// What sprite our tiles represent.
	public int token;





	public static void BuildBlocks()
	{
		blocks = new List<Block>();

		BlockOrientation up;
		BlockOrientation left;
		BlockOrientation down;
		BlockOrientation right;

		/*
			x x
			x x
		*/
		Block square = new Block();
		square.token = 1;
		up = new BlockOrientation( 2, 2, new Vector2Int(0,0), true );
		square.rotations = new List<BlockOrientation>(){ up, up, up, up };//Only one rotation for the square
		blocks.Add( square );

		/*
			x 
			x
			x
			x
		*/

		Block b1x4 = new Block();
		b1x4.token = 2;
		up = new BlockOrientation( 1, 4, new Vector2Int(0,0), true );
		left = new BlockOrientation( 4, 1, new Vector2Int(0,0), true );
		down = new BlockOrientation( 1, 4, new Vector2Int(0,3), true );
		right = new BlockOrientation( 4, 1, new Vector2Int(3,0), true );
		b1x4.rotations = new List<BlockOrientation>(){ up, left, down, right };
		blocks.Add( b1x4 );

		/*
			x
			x
			x x
		*/
		#region L1
		Block bL1 = new Block();
		bL1.token = 3;
		up = new BlockOrientation( 2, 3, new Vector2Int(0,0), true );
		up.tiles = new bool[,]
		{
			{ T, T, T },
			{ T, F, F }
		};
		left = new BlockOrientation( 3, 2, new Vector2Int(0,1), true );
		left.tiles = new bool[,]
		{
			{ T, T },
			{ F, T },
			{ F, T }
		};
		down = new BlockOrientation( 2, 3, new Vector2Int(1,2), true );
		down.tiles = new bool[,]
		{
			{ F, F, T },
			{ T, T, T }
		};
		right = new BlockOrientation( 3, 2, new Vector2Int(2,0), true );
		right.tiles = new bool[,]
		{
			{ T, F },
			{ T, F },
			{ T, T }
		};
		bL1.rotations = new List<BlockOrientation>(){ up, left, down, right };
		blocks.Add( bL1 );
		#endregion

		#region L2
		Block bL2 = new Block();
		bL2.token = 4;
		up = new BlockOrientation( 2, 3, new Vector2Int(1,0), true );
		up.tiles = new bool[,]
		{
			{ T, F, F },
			{ T, T, T }
		};
		left = new BlockOrientation( 3, 2, new Vector2Int(0,0), true );
		left.tiles = new bool[,]
		{
			{ T, T },
			{ T, F },
			{ T, F }
		};
		down = new BlockOrientation( 2, 3, new Vector2Int(0,2), true );
		down.tiles = new bool[,]
		{
			{ T, T, T },
			{ F, F, T }
		};
		right = new BlockOrientation( 3, 2, new Vector2Int(2,1), true );
		right.tiles = new bool[,]
		{
			{ F, T },
			{ F, T },
			{ T, T }
		};
		bL2.rotations = new List<BlockOrientation>(){ up, left, down, right };
		blocks.Add( bL2 );
		#endregion

		#region T
		Block bT = new Block();
		bT.token = 5;
		up = new BlockOrientation( 3, 2, new Vector2Int(1,0), true );
		up.tiles = new bool[,]
		{
			{ T, F },
			{ T, T },
			{ T, F }
		};
		left = new BlockOrientation( 2, 3, new Vector2Int(0,1), true );
		left.tiles = new bool[,]
		{
			{ T, T, T },
			{ F, T, F }
		};
		down = new BlockOrientation( 3, 2, new Vector2Int(1,1), true );
		down.tiles = new bool[,]
		{
			{ F, T },
			{ T, T },
			{ F, T }
		};
		right = new BlockOrientation( 2, 3, new Vector2Int(1,1), true );
		right.tiles = new bool[,]
		{
			{ F, T, F },
			{ T, T, T }
		};
		bT.rotations = new List<BlockOrientation>(){ up, left, down, right };
		blocks.Add( bT );
		#endregion

		#region Squiggle1
		Block bS1 = new Block();
		bS1.token = 6;
		up = new BlockOrientation( 3, 2, new Vector2Int(1,0), true );
		up.tiles = new bool[,]
		{
			{ T, F },
			{ T, T },
			{ F, T }
		};
		left = new BlockOrientation( 2, 3, new Vector2Int(0,1), true );
		left.tiles = new bool[,]
		{
			{ F, T, T },
			{ T, T, F }
		};
		bS1.rotations = new List<BlockOrientation>(){ up, left, up, left };
		blocks.Add( bS1 );
		#endregion

		#region Squiggle2
		Block bS2 = new Block();
		bS2.token = 7;
		up = new BlockOrientation( 3, 2, new Vector2Int(1,0), true );
		up.tiles = new bool[,]
		{
			{ F, T },
			{ T, T },
			{ T, F }
		};
		left = new BlockOrientation( 2, 3, new Vector2Int(0,1), true );
		left.tiles = new bool[,]
		{
			{ T, T, F },
			{ F, T, T }
		};
		bS2.rotations = new List<BlockOrientation>(){ up, left, up, left };
		blocks.Add( bS2 );
		#endregion
	}

}
