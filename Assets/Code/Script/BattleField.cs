using UnityEngine;

public class BattleField : MonoBehaviour
{
	[SerializeField] private Grid grid;
	[SerializeField] private BoundsInt bounds;

	private void OnDrawGizmosSelected()
	{
		if (grid)
		{
			Gizmos.color = Color.yellow;

			var min = grid.CellToWorld(bounds.min);
			var max = grid.CellToWorld(bounds.max);
			var size = max - min;
			
			Gizmos.DrawWireCube(min + size / 2, size);
		}
	}
}