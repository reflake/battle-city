using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelDesigner
{
	public enum Type
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
		[SerializeField] Type type;

		public Tilemap Tilemap => tilemap;
		public Type Type => type;

		public void OnDrawGizmosSelected()
		{
			if (type != Type.Null && tilemap != null)
			{
				var max = tilemap.CellToWorld(tilemap.cellBounds.max);
				var min = tilemap.CellToWorld(tilemap.cellBounds.min);
				var size = max - min;

				Gizmos.color = Color.magenta;
				Gizmos.DrawWireCube(min + size / 2, size);
			}
		}
	}
}