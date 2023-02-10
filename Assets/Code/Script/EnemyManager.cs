using UnityEngine;

using Zenject;

public class EnemyManager : MonoBehaviour
{
	[SerializeField] private int enemiesLeft = 4;
	
	[Inject] private readonly GameManager _gameManager = null;

	public void EnemyKilled(EnemyAI killedEnemy)
	{
		enemiesLeft--;

		if (enemiesLeft == 0)
		{
			_gameManager.LevelComplete();
		}
	}
}