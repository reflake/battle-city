using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelDesigner
{
	public class SaveNewButton : MonoBehaviour
	{
		[SerializeField] TMP_InputField _inputField;

		public event Action<string> OnSubmit;

		void Awake()
		{
			_inputField.onSubmit.AddListener(Submit);
		}

		void Submit(string fileName)
		{
			OnSubmit?.Invoke(fileName);
		}
	}
}