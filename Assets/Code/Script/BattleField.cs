using UnityEngine;

public class BattleField : MonoBehaviour
{
	[SerializeField] private Grid grid;
	[SerializeField] private Bounds bounds;

	public Bounds Bounds => bounds;

	private void OnDrawGizmosSelected()
	{
		if (grid)
		{
			Gizmos.color = Color.yellow;
			
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}
	}
}