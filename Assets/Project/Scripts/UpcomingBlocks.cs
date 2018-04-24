using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpcomingBlocks : MonoBehaviour 
{
	private static UpcomingBlocks _instance;
	public static UpcomingBlocks singleton{ get{ return _instance; } }

	void Awake()
	{
		if( _instance == null )
			_instance = this;
	}
	IEnumerator Start()
	{
		yield return null;
		UpdateUI();
	}

	public List<Image> imgs = new List<Image>(); 
	public void UpdateUI()
	{

		List<Block> blocks = new List<Block>( GameManager.singleton.upcomingBlocks );


		for (int i = 0; i < blocks.Count; i++) 
		{
			imgs[i].sprite = GameManager.singleton.tileSprites[blocks[i].token].uiIcon;
			imgs[i].color = GameManager.singleton.tileSprites[blocks[i].token].blockColor;
		}


	}
}
