using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Tank Data", menuName = "Tank/Enemy Tank Data", order = 0)]
public class EnemyData : ScriptableObject
{
	public EnemySpritesData spritesData;
	public Stats stats;
}