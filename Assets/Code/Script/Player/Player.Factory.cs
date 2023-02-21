using Zenject;

namespace Players
{
	public partial class Player
	{
		public class Factory : PlaceholderFactory<PlayerSpritesData, Player>
		{
			public override Player Create(PlayerSpritesData param)
			{
				var player = base.Create(param);

				player.gameObject.SetActive(false);
				
				return player;
			}
		}
	}
}