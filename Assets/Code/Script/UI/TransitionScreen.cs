using DG.Tweening;
using TMPro;
using UnityEngine;

public class TransitionScreen : MonoBehaviour
{
	public static string prefabPath = "TransitionScreen";

	[SerializeField] TMP_Text _levelNumberLabel = null;
	[SerializeField] CanvasGroup _canvasGroup = null;

	const string levelNumberFormat = "LEVEL {0}";
	
	public void Show(int levelNumber)
	{
		_levelNumberLabel.text = string.Format(levelNumberFormat, levelNumber);
		_canvasGroup.alpha = 1f;
	}

	public void Hide()
	{
		_canvasGroup.DOFade(0f, .5f);
	}
}