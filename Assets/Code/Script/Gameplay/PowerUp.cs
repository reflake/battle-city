using Players;
using UnityEngine;

namespace Gameplay
{
	public class PowerUp : MonoBehaviour
	{
		bool _picked = false;
	
		public void PickupByPlayer(Player player)
		{
			if (_picked)
				return;
		
			player.UpgradeTank();

			_picked = true;
		
			Destroy(gameObject);
		}
	}
}