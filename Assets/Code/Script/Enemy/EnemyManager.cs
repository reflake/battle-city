using System;
using UnityEngine;

using Zenject;
using Random = System.Random;

public class EnemyManager : MonoBehaviour
{
	[SerializeField] int maximumEnemiesOnScreen = 4;
	[SerializeField] int enemiesLeft = 4;
	[SerializeField] float spawnDelay = 4f;
	[SerializeField] Transform[] spawnPositions = null;
	[SerializeField] EnemyData[] enemiesData;
	
	[Inject] readonly GameManager _gameManager = null;
	[Inject] readonly PowerUpManager _powerUpManager = null;
	[Inject] readonly EnemyAI.Factory _enemyFactory = null;

	public int MaximumEnemiesOnScreen => maximumEnemiesOnScreen;

	bool _active = false;
	int _spawnCycleIndex = 0;
	int _tanksAlive = 0;
	int _enemyIndex = 0;
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

			var enemyData = enemiesData[UnityEngine.Random.Range(0, enemiesData.Length)];
			var enemy = _enemyFactory.Create(enemyData, spawnPosition);

			if (_powerUpManager.IsEnemyDropsPowerUp(_enemyIndex + 1))
			{
				enemy.ShouldDropPowerUp();
			}

			_enemyIndex++;
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
		_enemyIndex = 0;
		_nextSpawnDelay = -1;
		_spawnCycleIndex = 0;
		enemiesLeft = enemiesAmount;
	}

	public void DropPowerUp()
	{
		_powerUpManager.SpawnPowerUp();
	}
}