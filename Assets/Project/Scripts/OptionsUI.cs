using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class OptionsUI : MonoBehaviour 
{
	#region Singleton
	private static OptionsUI _singleton;
	public static OptionsUI Instance
	{
		get
		{
			if( _singleton == null )
			{
				_singleton = GameObject.FindObjectOfType<OptionsUI>();
				if( _singleton == null )
				{
					GameObject obj = new GameObject("OptionsUI Singleton");
					_singleton = obj.AddComponent<OptionsUI>();
				}
			}
			return _singleton;
		}
	}
	#endregion Singleton

	public Slider masterVolume;
	public Slider musicVolume;
	public Slider sfxVolume;

	private bool isDirty = false; //True if we need to save settings to playerprefs
	void Start () 
	{
		

		masterVolume.onValueChanged = new Slider.SliderEvent();
		masterVolume.onValueChanged.AddListener( delegate(float val) 
			{
				AudioManager.Instance.SetMasterVol( val );
				SaveManager.Instance.masterVol = val;
				isDirty = true;
			});
		musicVolume.onValueChanged = new Slider.SliderEvent();
		musicVolume.onValueChanged.AddListener( delegate(float val) 
			{
				AudioManager.Instance.SetMusicVol( val );
				SaveManager.Instance.musicVol = val;
				isDirty = true;
			});
		sfxVolume.onValueChanged = new Slider.SliderEvent();
		sfxVolume.onValueChanged.AddListener( delegate(float val) 
			{
				AudioManager.Instance.SetSFXVol( val );
				SaveManager.Instance.sfxVol = val;
				isDirty = true;
			});
	}

	public void Btn_Resume()
	{
		GameOverUI.singleton.SetMenuState( MenuState.None );
	}
	void Update()
	{
		if( Input.GetKey(KeyCode.Delete) && Input.GetKey(KeyCode.Backspace) )
		{
			SaveManager.Instance.ClearData();
			GameOverUI.singleton.Btn_Restart();
		}
	}

	void OnEnable()
	{


		float v = SaveManager.Instance.masterVol;
		if( v != masterVolume.value )
			masterVolume.value = v;
		
		v = SaveManager.Instance.musicVol;
		if( v != musicVolume.value )
			musicVolume.value = v;	
		
		v = SaveManager.Instance.sfxVol;
		if( v != sfxVolume.value )
			sfxVolume.value = v;	
	}
	void OnDisable()
	{
		if( isDirty )
		{
			SaveManager.Instance.SaveToDisc();
			isDirty = false;
		}
	}
		
}
