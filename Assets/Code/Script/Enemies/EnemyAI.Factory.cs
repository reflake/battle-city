using UnityEngine;
using Zenject;

namespace Enemies
{
	public partial class EnemyAI
	{
		public class Factory : PlaceholderFactory<EnemyData, Vector2, EnemyAI>
		{
			public override EnemyAI Create(EnemyData param1, Vector2 param2)
			{
				var enemy = base.Create(param1, param2);

				enemy.gameObject.SetActive(false);
				
				return enemy;
			}
		}
	}
}