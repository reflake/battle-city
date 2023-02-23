using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LevelDesigner
{
	public class CustomLevelList : MonoBehaviour
	{
		public IReadOnlyList<string> LevelNames => _levelNames;

		List<string> _levelNames = new();

		public event Action<IReadOnlyList<string>> OnListUpdated;
		
#if UNITY_EDITOR
		string levelsDirectory => $"{Application.dataPath}/Resources/Level";
#else
		string levelsDirectory => $"{Application.persistentDataPath}/Level";
#endif

		void Start()
		{
			UpdateList();
		}

		void UpdateList()
		{
			_levelNames.Clear();
			_levelNames.AddRange(
				Directory.GetFiles(levelsDirectory, "*.bytes")
					.Select(levelPath => Path.GetFileNameWithoutExtension(levelPath)));
			
			OnListUpdated?.Invoke(_levelNames);
		}

		public string GetFilePath(string levelName) => $"{levelsDirectory}/{levelName}.bytes";

		public async UniTask<LevelData> ReadLevel(string levelName, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!_levelNames.Contains(levelName))
				throw new Exception($"File {levelName} not found");

			var filePath = GetFilePath(levelName);
			var binaryData = await File.ReadAllBytesAsync(filePath, cancellationToken);
			
			IFormatter formatter = new BinaryFormatter();

			using (var memoryStreamData = new MemoryStream(binaryData))
			{
				var levelData = formatter.Deserialize(memoryStreamData) as LevelData;

				return levelData;
			}
		}

		public async UniTask WriteLevel(string levelName, LevelData levelData, CancellationToken cancellationToken = default(CancellationToken))
		{
			var filePath = GetFilePath(levelName);

			IFormatter formatter = new BinaryFormatter();
			
			using (var memoryStreamData = new MemoryStream())
			{
				formatter.Serialize(memoryStreamData, levelData);

				await File.WriteAllBytesAsync(filePath, memoryStreamData.ToArray(), cancellationToken);
			}
			
			UpdateList();
		}

		public void DeleteLevel(string fileName)
		{
			var filePath = GetFilePath(fileName);
			
			_levelNames.Remove(fileName);

			File.Delete(filePath);
			
			UpdateList();
		}
	}
}