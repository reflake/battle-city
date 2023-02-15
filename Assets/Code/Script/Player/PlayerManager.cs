using UnityEngine;

using Zenject;

public class PlayerManager : MonoBehaviour
{
	[Inject] readonly GameManager _gameManager;

	public event SpawnPlayersDelegate OnSpawnPlayers;

	public void PlayerDefeated()
	{
		_gameManager.GameOver();
	}

	public void SpawnPlayers()
	{
		OnSpawnPlayers?.Invoke();
	}
}