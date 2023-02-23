using UnityEngine;
using Zenject;

namespace Enemies
{
	public partial class EnemyAI
	{
		public class Factory : PlaceholderFactory<EnemyData, Vector2, EnemyAI>
		{
			public override EnemyAI Create(EnemyData enemyData, Vector2 spawnPosition)
			{
				var enemy = base.Create(enemyData, spawnPosition);

				enemy.gameObject.SetActive(false);
				
				return enemy;
			}
		}
	}
}