using System;
using Tanks;
using UnityEngine;

namespace Enemies
{
	[Serializable]
	public class EnemySpritesData : ITankSprites
	{
		[SerializeField] Sprite normalSprite;
		[SerializeField] Sprite poweredSprite;

		public Sprite NormalSprite => normalSprite;
		public Sprite PoweredSprite => poweredSprite;
	}
}