using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour 
{
	private static GameOverUI _instance;
	public static GameOverUI singleton{ get{ return _instance; } }

	public GameObject uiParent;
	public TextMeshProUGUI score;
	public AudioClip gameoversound;

	private AudioSource player;
	public void UpdateUI()
	{
		if( GameManager.singleton.isGameOver == false )
		{
			uiParent.SetActive( false );
		}
		else
		{
			GameManager.singleton.FinishCombo();

			player.PlayOneShot( gameoversound );

			uiParent.SetActive( true );

			score.text = string.Format("{0}", GameManager.singleton.score.ToString("N0"));
		}
	}

	void Awake()
	{
		if( _instance == null )
			_instance = this;
	}

	IEnumerator Start () 
	{
		yield return null;
		player = GetComponent<AudioSource>();
		UpdateUI();
	}

	public void Btn_Restart()
	{
		SceneManager.LoadScene( 0 );
		Time.timeScale = 1f;
	}
}
