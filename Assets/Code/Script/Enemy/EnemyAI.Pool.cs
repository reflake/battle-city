using System;
using UnityEngine;
using Zenject;

public partial class EnemyAI : IPoolable<Vector2, IMemoryPool>, IDisposable
{
	private IMemoryPool _pool = null;
	
	public void OnDespawned()
	{
		_pool = null;
	}

	public void OnSpawned(Vector2 spawnPosition, IMemoryPool pool)
	{
		Spawn(spawnPosition);
		_pool = pool;
	}

	public void Dispose()
	{
		_pool.Despawn(this);
	}
}