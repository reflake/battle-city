using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace UI.MessageBox.Generic
{
	public class Builder<T> where T : Enum
	{
		readonly ShowCallback<T> _showCallback;
		
		string _description;
		List<(T, string)> _options = new();
		T _defaultOption;

		public Builder(ShowCallback<T> showCallback)
		{
			_showCallback = showCallback;
		}

		public Builder<T> AddOption(T option, string title)
		{
			_options.Add((option, title));
				
			return this;
		}

		public Builder<T> Description(string description)
		{
			_description = description;

			return this;
		}

		public Builder<T> DefaultOptions(T defaultOption)
		{
			_defaultOption = defaultOption;

			return this;
		}

		public MessageBox<T> Build()
		{
			var messageBox = new MessageBox<T>(_description, _options, _defaultOption, _showCallback);

			return messageBox;
		}
	}
}