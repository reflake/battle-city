using System;
using UnityEngine;
using Zenject;

namespace Enemies
{
	public partial class EnemyAI : IPoolable<EnemyData, Vector2, IMemoryPool>, IDisposable
	{
		private IMemoryPool _pool = null;
	
		public void OnDespawned()
		{
			_pool = null;
		}

		public void OnSpawned(EnemyData enemyData, Vector2 spawnPosition, IMemoryPool pool)
		{
			Tank.SpritesData = enemyData.spritesData;
			Tank.Stats = enemyData.stats;
		
			Spawn(spawnPosition);
		
			_pool = pool;
		}

		public void Dispose()
		{
			_pool.Despawn(this);
		}
	}
}