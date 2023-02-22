using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class Spinner : MonoBehaviour
	{
		[SerializeField] Image _headImage = null;
		
		Sequence _sequence;

		void OnEnable()
		{
			_sequence = DOTween.Sequence();

			var rectTransform = _headImage.rectTransform;
			var rotation = new Vector3(0, 0, 360f);
			
			_sequence
				.Append(rectTransform.DOLocalRotate(rotation, 6f, RotateMode.LocalAxisAdd)
					.SetEase(Ease.OutQuad))
				.SetLoops(-1);
		}

		void OnDisable()
		{
			_sequence?.Kill();
		}
	}
}