using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class DestructibleTile : TileBase
{
	[SerializeField] protected DestructibleObject destructibleObject;

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
	{
		if (destructibleObject != null)
			tileData.gameObject = destructibleObject.gameObject;
	}

	public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
	{
		if (go != null)
		{
			var destructibleObject = go.GetComponent<DestructibleObject>();

			destructibleObject.Setup(name, position);
		}

		return true;
	}
}