using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(SpriteRenderer))]
public class TileAnimator : MonoBehaviour 
{
	[AssetsOnly] public TileAnimationSet tileSet;

	private SpriteRenderer _renderer;

	void Awake()
	{
		_renderer = GetComponent<SpriteRenderer>();
	}

	private bool isGlitched = false;
	public void SetTiles( TileAnimationSet tiles, bool glitched = false, float duration = Mathf.Infinity )
	{
		if( tiles == this.tileSet ) //Skip if we didnt change anything
			return;
		
		if( tiles == null || tiles == GameManager.singleton.tileSprites[0] )
		{
			tileSet = GameManager.singleton.tileSprites[0]; //Default to "empty" 
			_renderer.sprite = tiles.sprites[0];
			_renderer.enabled = false;
			StopCoroutine( TileLoop(tileSet.sprites) );
			return;
		}
			
		_renderer.enabled = true;
		StopCoroutine( TileLoop(null) );

		if( glitched )
		{
			StartCoroutine( TileLoop(tiles.destroySprites, duration) );
		}
		else
		{
			StartCoroutine( TileLoop(tiles.sprites, duration ) );
		}

		isGlitched = glitched;

		tileSet = tiles;
		_spriteIndex = 0;
		_remaining = 0f;
	}

	private List<Sprite> animation;
	int _spriteIndex;
	float _remaining;
	float disableTime;
	IEnumerator TileLoop( List<Sprite> sprites, float duration = Mathf.Infinity )
	{
		animation = sprites;
		_renderer.enabled = true;
		disableTime = Time.time + duration;
		_remaining = tileSet.speed;
		while( true )
		{
			if( Time.time >= disableTime )
			{
				_renderer.enabled = false;
				break;
			}

			if( animation.Count == 1 )
			{
				yield return null;
				continue;
			}

			_remaining -= Time.deltaTime;
			if( _remaining <= 0f )
			{
				_spriteIndex = (int)Mathf.Repeat((float)_spriteIndex + 1f, (float)animation.Count);
				_renderer.sprite = animation[_spriteIndex];
				_remaining = tileSet.speed;
			}

			yield return null;
		}
	}

}
