using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using Menu;
using Project;
using Scene;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LevelDesigner
{
	public class ConstructorPanel : MonoBehaviour
	{
		public static string prefabPath = "ConstructorPanel";
		
		[Inject] readonly Constructor _constructor = null;
		[Inject] readonly PanelManager _panelManager = null;
		[Inject] readonly SceneManager _sceneManager = null;
		
		[SerializeField] Button _saveBtn;
		[SerializeField] Button _loadBtn;
		[SerializeField] Button _startBtn;
		
		void Awake()
		{
			_saveBtn.onClick.AddListener(() => SaveLevel());
			_loadBtn.onClick.AddListener(() => LoadLevel());
			_startBtn.onClick.AddListener(() => StartGame());
		}

		LevelListDialog CreateListPanel()
		{
			return _panelManager.UsePanel<LevelListDialog>(LevelListDialog.prefabPath, 3, "LevelListDialog");
		}

		async UniTaskVoid SaveLevel()
		{
			var panel = CreateListPanel();
			var data = _constructor.GetLevelData();
			
			await panel.GetSaveFile(data);
		}

		async UniTaskVoid LoadLevel()
		{
			var panel = CreateListPanel();
			var levelData = await panel.GetLoadFile();

			_constructor.LoadLevelData(levelData);
		}

		void StartGame()
		{
			_constructor.SetNewGameCustomLevel();
			_sceneManager.MoveToMenu();
		}
	}
}