using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LevelDesigner
{
	public class Panel : MonoBehaviour
	{
		[Inject] readonly Constructor _constructor = null;
		
		[SerializeField] private Button _saveBtn;
		[SerializeField] private Button _loadBtn;

		private void Awake()
		{
			_saveBtn.onClick.AddListener(SaveLevel);
			_loadBtn.onClick.AddListener(LoadLevel);
		}

		private void OnDestroy()
		{
			_saveBtn.onClick.RemoveListener(SaveLevel);
			_loadBtn.onClick.RemoveListener(LoadLevel);
		}

		private void SaveLevel()
		{
			string fileName = $"{Application.dataPath}/Level/Test.level";
			IFormatter formatter = new BinaryFormatter();

			using (var file = new FileStream(fileName, FileMode.Create))
			{
				var data = _constructor.GetSerializedData();
				
				formatter.Serialize(file, data);
				file.Close();
			}
		}

		private void LoadLevel()
		{
			string fileName = $"{Application.dataPath}/Level/Test.level";
			IFormatter formatter = new BinaryFormatter();

			using (var file = new FileStream(fileName, FileMode.Open))
			{
				var data = formatter.Deserialize(file) as SaveDataBlock[];

				_constructor.LoadLevel(data);
			}
		}
	}
}