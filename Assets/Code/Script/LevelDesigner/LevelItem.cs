using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LevelDesigner
{
	public class LevelItem : MonoBehaviour
	{
		[SerializeField] TMP_Text _nameLabel;
		[SerializeField] Image _background;
		[SerializeField] Button _button;

		public Action ClickCallback;

		void Awake()
		{
			_button.onClick.AddListener(Click);
		}

		public string Name
		{
			get => _nameLabel.text;
			set => _nameLabel.text = value;
		}

		public void SetBackgroundColor(Color color)
		{
			_nameLabel.color = color.grayscale > .5f ? Color.black : Color.white;
			_background.color = color;
		}
		
		void Click()
		{
			ClickCallback?.Invoke();
		}
	}
}