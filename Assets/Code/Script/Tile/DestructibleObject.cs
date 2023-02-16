using System;
using System.Linq;
using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDestructible
{
	[SerializeField] int hp;
	[SerializeField] int durability;

	public bool Alive { get; private set; } = true;

	bool set = false;
	string _tileName;
	Vector3Int _position;

	public void Setup(string tileName, Vector3Int position)
	{
		if (set)
			throw new Exception("Already set!");
		
		set = true;
		_tileName = tileName;
		_position = position;
	}

	public void TakeDamage(DamageData damageData)
	{
		if (Alive && damageData.strength >= durability)
		{
			var tilemap = BattleField.Instance.Tilemap;
			
			Vector2 damageSize = GetNormalDamageSize(damageData.direction);
			Bounds damageBounds = new Bounds(damageData.position, damageSize);
			BoundsInt boundsInt = new BoundsInt();

			boundsInt.SetMinMax(tilemap.WorldToCell(damageBounds.min), tilemap.WorldToCell(damageBounds.max));

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

			const float boundsEpsilon = 0.04f;
			Bounds worldBounds = new Bounds();
			
			worldBounds.SetMinMax(tilemap.CellToWorld(boundsInt.min), tilemap.CellToWorld(boundsInt.max));
			worldBounds.Expand(-boundsEpsilon);

			var hits = Physics2D.OverlapBoxAll(damageData.position, worldBounds.size, 0f,
				LayerMask.GetMask("Default"));

			foreach (var collider in hits.Where(x => x.CompareTag("Destructible")))
			{
				if (collider.TryGetComponent<DestructibleObject>(out var otherDestructibleObject) &&
				    otherDestructibleObject._tileName == _tileName)
				{
					otherDestructibleObject.InnerTakeDamage();
				}
			}
			
			// InnerTakeDamage();
		}
	}

	void InnerTakeDamage()
	{
		hp--;

		if (hp <= 0)
		{
			Alive = false;

			BattleField.Instance.Tilemap.SetTile(_position, null);
		}
	}
	
	Vector2 GetNormalDamageSize(Direction direction)
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