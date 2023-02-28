using Common;
using UnityEngine;
using Zenject;

namespace Tanks
{
	public class TankInstaller : MonoInstaller
	{
		public Bullet bulletPrefab;
		
		public override void InstallBindings()
		{
			Container.Bind<Tank>().FromComponentSibling();
			Container.Bind<Rigidbody2D>().FromComponentSibling();
			Container.Bind<Collider2D>().FromComponentSibling();
			Container.Bind<SpriteRenderer>().FromComponentSibling();
			Container.BindFactory<Team, Vector3, Direction, Stats, Collider2D, Bullet, Bullet.Factory>()
				.FromMonoPoolableMemoryPool(x => x.WithInitialSize(1)
					.FromComponentInNewPrefab(bulletPrefab));
		}
	}
}
