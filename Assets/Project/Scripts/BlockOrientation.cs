using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockOrientation
{


	/// Defines what tiles are solid in this block.
	public bool[,] tiles;


	public Vector2Int pivot;



	public BlockOrientation( int width, int height, Vector2Int pivot, bool fillWithSolid = false )
	{
		tiles = new bool[width, height];

		this.pivot = pivot;

		if( fillWithSolid )
		{
			for (int x = 0; x < width; x++) 
			{
				for (int y = 0; y < height; y++) 
				{
					tiles[x,y] = true;
				}
			}
		}
	}



}
