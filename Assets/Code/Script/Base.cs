using System;

using UnityEngine;

public class Base : MonoBehaviour, IDestructible
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Sprite destroyedSprite;

	public bool Alive { get; private set; } = true;
	
	public void Kill()
	{
		Alive = false;

		spriteRenderer.sprite = destroyedSprite;

		// TODO: Tell game manager that game is over
		throw new NotImplementedException();
	}
}
