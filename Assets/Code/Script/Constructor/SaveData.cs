using System;
using System.Collections.Generic;

namespace LevelDesigner
{
	[Serializable]
	public class SaveData
	{
		public Dictionary<int, string> int2tileName;
		public SaveLayerData[] layers;
	}
}