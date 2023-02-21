using Gameplay;
using UnityEngine;
using Zenject;

namespace UI
{
	public class GameOverPanel : MonoBehaviour
	{
		public static readonly string prefabPath = "GameOverPanel";
	
		[SerializeField] private CanvasGroup _canvasGroup = null;

		public void ShowPanel()
		{
			_canvasGroup.alpha = 1f;
		}
	}
}