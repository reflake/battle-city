using System;
using System.Collections.Generic;

namespace LevelDesigner
{
	[Serializable]
	public class LevelData
	{
		public int[] tileMapIds;
		public string[] tileMapNames;
		public int[] tilesData;
		public int boundsX;
		public int boundsY;
		public int boundsZ;
		public int boundsW;
		public int boundsH;
		public int boundsD;
	}
}