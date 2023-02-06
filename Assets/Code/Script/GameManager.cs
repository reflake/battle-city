using UnityEngine;

public class GameManager : MonoBehaviour
{
	public event GameOverDelegate OnGameOver;
	
	public void GameOver()
	{
		OnGameOver.Invoke();
	}
}