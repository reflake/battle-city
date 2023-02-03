using UnityEngine;
using Zenject;

public class TankInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		Container.Bind<Rigidbody2D>().FromComponentSibling();
		Container.Bind<Collider2D>().FromComponentSibling();
	}
}
