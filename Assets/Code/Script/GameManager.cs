using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
	[Inject] readonly LevelManager _levelManager;
	[Inject] readonly EnemyManager _enemyManager;
	[Inject] readonly PlayerManager _playerManager;
	
	public event GameOverDelegate OnGameOver;

	bool _gameIsOver = false;
	int waveStrength = 0;

	void Start()
	{
		FirstLevel();
	}

	async UniTaskVoid FirstLevel()
	{
		await _levelManager.FirstLevel();

		BeginGame();
	}

	public void LevelComplete()
	{
		TransitionToNextLevel();
	}

	async UniTaskVoid TransitionToNextLevel()
	{
		// Show scores
		
		_playerManager.DespawnAll();
		
		await _levelManager.NextLevel();
		
		BeginGame();
	}

	void BeginGame()
	{
		// Start spawn enemies
		int enemiesAmount = 11 + waveStrength++;

		_enemyManager.SetEnemiesWave(enemiesAmount);

		// Prepare players
		_playerManager.SpawnPlayers();
	}
	
	public void GameOver()
	{
		if (_gameIsOver)
			return;

		_gameIsOver = true;

		OnGameOver.Invoke();
		
		// Disable all players
		foreach (var player in FindObjectsOfType<Player>())
		{
			player.SetControlsEnabled(false);
		}

		Task.Run(() => TransitToMainMenu());
	}

	async UniTaskVoid TransitToMainMenu()
	{
		await UniTask.Delay(TimeSpan.FromSeconds(5f), DelayType.Realtime, PlayerLoopTiming.Update);
		
		throw new NotImplementedException("Cannot transit to main menu: main menu is yet to be implemented!");
	}
}