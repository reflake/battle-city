using UnityEngine;

using Zenject;

public class GameOverPanel : MonoBehaviour
{
	[SerializeField] private CanvasGroup _canvasGroup = null;

	[Inject] private readonly GameManager _gameManager = null;

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