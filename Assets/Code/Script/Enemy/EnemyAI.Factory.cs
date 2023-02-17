using UnityEngine;
using Zenject;

public partial class EnemyAI
{
	public class Factory : PlaceholderFactory<EnemyData, Vector2, EnemyAI> {}
}