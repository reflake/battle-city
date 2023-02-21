using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LevelDesigner
{
	public class ConstructorPanel : MonoBehaviour
	{
		public static string prefabPath = "ConstructorPanel";
		
		[Inject] readonly Constructor _constructor = null;
		[Inject] readonly CustomLevelList _customLevelList = null;
		
		[SerializeField] private Button _saveBtn;
		[SerializeField] private Button _loadBtn;

		private void Awake()
		{
			_saveBtn.onClick.AddListener(() => SaveLevel());
			_loadBtn.onClick.AddListener(() => LoadLevel());
		}

		private async UniTaskVoid SaveLevel()
		{
			var data = _constructor.GetLevelData();

			await _customLevelList.WriteLevel("Test", data);
		}

		private async UniTaskVoid LoadLevel()
		{
			var levelData = await _customLevelList.ReadLevel("Test");
			
			_constructor.LoadLevelData(levelData);
		}
	}
}