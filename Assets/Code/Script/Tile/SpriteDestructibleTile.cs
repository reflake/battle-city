using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles
{
	[CreateAssetMenu]
	[Serializable]
	public class SpriteDestructibleTile : DestructibleTile
	{
		[SerializeField] Sprite sprite;
		[SerializeField] Matrix4x4 transform = Matrix4x4.identity;
		[SerializeField] Color color = Color.white;
		[SerializeField] UnityEngine.Tilemaps.Tile.ColliderType colliderType = UnityEngine.Tilemaps.Tile.ColliderType.Grid;
		[SerializeField] TileFlags flags = TileFlags.LockColor;
		
		public override void GetTileData(Vector3Int pos, ITilemap tilemap, ref TileData data)
		{
			base.GetTileData(pos, tilemap, ref data);
		
			data.sprite = sprite;
			data.color = color;
			data.transform = transform;
			data.colliderType = colliderType;
			data.flags = flags;
		}
	}
}