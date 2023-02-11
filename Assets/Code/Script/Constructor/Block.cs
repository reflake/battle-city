using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelDesigner
{
	public enum LayerType
	{
		Null = 0,
		Brick,
		Concrete,
		Top,
		Bottom
	}
	
	public class Block : MonoBehaviour
	{
		[SerializeField] Tilemap tilemap;
		[SerializeField] LayerType layerType;
		[SerializeField] new string name;

		public Tilemap Tilemap => tilemap;
		public LayerType LayerType => layerType;
		public string Name => name;

		public void OnDrawGizmosSelected()
		{
			if (layerType != LayerType.Null && tilemap != null)
			{
				var max = tilemap.CellToWorld(tilemap.cellBounds.max);
				var min = tilemap.CellToWorld(tilemap.cellBounds.min);
				var size = max - min;

				Gizmos.color = Color.magenta;
				Gizmos.DrawWireCube(min + size / 2, size);
			}
		}

		public int GetBlockTilesNonAlloc(TileBase[] array)
		{
			var blockSize = Tilemap.cellBounds;

			int count = Tilemap.GetTilesBlockNonAlloc(blockSize, array);
			
			for (int i = 0; i < count; i++)
			{
				if (array[i] != null && array[i].name.Contains("null_block"))
				{
					array[i] = null;
				}
			}

			return count;
		}
	}
}