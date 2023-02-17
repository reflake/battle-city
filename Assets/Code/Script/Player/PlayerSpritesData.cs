using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Tank Sprites", menuName = "Tank/Tank Sprites", order = 0)]
public class PlayerSpritesData : ScriptableObject, ITankSprites
{
	[SerializeField] Sprite normalSprite;

	public Sprite NormalSprite => normalSprite;
	public Sprite PoweredSprite => throw new Exception("No powered sprite for player!");
}