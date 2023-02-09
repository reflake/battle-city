using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Terrain : MonoBehaviour, IDestructible
{
	[SerializeField] private Tilemap _tilemap = null;
	[SerializeField] private bool _hasTile = false;
	
	public bool Alive => true;

	private Vector2 damageSize = Vector2.one * .5f;
	[SerializeField] private Vector3Int _position;
	[SerializeField] private Bounds _bounds;

	public void TakeDamage(DamageData damageData)
	{
		Bounds damageBounds = new Bounds(damageData.position, damageSize);
		BoundsInt boundsInt = new BoundsInt();

		boundsInt.SetMinMax(_tilemap.WorldToCell(damageBounds.min), _tilemap.WorldToCell(damageBounds.max));

		for (int i = boundsInt.xMin; i <= boundsInt.xMax; i++)
		for (int j = boundsInt.yMin; j <= boundsInt.yMax; j++)
		{
			Vector3Int position = new Vector3Int(i, j);
			var tile = _tilemap.GetTile(position);
			
			CheckTile(position, tile);
		}
	}

	private void CheckTile(Vector3Int position, TileBase tile)
	{
		if (tile is BattleTile battleTile && (battleTile.CanTakeDamage))
		{
			_tilemap.SetTile(position, null);
		}
	}

	private void Update()
	{
		Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
	
		_position = _tilemap.WorldToCell(worldPosition);

		_hasTile = _tilemap.GetTile(_position);
	}
}