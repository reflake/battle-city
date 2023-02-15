using UnityEngine;
using Zenject;

public partial class EnemyAI
{
	public class Factory : PlaceholderFactory<Vector2, EnemyAI> {}
}