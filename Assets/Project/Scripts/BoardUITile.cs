using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class BoardUITile : MonoBehaviour 
{

	public BoxCollider2D collision;
	public SpriteRenderer render;

	public TextMeshPro text;

	private int x;
	private int y;
	[HideInInspector]public int currentToken = -int.MaxValue;

	public void Initalize(int x, int y)
	{
		this.x = x;
		this.y = y;

		text.text = string.Format("[{0}, {1}]", x, y);
	}

	public void Set( int token )
	{
		if( token == currentToken )
		{
			Debug.Log("No change");
			return;
		}
		collision.enabled = token != 0;
			

		render.sprite = GameManager.singleton.tileSprites[token];
		currentToken = token;
	}
}
