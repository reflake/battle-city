using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using LevelDesigner;

using UnityEngine;

using Zenject;

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
		_panel = _panelManager.CreatePanel<TransitionScreen>(TransitionScreen.prefabPath, 1);
	}

	void OnDestroy()
	{
		Destroy(_panel);
	}

	public async UniTask FirstLevel()
	{
		_currentLevel = 0;

		await LoadLevel(_currentLevel);
	}
	
	public async UniTask NextLevel()
	{
		_currentLevel = (_currentLevel + 1) % levelList.Length;
		
		await LoadLevel(_currentLevel);
	}

	async UniTask LoadLevel(int index)
	{
		// Show transition screen and level number
		var waitTransitionScreen = UniTask.Delay(TimeSpan.FromSeconds(transitionScreenShowTime));

		// need to switch to main thread so animations play
		await UniTask.SwitchToMainThread();
		_panel.Show(index + 1);

		var levelDataPath = $"Level/{levelList[index]}";
		var levelLoadTask = LoadLevel(levelDataPath);
		
		await UniTask.WhenAll(levelLoadTask, waitTransitionScreen);
		
		// Level loaded, ready to play
		_panel.Hide();
	}

	async UniTask LoadLevel(string path)
	{
		// Assets can be loaded only on main thread
		await UniTask.SwitchToMainThread();
		
		var asset = (await Resources.LoadAsync<TextAsset>(path)) as TextAsset;

		using var file = new MemoryStream(asset.bytes);
		IFormatter formatter = new BinaryFormatter();

		var data = formatter.Deserialize(file) as LevelData;
		
		_constructor.LoadLevelData(data);
	}
}