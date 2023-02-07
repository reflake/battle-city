using System;

using UnityEngine;
using Zenject;

public class Base : MonoBehaviour, IDestructible
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Sprite destroyedSprite;
	[SerializeField] private Collider2D collider;

	[Inject] private readonly GameManager _gameManager = null;
	
	public bool Alive { get; private set; } = true;
	
	public void Kill()
	{
		Alive = false;

		spriteRenderer.sprite = destroyedSprite;

		collider.enabled = false;
		
		_gameManager.GameOver();
	}
}
