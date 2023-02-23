using System;
using UnityEngine;
using Zenject;

namespace Effects
{
	public partial class Effect : IPoolable<Vector2, AnimationData, IMemoryPool>, IDisposable
	{
		IMemoryPool _pool = null;
		
		public void OnDespawned()
		{
			_pool = null;
		}

		public void OnSpawned(Vector2 spawnPosition, AnimationData animationData, IMemoryPool pool)
		{
			_pool = pool;
			
			gameObject.SetActive(true);

			transform.position = spawnPosition;

			_animationData = animationData;
			_timer = 0f;
		}

		public void Dispose()
		{
			gameObject.SetActive(false);
			
			_pool.Despawn(this);
		}
	}
}