using UnityEngine;

namespace Tanks
{
	public interface ITankSprites
	{
		public Sprite NormalSprite { get; }
		public Sprite PoweredSprite { get; }
	}
}