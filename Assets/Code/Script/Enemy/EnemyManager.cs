using System;
using UnityEngine;

using Zenject;

public class EnemyManager : MonoBehaviour
{
	[SerializeField] int maximumEnemiesOnScreen = 4;
	[SerializeField] int enemiesLeft = 4;
	[SerializeField] float spawnDelay = 4f;
	[SerializeField] Transform[] spawnPositions = null;
	
	[Inject] readonly GameManager _gameManager = null;
	[Inject] readonly EnemyAI.Factory _enemyFactory = null;

	public int MaximumEnemiesOnScreen => maximumEnemiesOnScreen;

	bool _active = false;
	int _spawnCycleIndex = 0;
	int _tanksAlive = 0;
	float _nextSpawnDelay = -1;

	void Update()
	{
		bool timeForRespawn = _nextSpawnDelay < Time.time;
		bool notEnoughEnemyOnScreen = _tanksAlive < Mathf.Min(maximumEnemiesOnScreen, enemiesLeft);

		if (_active && notEnoughEnemyOnScreen && timeForRespawn)
		{
			// Spawn enemy
			Transform chosenSpawn = spawnPositions[_spawnCycleIndex % spawnPositions.Length];
			Vector2 spawnPosition = chosenSpawn.transform.position;
			
			_enemyFactory.Create(spawnPosition);
			
			_tanksAlive++;
			_spawnCycleIndex++;
			_nextSpawnDelay = Time.time + spawnDelay;
		}
	}

	public void EnemyKilled(EnemyAI killedEnemy)
	{
		enemiesLeft--;
		_tanksAlive--;

		if (enemiesLeft == 0)
		{
			// Stop spawning after all enemies destroyed
			_active = false;
			_gameManager.LevelComplete();
		}
	}

	public void SetEnemiesWave(int enemiesAmount)
	{
		_active = true;
		_nextSpawnDelay = -1;
		_spawnCycleIndex = 0;
		enemiesLeft = enemiesAmount;
	}
}