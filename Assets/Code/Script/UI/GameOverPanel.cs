using Gameplay;
using UnityEngine;
using Zenject;

namespace UI
{
	public class GameOverPanel : MonoBehaviour
	{
		public static readonly string prefabPath = "GameOverPanel";
	
		[SerializeField] private CanvasGroup _canvasGroup = null;

		[Inject] readonly GameManager _gameManager = null;

		[Inject]
		public void Construct()
		{
			_gameManager.OnGameOver += ShowPanel;
		}
	
		public void ShowPanel()
		{
			_canvasGroup.alpha = 1f;
		}
	}
}