using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleLayer : MonoBehaviour, IDestructible
{
	[SerializeField] private Tilemap _tilemap = null;
	
	public bool Alive => true;

	private Vector2 damageSize = Vector2.one * .5f;

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
		if (tile != null)
		{
			_tilemap.SetTile(position, null);
		}
	}
}