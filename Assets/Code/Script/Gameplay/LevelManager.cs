using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LevelDesigner;
using UI;
using UnityEngine;
using Zenject;

namespace Gameplay
{
	public class LevelManager : MonoBehaviour
	{
		[Inject] readonly Constructor _constructor;
		[Inject] readonly PanelManager _panelManager;

		[SerializeField] float transitionScreenShowTime = 1.5f;
		[SerializeField] string[] levelList;

		int _currentLevel = 0;
		TransitionScreen _panel;

		void Awake()
		{
			_panel = _panelManager.CreatePanel<TransitionScreen>(TransitionScreen.prefabPath, 3);
		}

		void OnDestroy()
		{
			Destroy(_panel);
		}

		public async UniTask SetLevel(int levelNumber, LevelData customLevelData = null)
		{
			_currentLevel = levelNumber;

			await ShowLevelTransition(levelNumber);

			if (customLevelData != null)
			{
				_constructor.LoadLevelData(customLevelData);
			}
			else
			{ 
				var levelData = await LoadLevel(levelNumber);
				
				_constructor.LoadLevelData(levelData);
			}
			
			
			await HideLevelTransition();
		}
	
		public async UniTask NextLevel()
		{
			_currentLevel = (_currentLevel + 1) % levelList.Length;

			await ShowLevelTransition(_currentLevel);
			var levelData = await LoadLevel(_currentLevel);
			
			_constructor.LoadLevelData(levelData);
			
			await HideLevelTransition();
		}

		async Task ShowLevelTransition(int levelIndex)
		{
			// need to switch to main thread so animations play
			await UniTask.SwitchToMainThread();
			_panel.Show(levelIndex + 1);
		}

		async Task HideLevelTransition()
		{
			await Task.Delay(2000, this.GetCancellationTokenOnDestroy());

			// Level loaded, ready to play
			_panel.Hide();
		}

		async UniTask<LevelData> LoadLevel(int index)
		{
			var levelDataPath = $"Level/{levelList[index]}";
			
			// Assets can be loaded only on main thread
			await UniTask.SwitchToMainThread();
		
			var asset = (await Resources.LoadAsync<TextAsset>(levelDataPath)
										.WithCancellation(this.GetCancellationTokenOnDestroy())) as TextAsset;

			using var file = new MemoryStream(asset.bytes);
			var formatter = new BinaryFormatter();

			return formatter.Deserialize(file) as LevelData;
		}
	}
}