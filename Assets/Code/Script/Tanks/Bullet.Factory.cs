﻿using Common;
using UnityEngine;
using Zenject;

namespace Tanks
{
	public partial class Bullet
	{
		public class Factory : PlaceholderFactory<Vector3, Direction, Stats, Collider2D, Bullet>
		{
			public override Bullet Create(Vector3 position, Direction shootDirection, Stats shooterStats, Collider2D ignoreCollider2D)
			{
				return base.Create(position, shootDirection, shooterStats, ignoreCollider2D);
			}
		}
	}
}