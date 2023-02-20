using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles
{
	[CreateAssetMenu]
	[Serializable]
	public class RepeatDestructibleTile : DestructibleTile
	{
		[SerializeField] Sprite[] sprites = null;
		[SerializeField] int power = 1;
		[SerializeField] Matrix4x4 transform = Matrix4x4.identity;
		[SerializeField] Color color = Color.white;
		[SerializeField] UnityEngine.Tilemaps.Tile.ColliderType colliderType = UnityEngine.Tilemaps.Tile.ColliderType.Grid;
		[SerializeField] TileFlags flags = TileFlags.LockColor;

		public override void GetTileData(Vector3Int pos, ITilemap tilemap, ref TileData data)
		{
			base.GetTileData(pos, tilemap, ref data);
		
			int modX = (int)Mathf.Repeat(pos.x, power);
			int modY = (int)Mathf.Repeat(pos.y, power);
			int spriteIndex = modX + modY * power;

			data.sprite = sprites[spriteIndex];
			data.color = color;
			data.transform = transform;
			data.colliderType = colliderType;
			data.flags = flags;
		}
	}
}