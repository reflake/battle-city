using Gameplay;
using Players;
using UI;
using UnityEngine;
using Zenject;

namespace Players
{
	public class PlayerManager : MonoBehaviour
	{
		[SerializeField] Transform[] spawnPositions = null;
		[SerializeField] PlayerSpritesData playerSpritesData = null;
	
		[Inject] readonly GameManager _gameManager;
		[Inject] readonly Player.Factory _playerFactory;
		[Inject] readonly PanelManager _panelManager;
		[Inject] readonly Base _playerBase;
	
		public event DespawnPlayersDelegate OnDespawnPlayers;
		public event SpawnPlayersDelegate OnSpawnPlayers;

		void Awake()
		{
			var panel = _panelManager.CreatePanel<PlayerHUD>(PlayerHUD.prefabPath, 1);
		
			var spawnPosition = spawnPositions[0].position;
			var player = _playerFactory.Create(playerSpritesData);

			player.SetPlayerSpawn(spawnPosition);

			panel.HookPlayer(player);

			_playerBase.OnBaseDestroy += BaseDestroy;
		}

		void BaseDestroy()
		{
			_gameManager.GameOver();
		}

		public void PlayerDefeated()
		{
			_gameManager.GameOver();
		}

		public void DespawnAll()
		{
			OnDespawnPlayers?.Invoke();
		}

		public void SpawnPlayers()
		{
			OnSpawnPlayers?.Invoke();
		}
	}
}