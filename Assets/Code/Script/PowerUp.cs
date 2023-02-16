using UnityEngine;

public class PowerUp : MonoBehaviour
{
	public void PickupByPlayer(Player player)
	{
		player.UpgradeTank();
	}
}