using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour 
{
	#region Singleton
	private static AudioManager _singleton;
	public static AudioManager Instance
	{
		get
		{
			return _singleton;
		}
	}
	#endregion Singleton

	public AudioMixer masterMixer;
	public AudioSource lowMusic, highMusic;

	void Awake()
	{
		if( _singleton == null )
		{
			DontDestroyOnLoad( this.gameObject );
			_singleton = this;
		}
		else
			Destroy( this.gameObject );
	}
	void Start()
	{
		SetMasterVol( SaveManager.Instance.masterVol );
		SetMusicVol( SaveManager.Instance.musicVol );
		SetSFXVol( SaveManager.Instance.sfxVol );

		lowMusic.Play();
		highMusic.Play();
	}

	void Update()
	{
		// Alter the music mix based on the board's highest point
		float mix = GameManager.singleton.board.highestPoint * 1f / GameManager.singleton.board.height;
		lowMusic.volume = Mathf.Lerp(lowMusic.volume, Mathf.Sqrt(mix), Time.deltaTime);
		highMusic.volume = Mathf.Lerp(highMusic.volume, Mathf.Sqrt(1f - mix), Time.deltaTime);
	}

	/*
	public float masterVol
	{
		get{ 
			float v;
			masterMixer.GetFloat("masterVolume", out v);
			return v;
		}
	}
	public float musicVol
	{
		get{ 
			float v;
			masterMixer.GetFloat("musicVolume", out v);
			return v;
		}
	}
	public float sfxVol
	{
		get{ 
			float v;
			masterMixer.GetFloat("sfxVolume", out v);
			return v;
		}
	}
	*/
	
	public void SetMasterVol( float volume )
	{
		masterMixer.SetFloat("masterVolume", volume);
	}
	public void SetMusicVol( float volume )
	{
		masterMixer.SetFloat("musicVolume", volume);
	}
	public void SetSFXVol( float volume )
	{
		masterMixer.SetFloat("sfxVolume", volume);
	}
}
