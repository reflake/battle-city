using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UI.MessageBox;

namespace UI.MessageBox.Generic
{
	public partial class MessageBox<T> where T : Enum
	{
		public string Description { get; private set; }
		public List<(T, string)> Options { get; private set; }
		public T DefaultOption { get; set; }

		readonly ShowCallback<T> _showCallback;

		public MessageBox(string description, List<(T, string)> options, T defaultOption, ShowCallback<T> showCallback)
		{
			Description = description;
			Options = options;
			DefaultOption = defaultOption;
			
			_showCallback = showCallback;
		}

		public async UniTask<T> AsyncShow()
		{
			T selectedOption = DefaultOption;
			bool waitingAnswer = true;
			
			void Answer(Enum option)
			{
				selectedOption = (T)option;
				waitingAnswer = false;
			}
			
			_showCallback?.Invoke(this, Answer);
			
			await UniTask.WaitWhile(() => waitingAnswer);

			return selectedOption;
		}
	}
}