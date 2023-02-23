using System;
using System.Threading.Tasks;
using Players;
using Cysharp.Threading.Tasks;
using Enemies;
using LevelDesigner;
using UnityEngine;
using Zenject;
using Scene;

namespace Gameplay
{
	public class GameManager : MonoBehaviour
	{
		[Inject] readonly LevelManager _levelManager;
		[Inject] readonly EnemyManager _enemyManager;
		[Inject] readonly PlayerManager _playerManager;
		[Inject] readonly SceneManager _sceneManager;
	
		public event GameOverDelegate OnGameOver;

		bool _gameIsOver = false;
		int waveStrength = 0;

		public async UniTaskVoid StartLevel(int levelNumber, LevelData customLevelData = null)
		{
			gameObject.SetActive(true);
			
			await _levelManager.SetLevel(levelNumber, customLevelData);

			BeginGame();
		}

		public async UniTaskVoid LevelComplete()
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

			Task.Run(() => TransitToMainMenu());
		}

		async UniTaskVoid TransitToMainMenu()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(5f), DelayType.Realtime, PlayerLoopTiming.Update);
			
			_sceneManager.MoveToMenu();
		}
	}
}