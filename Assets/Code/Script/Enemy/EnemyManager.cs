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

	int _spawnCycleIndex = 0;
	int _tanksAlive = 0;
	float _nextSpawnDelay = -1;

	void Update()
	{
		if (_tanksAlive < maximumEnemiesOnScreen && _nextSpawnDelay < Time.time)
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
			_gameManager.LevelComplete();
		}
	}
}