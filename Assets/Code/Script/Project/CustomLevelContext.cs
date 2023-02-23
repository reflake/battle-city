using System;
using LevelDesigner;

namespace Project
{
	public class CustomLevelContext
	{
		public bool HasCustomLevelForNewGame { get; private set; } = false;
		
		LevelData _customLevelData = default;

		public void SetNewGameLevel(LevelData customLevelData)
		{
			HasCustomLevelForNewGame = true;
			_customLevelData = customLevelData;
		}

		public LevelData GetNewGameLevel()
		{
			if (!HasCustomLevelForNewGame)

				throw new Exception("No custom level data");
			
			return _customLevelData;
		}

		public void ClearNewGameCustomLevel()
		{
			HasCustomLevelForNewGame = false;
			_customLevelData = null;
		}
	}
}