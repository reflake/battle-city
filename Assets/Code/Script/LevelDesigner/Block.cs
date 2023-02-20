using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelDesigner
{
	public class Block : MonoBehaviour
	{
		[SerializeField] Tilemap tilemap;
		[SerializeField] new string name;
		[SerializeField] bool nullBlock = false;

		public Tilemap Tilemap => tilemap;
		public string Name => name;
		public bool IsNullBlock => nullBlock;

		public void OnDrawGizmosSelected()
		{
			if (tilemap != null)
			{
				var max = tilemap.CellToWorld(tilemap.cellBounds.max);
				var min = tilemap.CellToWorld(tilemap.cellBounds.min);
				var size = max - min;

				Gizmos.color = Color.magenta;
				Gizmos.DrawWireCube(min + size / 2, size);
			}
		}

		public int GetBlockTilesNonAlloc(TileBase[] array, Matrix4x4[] transforms)
		{
			var blockSize = Tilemap.cellBounds;

			int count = Tilemap.GetTilesBlockNonAlloc(blockSize, array);

			int index = 0;
			
			for (int j = blockSize.yMin; j < blockSize.yMax; j++)
			for (int i = blockSize.xMin; i < blockSize.xMax; i++)
			{
				if (array[index] != null && array[index].name.Contains("null_block"))
				{
					array[index] = null;
				}
				else
				{
					transforms[index] = tilemap.GetTransformMatrix(new Vector3Int(i,j,0));
				}

				index++;
			}


			return count;
		}
	}
}