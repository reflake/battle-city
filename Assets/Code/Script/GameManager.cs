using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
	[Inject] private readonly LevelManager _levelManager;
	
	public event GameOverDelegate OnGameOver;

	private bool _gameIsOver = false;

	async void Start()
	{
		await _levelManager.FirstLevel();
	}

	public void LevelComplete()
	{
		Task.Run(() => TransitionToNextLevel());
	}

	async UniTaskVoid TransitionToNextLevel()
	{
		await _levelManager.NextLevel();
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

	private async UniTaskVoid TransitToMainMenu()
	{
		await UniTask.Delay(TimeSpan.FromSeconds(5f), DelayType.Realtime, PlayerLoopTiming.Update);
		
		throw new NotImplementedException("Cannot transit to main menu: main menu is yet to be implemented!");
	}
}