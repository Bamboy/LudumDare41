using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour 
{
	#region Singleton
	private static SaveManager _singleton;
	public static SaveManager Instance
	{
		get
		{
			if( _singleton == null )
			{
				_singleton = GameObject.FindObjectOfType<SaveManager>();
				if( _singleton == null )
				{
					GameObject obj = new GameObject("SaveManager Singleton");
					_singleton = obj.AddComponent<SaveManager>();
				}
			}
			return _singleton;
		}
	}
	#endregion Singleton


	public float masterVol, musicVol, sfxVol = -10f;
	public int highscore = 0;

	public bool hasHighscore{ get{ return PlayerPrefs.HasKey("Highscore") == true && PlayerPrefs.GetInt("Highscore") > 0; } }
	public void Awake()
	{
		Load();
	}

	public void Load()
	{
		if( hasHighscore == false )
		{
			//Create default playerprefs
			masterVol = -10f;
			musicVol = -10f;
			sfxVol = -10f;
			highscore = 0;

			PlayerPrefs.SetFloat("MasterVolume", masterVol);
			PlayerPrefs.SetFloat("MusicVolume", musicVol);
			PlayerPrefs.SetFloat("SFXVolume", sfxVol);
			PlayerPrefs.SetInt("Highscore", highscore);
		}
		else
		{
			Debug.Log("Existing data found");
			masterVol = PlayerPrefs.GetFloat("MasterVolume");
			musicVol = PlayerPrefs.GetFloat("MusicVolume");
			sfxVol = PlayerPrefs.GetFloat("SFXVolume");
			highscore = PlayerPrefs.GetInt("Highscore");
		}
		/*
		AudioManager.Instance.SetMasterVol( masterVol );
		AudioManager.Instance.SetMusicVol( musicVol );
		AudioManager.Instance.SetSFXVol( sfxVol ); */
		//"MasterVolume"
		//"MusicVolume"
		//"SFXVolume"
		//"HighScore"
	}
	public void SetHighScore( int score )
	{
		if( score > this.highscore )
		{
			PlayerPrefs.SetInt("Highscore", score);
			this.highscore = score;
			SaveToDisc();
		}
	}

	public void SaveToDisc()
	{
		PlayerPrefs.SetFloat("MasterVolume", masterVol);
		PlayerPrefs.SetFloat("MusicVolume", musicVol);
		PlayerPrefs.SetFloat("SFXVolume", sfxVol);
		PlayerPrefs.Save();
		Debug.Log("Saved!");
	}

	public void ClearData()
	{
		PlayerPrefs.DeleteAll();
		masterVol = -10f;
		musicVol = -10f;
		sfxVol = -10f;
		highscore = 0;
		SaveToDisc();
	}
}
