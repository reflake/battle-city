using Zenject;

public class PlayerInstaller : TankInstaller
{
	[Inject] readonly PlayerSpritesData _playerSpritesData;

	public override void InstallBindings()
	{
		Container.BindInstance(_playerSpritesData);
		
		base.InstallBindings();
	}
}