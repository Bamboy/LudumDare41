using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableAnimator : MonoBehaviour 
{
	public float animDelay = 0.1f;
	public float animDelayVariance = 0.05f;
	public List<Sprite> sprites;

	SpriteRenderer render;


	void Start()
	{
		render = GetComponent<SpriteRenderer>();
		StartCoroutine(Animate());
	}

	private int _curSpriteIndex;
	private float _animTimeLeft;
	IEnumerator Animate()
	{
		if( sprites.Count <= 1 )
			yield break;

		_curSpriteIndex = 0;
		_animTimeLeft = animDelay + Random.Range( -animDelayVariance, animDelayVariance );
		while( true )
		{
			_animTimeLeft -= Time.deltaTime;
			if( _animTimeLeft <= 0f )
			{

				while( true ) //Loop until we find a new sprite
				{
					int newIndex = Random.Range(0, sprites.Count);
					if( newIndex != _curSpriteIndex )
					{
						render.sprite = sprites[newIndex];
						break;
					}
				}


				_animTimeLeft = animDelay + Random.Range( -animDelayVariance, animDelayVariance );
			}
			yield return null;
		}
	}
}
