using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelDesigner
{
	[Serializable]
	public struct SaveLayerData
	{
		public LayerType layerType;
		public int[] tilesData;
		public int boundsX;
		public int boundsY;
		public int boundsZ;
		public int boundsW;
		public int boundsH;
		public int boundsD;
	}
}