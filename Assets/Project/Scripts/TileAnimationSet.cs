using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName="TileAnimationSet")]
public class TileAnimationSet : ScriptableObject
{
	public Color blockColor = Color.white;
	public float speed = 0.2f;


	public List<Sprite> sprites;

	public List<Sprite> destroySprites;

}
