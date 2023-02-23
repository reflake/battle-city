using UnityEngine;
using Zenject;

namespace Effects
{
	public partial class Effect
	{
		public class Factory : PlaceholderFactory<Vector2, AnimationData, Effect>
		{
			
		}
	}
}