using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Sprite destroyedSprite;

	private bool _alive = true;
	
	public void Kill()
	{
		if (_alive)
		{
			_alive = false;

			spriteRenderer.sprite = destroyedSprite;

			// TODO: Tell game manager that game is over
			throw new NotImplementedException();
		}
	}
}
