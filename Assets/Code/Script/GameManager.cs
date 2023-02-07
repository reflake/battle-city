using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public event GameOverDelegate OnGameOver;
	
	public async UniTaskVoid GameOver()
	{
		OnGameOver.Invoke();
		
		// Disable all players
		foreach (var player in FindObjectsOfType<Player>())
		{
			player.SetControlsEnabled(false);
		}

		await UniTask.Delay(TimeSpan.FromSeconds(5f), DelayType.Realtime, PlayerLoopTiming.Update);
		
		GoToMainMenu();
	}

	private void GoToMainMenu()
	{
		throw new NotImplementedException("Cannot transit to main menu: main menu is yet to be implemented!");
	}
}