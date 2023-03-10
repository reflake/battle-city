using UnityEngine;
using Zenject;

namespace Enemies
{
	public class EnemyManagerInstaller : MonoInstaller
	{
		public EnemyManager enemyManager;
		public EnemyAI enemyTankPrefab;

		public override void InstallBindings()
		{
			int maximumEnemiesOnScreen = enemyManager.MaximumEnemiesOnScreen;

			Container.BindInstance(enemyManager);
			Container.BindFactory<EnemyData, Vector2, EnemyAI, EnemyAI.Factory>()
				.FromMonoPoolableMemoryPool(x => x.WithInitialSize(maximumEnemiesOnScreen)
					.FromComponentInNewPrefab(enemyTankPrefab));
		}
	}
}