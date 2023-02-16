using UnityEngine;

namespace Code.Script.Tank
{
	[CreateAssetMenu(fileName = "Tank Sprites", menuName = "Tank/Tank Sprites", order = 0)]
	public class TankSprites : ScriptableObject
	{
		[SerializeField] Sprite normalSprite;
		[SerializeField] Sprite poweredSprite;

		public Sprite NormalSprite => normalSprite;
		public Sprite PoweredSprite => poweredSprite;
	}
}