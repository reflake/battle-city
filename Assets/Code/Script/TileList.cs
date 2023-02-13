using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Tile Data", menuName = "Create Tile Data", order = 0)]
public class TileList : ScriptableObject
{
	public TileBase[] tiles;
}