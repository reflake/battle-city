using System;
using UnityEngine;
using Zenject;

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
		Tank.MaxHp = enemyData.hitPoints;
		Tank.FireRate = enemyData.fireRate;
		Tank.FirePower = enemyData.firePower;
		Tank.DamageBonus = enemyData.damageBonus;
		Tank.ProjectileSpeed = enemyData.projectileSpeed;
		Tank.Speed = enemyData.moveSpeed;
		
		Spawn(spawnPosition);
		
		_pool = pool;
	}

	public void Dispose()
	{
		_pool.Despawn(this);
	}
}