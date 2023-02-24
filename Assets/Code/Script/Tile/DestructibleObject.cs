using System;
using System.Linq;
using Common;
using Gameplay;
using UnityEngine;

namespace Tiles
{
	public class DestructibleObject : MonoBehaviour, IDestructible
	{
		[SerializeField] int hp;
		[SerializeField] int durability;

		public bool Alive { get; private set; } = true;

		bool set = false;
		Vector3Int _position;

		public void Setup(Vector3Int position)
		{
			if (set)
				throw new Exception("Already set!");
		
			set = true;
			_position = position;
		}

		public void TakeDamage(DamageData damageData)
		{
			const float half = 1 / 2f;
			float penetrationAbility = damageData.firePower;
			Vector2 impactPosition = damageData.position;
			Bounds damageBounds = new Bounds();
			Vector2 penetrationForward = damageData.directionVector;
			Vector2 penetrationSide = Vector2.Perpendicular(penetrationForward);
			
			// Create hit box
			damageBounds.Encapsulate(penetrationForward * half * penetrationAbility);
			damageBounds.Encapsulate(penetrationSide);
			
			damageBounds.center = impactPosition;

			const float errorMargin = -.1f;
			
			damageBounds.Expand(errorMargin);
			
			// Snap hitbox to grid
			Vector2 snappedCenter = BattleField.Instance.Tilemap.Snap(damageBounds.center);

			damageBounds.center = snappedCenter;

			// Raycast closest tiles
			var hits = Physics2D.OverlapBoxAll(damageBounds.center, damageBounds.size, 0f,
				LayerMask.GetMask("Default"));

#if UNITY_EDITOR
			DebugGizmos.Instance?.DrawBox(damageBounds);
#endif

			foreach (var collider in hits.Where(x => x.CompareTag("Destructible")))
			{
				if (collider.TryGetComponent<DestructibleObject>(out var otherDestructibleObject))
				{
					otherDestructibleObject.InnerTakeDamage(damageData.damage);
				}
			}
		}

		void InnerTakeDamage(int damage)
		{
			if (!Alive || damage < durability)
				return;
			
			hp--;

			if (hp <= 0)
			{
				Alive = false;

				BattleField.Instance.Tilemap.SetTile(_position, null);
			}
		}
	}
}