using Enemies;
using TMPro;
using UnityEngine;
using Zenject;

namespace Players
{
	public class PlayerHUD : MonoBehaviour
	{
		public static string prefabPath => "PlayerHUD";

		[SerializeField] TMP_Text _playerLivesLabel;
		[SerializeField] TMP_Text _enemiesLeftLabel;

		[Inject] readonly EnemyManager _enemyManager;

		Player hookedPlayer;
	
		void Awake()
		{
			SetEnemiesLeft(_enemyManager.EnemiesLeft);

			_enemyManager.OnEnemyLeftChange += SetEnemiesLeft;
		}

		void OnDestroy()
		{
			if (_enemyManager)
				_enemyManager.OnEnemyLeftChange -= SetEnemiesLeft;

			if (hookedPlayer)
				hookedPlayer.OnLivesChange -= SetLives;
		}

		public void HookPlayer(Player player)
		{
			SetLives(player.Lives);

			player.OnLivesChange += SetLives;
		}

		void SetEnemiesLeft(int amount)
		{
			_enemiesLeftLabel.text = $"ENEMY: {amount}";
		}

		void SetLives(int lives)
		{
			_playerLivesLabel.text = $"LIVES: {lives}";
		}
	}
}