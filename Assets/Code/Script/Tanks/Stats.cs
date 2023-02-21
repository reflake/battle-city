using System;

namespace Tanks
{
	[Serializable]
	public struct Stats
	{
		// Amount of hits tank can take before destruction
		public int hitPoints;
		// How much bullets can shoot tank
		public int fireRate;
		// How much tank's bullets penetrate through walls
		public int firePower;
		// Bullet damage bonus
		public int damageBonus;
		// Movement speed of tank
		public float moveSpeed;
		// Bullet speed
		public float projectileSpeed;
	}
}