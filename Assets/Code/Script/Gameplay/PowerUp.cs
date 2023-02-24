using System;
using Players;
using Tanks;
using UnityEngine;

namespace Gameplay
{
	public class PowerUp : MonoBehaviour
	{
		bool _picked = false;
	
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