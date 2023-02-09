using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleTile : Tile
{
	[SerializeField] private bool _canTakeDamage;

	public bool CanTakeDamage => _canTakeDamage;
}