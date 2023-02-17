using System;
using UnityEngine;

[Serializable]
public class EnemySpritesData : ITankSprites
{
	[SerializeField] Sprite normalSprite;
	[SerializeField] Sprite poweredSprite;

	public Sprite NormalSprite => normalSprite;
	public Sprite PoweredSprite => poweredSprite;
}