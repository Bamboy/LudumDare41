using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum MenuState
{
	None,
	Pause,
	GameOver
}

public class GameOverUI : MonoBehaviour 
{
	private static GameOverUI _instance;
	public static GameOverUI singleton{ get{ return _instance; } }

	public GameObject shadeBG;
	public GameObject gameOverParent;
	public GameObject pauseParent;

	public TextMeshProUGUI score;
	public AudioClip gameoversound;

	private AudioSource player;


	public MenuState shownMenu;
	public void SetMenuState( MenuState menu )
	{
		if( menu != shownMenu )
		{
			Time.timeScale = menu != MenuState.None ? 0f : 1f;
			shadeBG.SetActive( menu != MenuState.None );

			gameOverParent.SetActive( menu == MenuState.GameOver );
			pauseParent.SetActive( menu == MenuState.Pause );

			switch( menu )
			{

			case MenuState.GameOver:
				GameManager.singleton.FinishCombo();
				player.PlayOneShot( gameoversound );
				gameOverParent.SetActive( true );
				score.text = string.Format("{0}", GameManager.singleton.score.ToString("N0"));
				SaveManager.Instance.SetHighScore( GameManager.singleton.score );
				break;

			case MenuState.Pause:
				ShowPauseUI();
				break;

			default:
				break;

			}

			shownMenu = menu;
		}
	}
		
	void ShowPauseUI()
	{

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
		SetMenuState(MenuState.None);
	}

	public void Btn_Restart()
	{
		SceneManager.LoadScene( 0 );
		Time.timeScale = 1f;
	}
}
