using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Tank Data", menuName = "Tank/Enemy Tank Data", order = 0)]
public class EnemyData : ScriptableObject
{
	public EnemySpritesData spritesData;
	public int hitPoints;
	public int fireRate;
	public int firePower;
	public int damageBonus;
	public float moveSpeed;
	public float projectileSpeed;
}