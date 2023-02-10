using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelDesigner
{
	public class Block : MonoBehaviour
	{
		[SerializeField] Tilemap tilemap;

		public Tilemap Tilemap => tilemap;

		public void OnDrawGizmosSelected()
		{
			var max = tilemap.CellToWorld(tilemap.cellBounds.max);
			var min = tilemap.CellToWorld(tilemap.cellBounds.min);
			var size = max - min;
			
			Gizmos.color = Color.gray;
			Gizmos.DrawWireCube(min + size / 2, size);
		}
	}
}