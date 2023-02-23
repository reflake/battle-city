using Zenject;

namespace Project
{
	public class ProjectInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<CustomLevelContext>().AsSingle();
		}
	}
}