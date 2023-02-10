using UnityEngine;

using Zenject;

public class PlayerManager : MonoBehaviour
{
	[Inject] private readonly GameManager _gameManager;

	public void PlayerDefeated()
	{
		_gameManager.GameOver();
	}
}