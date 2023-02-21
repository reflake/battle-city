using System;
using System.IO;
using UnityEngine;

namespace LevelDesigner
{
	public class LevelList : MonoBehaviour
	{
		void Start()
		{
#if UNITY_EDITOR
			string levelsDirectory = $"{Application.dataPath}/Resources/Level/";
#else
			string levelsDirectory = $"{Application.persistentDataPath}/Level/";
#endif
			
			string[] levels = Directory.GetFiles(levelsDirectory);
			
			Debug.Log(string.Join("\n",levels));
		}
	}
}