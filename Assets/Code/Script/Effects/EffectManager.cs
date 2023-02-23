using UnityEngine;
using Zenject;

namespace Effects
{
	public class EffectManager : MonoBehaviour
	{
		[Inject] Effect.Factory _factory = default;

		public Effect CreateEffect(Vector2 spawnPosition, AnimationData animationData)
		{
			return _factory.Create(spawnPosition, animationData);
		}
	}
}