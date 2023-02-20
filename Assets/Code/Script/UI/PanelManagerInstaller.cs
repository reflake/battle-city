using UI;
using Zenject;

namespace UI
{
	public class PanelManagerInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<IPanelFactory>().To<PanelFactory>().AsSingle();
		}
	}
}