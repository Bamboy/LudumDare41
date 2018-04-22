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

	public void SetTiles( TileAnimationSet tiles )
	{
		if( tiles.sprites.Count == 1 )
			_renderer.sprite = tileSet.sprites[0];

		tileSet = tiles;
		_spriteIndex = 0;
		_remaining = 0f;
	}

	void OnEnable()
	{
		StartCoroutine( TileLoop() );
	}
	void OnDisable()
	{
		StopCoroutine( TileLoop() );
	}

	int _spriteIndex;
	float _remaining;
	IEnumerator TileLoop()
	{
		_remaining = tileSet.speed;
		while( true )
		{
			if( tileSet.sprites.Count == 1 )
			{
				yield return null;
				continue;
			}

			_remaining -= Time.deltaTime;
			if( _remaining <= 0f )
			{
				_spriteIndex = (int)Mathf.Repeat((float)_spriteIndex + 1f, (float)tileSet.sprites.Count);
				_renderer.sprite = tileSet.sprites[_spriteIndex];
				_remaining = tileSet.speed;
			}

			yield return null;

		}
	}
}
