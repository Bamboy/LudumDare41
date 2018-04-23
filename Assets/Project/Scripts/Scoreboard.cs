using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour 
{
	private static Scoreboard _instance;
	public static Scoreboard singleton{ get{ return _instance; } }



	public TextMeshProUGUI score;

	public TextMeshProUGUI multiplier;

	public TextMeshProUGUI pendingScore;

	public TextMeshProUGUI ballHits;
	public TextMeshProUGUI rowClears;

	public void UpdateUI () 
	{
		score.text = string.Format("{0}", GameManager.singleton.score.ToString("N0"));

		if( GameManager.singleton.scoreMultiplier > 1f )
		{
			multiplier.text = string.Format("<size=90>x</size><color=red>{0}</color>", GameManager.singleton.scoreMultiplier.ToString("F1"));
		}
		else
		{
			multiplier.text = "";
		}

		if( GameManager.singleton.scoreCombo > 1f )
		{
			pendingScore.text = string.Format("{0}", GameManager.singleton.scoreCombo.ToString("N0"));
		}
		else
		{
			pendingScore.text = "";
		}

		if( GameManager.singleton.ballHitsCombo > 2 )
		{
			ballHits.text = string.Format(" Hits <size=90>x</size>{0}", GameManager.singleton.ballHitsCombo.ToString("N0"));
		}
		else
			ballHits.text = "";

		if( GameManager.singleton.rowsClearedCombo > 2 )
		{
			rowClears.text = string.Format("  Rows <size=90>x</size>{0}", GameManager.singleton.rowsClearedCombo.ToString("N0"));
		}
		else
			rowClears.text = "";
	}

	void Awake()
	{
		if( _instance == null )
		{
			_instance = this;
		}
	}

	void Start()
	{
		UpdateUI();
	}

}
