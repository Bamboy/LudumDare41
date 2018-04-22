using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardUI : MonoBehaviour 
{
	public float tileSize = 1f;

	public GameObject tilePrefab;

	public BoardUITile[,] tiles;

	/// The board we are currently tracking for changes.
	public Board tracking;

	private bool _initalized = false;
	public void Initalize( Board data )
	{
		if( _initalized )
			return;
		if( tilePrefab == null )
		{
			Debug.LogError("Tile prefab is not specified!", this);
			Debug.Break();
		}

		tiles = new BoardUITile[data.width, data.height];
		for (int x = 0; x < data.width; x++) 
		{
			for (int y = 0; y < data.height; y++)
			{
				GameObject obj = Instantiate<GameObject>( tilePrefab, this.transform, false );
				obj.transform.localScale = new Vector3( tileSize, tileSize, tileSize );
				obj.transform.localPosition = new Vector3( x * tileSize, y * tileSize, 0f );

				BoardUITile ui = obj.GetComponent<BoardUITile>();
				ui.Initalize(x, y);
				tiles[x, y] = ui;
			}
		}

		tracking = data;

		//Update the specified BoardUITile when the specified board tile has changed.
		tracking.onResolveDirty += delegate(int tx, int ty) 
		{
			this.tiles[tx, ty].Set( data[tx, ty] );
		};

		//Whenever the board we are tracking changes, have it update this ui
		data.onAfterChanged += delegate() 
		{
			this.UpdateUI();
		}; 
			
		_initalized = true;
		UpdateUI();
	}
		
	/// Update this UI to the current state of the board.
	public void UpdateUI()
	{
		for (int x = 0; x < tracking.width; x++) 
		{
			for (int y = 0; y < tracking.height; y++) 
			{
				UpdateUI(x, y);
			}
		}

	}
	public void UpdateUI(int x, int y)
	{
		this.tiles[x, y].Set( tracking[x, y] );
	}




}
