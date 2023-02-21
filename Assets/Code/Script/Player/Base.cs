using Common;
using Gameplay;
using UnityEngine;
using Zenject;

namespace Players
{
	public class Base : MonoBehaviour, IDestructible
	{
		[SerializeField] SpriteRenderer spriteRenderer;
		[SerializeField] Sprite destroyedSprite;
		[SerializeField] Collider2D collider;

		// Event
		public event BaseDestroyDelegate OnBaseDestroy;
		
		public bool Alive { get; private set; } = true;

		public void TakeDamage(DamageData _)
		{
			Alive = false;

			spriteRenderer.sprite = destroyedSprite;

			collider.enabled = false;
		
			OnBaseDestroy?.Invoke();
		}
	}
}
