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
		if (Alive && damageData.damage >= durability)
		{
			Vector2 impactPosition = damageData.position;
			Vector2 damageSize = GetNormalDamageSize(damageData.damage, damageData.direction);
			Bounds damageBounds = new Bounds(impactPosition, damageSize);

			var snappedCenter = damageBounds.center;
			
			snappedCenter *= 4;
			snappedCenter.x = Mathf.Round(snappedCenter.x);
			snappedCenter.y = Mathf.Round(snappedCenter.y);
			snappedCenter /= 4;

			damageBounds.center = snappedCenter;

			var hits = Physics2D.OverlapBoxAll(damageBounds.center, damageBounds.size, 0f,
				LayerMask.GetMask("Default"));

#if UNITY_EDITOR
			// DebugGizmos.Instance?.DrawBox(damageBounds);
#endif

			foreach (var collider in hits.Where(x => x.CompareTag("Destructible")))
			{
				if (collider.TryGetComponent<DestructibleObject>(out var otherDestructibleObject))
				{
					otherDestructibleObject.InnerTakeDamage();
				}
			}
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
	
	Vector2 GetNormalDamageSize(int damage, Direction direction)
	{
		const float widthEpsilon = 0.175f;
		
		switch (direction)
		{
			case Direction.North:
			case Direction.South:
				return new Vector2(1f - widthEpsilon, .24f * damage);
			case Direction.East:
			case Direction.West:
				return new Vector2(.24f * damage, 1f - widthEpsilon);
		}

		throw new Exception("Unexpected behaviour!");
	}
}