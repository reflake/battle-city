using System.Linq;
using UnityEngine;
using Zenject;

namespace Gameplay
{
	public class PowerUpManager : MonoBehaviour
	{
		[Inject] readonly BattleField _battleField;

		[SerializeField] int[] powerUpAppearanceOrder;
		[SerializeField] PowerUp powerUpPrefab;
	
		public bool IsEnemyDropsPowerUp(int enemyNumber)
		{
			return powerUpAppearanceOrder.Contains(enemyNumber);
		}

		public void SpawnPowerUp()
		{
			const float padding = 1f;
			Bounds spawnBounds = _battleField.Bounds;
			spawnBounds.Expand(-padding);

			var min = spawnBounds.min;
			var max = spawnBounds.max;
		
			float x = Random.Range(min.x, max.x);
			float y = Random.Range(min.y, max.y);

			Instantiate(powerUpPrefab, new Vector3(x, y, -1f), Quaternion.identity);
		}
	}
}