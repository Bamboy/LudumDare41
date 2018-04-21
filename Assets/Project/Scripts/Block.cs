using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// A block of tiles that are glued together.
/// </summary>
public struct Block
{
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

		Block square = new Block();
		square.token = 1;
		up = new BlockOrientation( 2, 2, new Vector2Int(0,0), true );
		square.rotations = new List<BlockOrientation>(){ up, up, up, up };//Only one rotation for the square
		blocks.Add( square );


		Block b1x4 = new Block();
		b1x4.token = 1;
		up = new BlockOrientation( 1, 4, new Vector2Int(0,0), true );
		left = new BlockOrientation( 4, 1, new Vector2Int(0,0), true );
		down = new BlockOrientation( 1, 4, new Vector2Int(0,3), true );
		right = new BlockOrientation( 4, 1, new Vector2Int(3,0), true );
		b1x4.rotations = new List<BlockOrientation>(){ up, left, down, right };
		blocks.Add( b1x4 );
	}

}
