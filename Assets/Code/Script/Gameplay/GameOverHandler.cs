using Players;
using UI;
using UnityEngine;
using Zenject;

namespace Gameplay
{
	public class GameOverHandler : MonoBehaviour
	{
		[Inject] readonly GameManager _gameManager;
		[Inject] readonly PanelManager _panelManager;

		[Inject]
		public void Construct()
		{
			_gameManager.OnGameOver += Handle;
		}

		void Handle()
		{
			_panelManager.CreatePanel<GameOverPanel>(GameOverPanel.prefabPath, 2).ShowPanel();
		
			// Disable all players
			foreach (var player in FindObjectsOfType<Player>())
			{
				// TODO: stun player instead of taking control
				player.SetControlsEnabled(false);
			}
		}
	}
}