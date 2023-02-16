using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

public class GameManager : MonoBehaviour
{
	[Inject] readonly LevelManager _levelManager;
	[Inject] readonly EnemyManager _enemyManager;
	[Inject] readonly PlayerManager _playerManager;
	[Inject] readonly ZenjectSceneLoader _sceneLoader;
	
	public event GameOverDelegate OnGameOver;

	bool _gameIsOver = false;
	int waveStrength = 0;
	Keyboard _kbDevice;

	void Start()
	{
		FirstLevel();

		_kbDevice = InputSystem.GetDevice<Keyboard>();
	}

	void Update()
	{
		if (_kbDevice.escapeKey.IsPressed())
		{
			_sceneLoader.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
		}
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

		Task.Run(() => TransitToMainMenu());
	}

	async UniTaskVoid TransitToMainMenu()
	{
		await UniTask.Delay(TimeSpan.FromSeconds(5f), DelayType.Realtime, PlayerLoopTiming.Update);
		await _sceneLoader.LoadSceneAsync("MainMenu", LoadSceneMode.Single, container =>
		{
			// TODO: maybe pass HiScore there to highlight it in main menu
		});
	}
}