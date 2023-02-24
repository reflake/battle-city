using System;
using System.Collections;
using Players;
using Tanks;
using UnityEngine;

namespace Gameplay
{
	public class PowerUp : MonoBehaviour
	{
		[SerializeField] SpriteRenderer _spriteRenderer;
		
		bool _picked = false;

		void Awake()
		{
			const float period = .5f;
			
			InvokeRepeating("Flickering", 0f, period);
		}

		void Flickering()
		{
			_spriteRenderer.enabled = !_spriteRenderer.enabled;
		}

		void PickupByPlayer(Player player)
		{
			if (_picked)
				return;
		
			player.UpgradeTank();

			_picked = true;
		
			Destroy(gameObject);
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<Player>(out var player))
			{
				PickupByPlayer(player);
			}
		}
	}
}