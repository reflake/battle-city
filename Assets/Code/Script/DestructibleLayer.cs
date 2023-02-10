using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleLayer : MonoBehaviour, IDestructible
{
	[SerializeField] private Tilemap _tilemap = null;
	
	public bool Alive => true;

	private float damageSizeRadius = 0.7f;

	public void TakeDamage(DamageData damageData)
	{
		Vector2 damageSize = GetNormalDamageSize(damageData.direction) * damageSizeRadius;
		
		Bounds damageBounds = new Bounds(damageData.position, damageSize);
		BoundsInt boundsInt = new BoundsInt();

		boundsInt.SetMinMax(_tilemap.WorldToCell(damageBounds.min), _tilemap.WorldToCell(damageBounds.max));

		switch (damageData.direction)
		{
			case Direction.North:
			case Direction.South:
			{
				int offset = boundsInt.xMax - boundsInt.xMin - 3;
				boundsInt.xMin += offset;
			}
				break;
			case Direction.East:
			case Direction.West:
			{
				int offset = boundsInt.yMax - boundsInt.yMin - 3;
				boundsInt.yMin += offset;
			}
				break;
		}

		for (int i = boundsInt.xMin; i <= boundsInt.xMax; i++)
		for (int j = boundsInt.yMin; j <= boundsInt.yMax; j++)
		{
			Vector3Int position = new Vector3Int(i, j);
			var tile = _tilemap.GetTile(position);
			
			if (tile != null)
			{
				_tilemap.SetTile(position, null);
			}
		}
	}

	private Vector2 GetNormalDamageSize(Direction direction)
	{
		switch (direction)
		{
			case Direction.North:
			case Direction.South:
				return new Vector2(1, 0.2f);
			case Direction.East:
			case Direction.West:
				return new Vector2(0.2f, 1f);
		}

		throw new Exception("Unexpected behaviour!");
	}
}