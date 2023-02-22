using System;
using TMPro;
using UI.MessageBox.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.MessageBox
{
	public class MessageBoxPanel : MonoBehaviour
	{
		[SerializeField] TMP_Text _descriptionLabel;
		[SerializeField] OptionItem _optionItemPrefab;
		[SerializeField] Button _cancelButton;
		[SerializeField] Button _backButton;
		[SerializeField] Transform _container;

		public void Setup<TOptions>(MessageBox<TOptions> messageBox, AnswerDelegate answerCallback) where TOptions : Enum
		{
			void Answer(TOptions option)
			{
				Destroy(gameObject);
				
				answerCallback?.Invoke(option);
			}
			
			_descriptionLabel.text = messageBox.Description;

			_cancelButton.onClick.AddListener(() => Answer(messageBox.DefaultOption));
			_backButton.onClick.AddListener(() => Answer(messageBox.DefaultOption));
			
			foreach ((var option, var title) in messageBox.Options)
			{
				var newItem = Instantiate(_optionItemPrefab, _container);

				newItem.Setup(title, () => Answer(option));
			}
		}
	}
}