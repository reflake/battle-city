using UnityEngine;
using Zenject;

public partial class Player
{
	public class Factory : PlaceholderFactory<PlayerSpritesData, Player>
	{
	}
}