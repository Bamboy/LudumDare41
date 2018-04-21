using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TetrisBlock : MonoBehaviour
{
	private Block template;

	public int rotation;

	/// Position of our pivot on the game board.
	public Vector2Int position;

	public void Initalize( Block template )
	{
		this.template = template;
		rotation = 0;
		position = GameManager.singleton.board.blockSpawn;
	}


	public void OnGameUpdate()
	{
		position += new Vector2Int(0, -1);


		//TODO check if we intersect something. If we do, do not move
		// and instead spawn a new block

	}



	/// Tries to rotate this block. Returns true if the rotation is possible.
	public bool Rotate( bool clockwise )
	{
		rotation = clockwise ? rotation + 1 : rotation - 1;
		return false;
	}


}
