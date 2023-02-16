using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleLayer : MonoBehaviour, IDestructible
{
	[SerializeField] private Tilemap _tilemap = null;
	[SerializeField] private int durability = 1;
	[SerializeField] private int blocksPerUnit = 4;
	
	public bool Alive => true;


	public void TakeDamage(DamageData damageData)
	{
		if (damageData.damage < durability)
			return;
		
	}
}