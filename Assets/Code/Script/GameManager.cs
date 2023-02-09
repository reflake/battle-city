using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public event GameOverDelegate OnGameOver;

	private bool _gameIsOver = false;
	
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
		
		GoToMainMenu();
	}

	private void GoToMainMenu()
	{
		throw new NotImplementedException("Cannot transit to main menu: main menu is yet to be implemented!");
	}
}