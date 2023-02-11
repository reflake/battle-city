using System;
using UnityEngine;

namespace LevelDesigner
{
	[Serializable]
	public struct SaveDataBlock
	{
		/*public int bitInformation;
		public string blockName;
		public int blockPower;
		public LayerType layerType;*/
		public int blockIndex;
		public int blockPositionX;
		public int blockPositionY;
	}
}