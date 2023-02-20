using Zenject;

namespace Players
{
	public partial class Player
	{
		public class Factory : PlaceholderFactory<PlayerSpritesData, Player>
		{
		}
	}
}