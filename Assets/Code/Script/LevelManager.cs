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
	
	[SerializeField] string[] levelList;

	int _currentLevel = 0;

	public void FirstLevel()
	{
		// TODO: show first level title
		_currentLevel = 0;
		
		Task.Run(() => LoadLevel(_currentLevel));
	}
	
	public void NextLevel()
	{
		// TODO: show next level Title
		
		Task.Run(() => LoadLevel(++_currentLevel));
	}

	async UniTaskVoid LoadLevel(int index)
	{
		string path = $"Level/{levelList[index]}";
		
		var levelData = await LoadAsLevelData(path);
		
		_constructor.LoadLevelData(levelData);
	}

	async UniTask<LevelData> LoadAsLevelData(string path)
	{
		// Assets can be loaded only on main thread
		await UniTask.SwitchToMainThread();
		
		var asset = (await Resources.LoadAsync<TextAsset>(path)) as TextAsset;

		using var file = new MemoryStream(asset.bytes);
		IFormatter formatter = new BinaryFormatter();

		var data = formatter.Deserialize(file) as LevelData;

		return data;
	}
}