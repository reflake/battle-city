using System;
using Common;
using UnityEngine;
using Zenject;

namespace Tanks
{
	public partial class Bullet : IPoolable<Team, Vector3, Direction, Stats, Collider2D, IMemoryPool>, IDisposable
	{
		IMemoryPool _pool;
		
		public void OnDespawned()
		{
			_pool = null;
		}

		public void OnSpawned(Team team, Vector3 position, Direction direction, Stats shooterStats, Collider2D ignoreCollider, IMemoryPool pool)
		{
			transform.parent = null;
			
			Shoot(team, position, direction, shooterStats, ignoreCollider);
			
			_pool = pool;
		}

		public void Dispose()
		{
			_pool.Despawn(this);
		}
	}
}