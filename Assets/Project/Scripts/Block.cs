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


		Block O = new Block();
		O.token = 4;
		up = new BlockOrientation( 2, 2, new Vector2Int(0,0), true );
		O.rotations = new List<BlockOrientation>(){ up, up, up, up };//Only one rotation for the square
		blocks.Add( O );


		Block I = new Block();
		I.token = 1;
		up = new BlockOrientation( 1, 4, new Vector2Int(0,0), true );
		left = new BlockOrientation( 4, 1, new Vector2Int(0,0), true );
		down = new BlockOrientation( 1, 4, new Vector2Int(0,3), true );
		right = new BlockOrientation( 4, 1, new Vector2Int(3,0), true );
		I.rotations = new List<BlockOrientation>(){ up, left, down, right };
		blocks.Add( I );

		#region L
		Block L = new Block();
		L.token = 3;
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
		L.rotations = new List<BlockOrientation>(){ up, left, down, right };
		blocks.Add( L );
		#endregion

		#region J
		Block J = new Block();
		J.token = 2;
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
		J.rotations = new List<BlockOrientation>(){ up, left, down, right };
		blocks.Add( J );
		#endregion

		#region T
		Block t = new Block();
		t.token = 6;
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
		t.rotations = new List<BlockOrientation>(){ up, left, down, right };
		blocks.Add( t );
		#endregion

		#region S
		Block S = new Block();
		S.token = 5;
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
		S.rotations = new List<BlockOrientation>(){ up, left, up, left };
		blocks.Add( S );
		#endregion

		#region Z
		Block Z = new Block();
		Z.token = 7;
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
		Z.rotations = new List<BlockOrientation>(){ up, left, up, left };
		blocks.Add( Z );
		#endregion
	}

}
