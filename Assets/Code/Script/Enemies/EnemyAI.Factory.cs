using UnityEngine;
using Zenject;

namespace Enemies
{
	public partial class EnemyAI
	{
		public class Factory : PlaceholderFactory<EnemyData, Vector2, EnemyAI> {}
	}
}