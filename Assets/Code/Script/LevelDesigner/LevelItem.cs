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
		[SerializeField] Button _deleteButton;

		public Action SelectedCallback;
		public Action DeleteCallback;

		void Awake()
		{
			_button.onClick.AddListener(SelectClicked);
			_deleteButton.onClick.AddListener(DeleteClicked);
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
		
		void SelectClicked()
		{
			SelectedCallback?.Invoke();
		}

		void DeleteClicked()
		{
			DeleteCallback?.Invoke();
		}
	}
}