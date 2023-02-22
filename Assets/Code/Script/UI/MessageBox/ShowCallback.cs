using System;
using UI.MessageBox.Generic;

namespace UI.MessageBox
{
	public delegate void ShowCallback<T>(MessageBox<T> messageBox, AnswerDelegate answerCallback) where T : Enum;
}