using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MessageBox
{
	public class OptionItem : MonoBehaviour
	{
		[SerializeField] TMP_Text _label;
		[SerializeField] Button _button;
		
		public void Setup(string title, Action action)
		{
			_label.text = title;
			_button.onClick.AddListener(() => action());
		}
	}
}