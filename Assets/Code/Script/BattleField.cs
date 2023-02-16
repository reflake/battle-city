using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleField : MonoBehaviour
{
	public static BattleField Instance { get; private set; } = null;

	[SerializeField] Tilemap tilemap;
	[SerializeField] private Grid grid;
	[SerializeField] private Bounds bounds;

	public Bounds Bounds => bounds;
	public Tilemap Tilemap => tilemap;

	private void Awake()
	{
		var padding = 1f;
		var width = new Vector2(bounds.size.x + padding * 2f, padding);
		var height = new Vector2(padding, bounds.size.y);
		
		// Some magic
		CreateBox(bounds.center + (padding * .5f + bounds.extents.y) * Vector3.up, width);
		CreateBox(bounds.center + (padding * .5f + bounds.extents.y) * Vector3.down, width);
		CreateBox(bounds.center + (padding * .5f + bounds.extents.x) * Vector3.left, height);
		CreateBox(bounds.center + (padding * .5f + bounds.extents.x) * Vector3.right, height);

		Instance = this;
	}

	void OnDestroy()
	{
		Instance = null;
	}

	private void CreateBox(Vector2 offset, Vector2 size)
	{
		var box = gameObject.AddComponent<BoxCollider2D>();

		box.offset = offset - (Vector2)transform.position;
		box.size = size;
	}

	private void OnDrawGizmosSelected()
	{
		if (grid)
		{
			Gizmos.color = Color.yellow;
			
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}
	}

	public Vector2Int GetCell(Vector3 location)
	{
		return (Vector2Int)grid.WorldToCell(location);
	}
}