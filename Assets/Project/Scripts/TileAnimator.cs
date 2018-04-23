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
		if( tiles == null )
			return;
		/*
		if( tiles.name == "Empty" )
		{
			_renderer.enabled = false;
			Debug.Log(_renderer.sprite.name, this.gameObject);

			isGlitched = false;
			return;
		} */

		if( tiles == this.tileSet && isGlitched )
			return;


		_renderer.enabled = true;

		if( glitched != isGlitched || this.tileSet != tiles )
		{
			StopCoroutine( TileLoop(null) );

			if( glitched )
			{
				StartCoroutine( TileLoop(tiles.destroySprites, duration) );
			}
			else
			{
				StartCoroutine( TileLoop(tiles.sprites, duration ) );
			}
		}

		isGlitched = glitched;

		tileSet = tiles;
		_spriteIndex = 0;
		_remaining = 0f;
	}

	void OnEnable()
	{
		StartCoroutine( TileLoop(tileSet.sprites) );
	}
	void OnDisable()
	{
		StopCoroutine( TileLoop(null) );
	}

	int _spriteIndex;
	float _remaining;
	float disableTime;
	IEnumerator TileLoop( List<Sprite> sprites, float duration = Mathf.Infinity )
	{
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

			if( sprites.Count == 1 )
			{
				yield return null;
				continue;
			}

			_remaining -= Time.deltaTime;
			if( _remaining <= 0f )
			{
				_spriteIndex = (int)Mathf.Repeat((float)_spriteIndex + 1f, (float)sprites.Count);
				_renderer.sprite = sprites[_spriteIndex];
				_remaining = tileSet.speed;
			}

			yield return null;
		}
	}

}
