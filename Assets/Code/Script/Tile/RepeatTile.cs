using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
[Serializable]
public class RepeatTile : TileBase
{
	[SerializeField] private Sprite[] sprites = null;
	[SerializeField] private int power = 1;
	[SerializeField] private Matrix4x4 transform = Matrix4x4.identity;
	[SerializeField] private Color color = Color.white;
	[SerializeField] private Tile.ColliderType colliderType = Tile.ColliderType.Grid;
	[SerializeField] private TileFlags flags = TileFlags.LockColor;

	public override void GetTileData(Vector3Int pos, ITilemap tilemap, ref TileData data)
	{
		int modX = (int)Mathf.Repeat(pos.x, power);
		int modY = (int)Mathf.Repeat(pos.y, power);
		int spriteIndex = modX + modY * power;

		data.sprite = sprites[spriteIndex];
		data.color = color;
		data.transform = transform;
		data.colliderType = colliderType;
		data.flags = flags;
		data.gameObject = null;
	}
}