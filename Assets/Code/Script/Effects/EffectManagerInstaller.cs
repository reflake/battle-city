using UnityEngine;
using Zenject;

namespace Effects
{
	public class EffectManagerInstaller : MonoInstaller
	{
		public Effect effectPrefab;
		
		public override void InstallBindings()
		{
			Container.BindFactory<Vector2, AnimationData, Effect, Effect.Factory>()
				.FromMonoPoolableMemoryPool(x => x.WithInitialSize(40)
					.FromComponentInNewPrefab(effectPrefab));
		}
	}
}