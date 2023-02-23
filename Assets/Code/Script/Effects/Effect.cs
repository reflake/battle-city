using System;
using UnityEngine;
using Zenject;

namespace Effects
{
	public partial class Effect : MonoBehaviour
	{
		[SerializeField] SpriteRenderer _spriteRenderer = default;
		
		AnimationData _animationData = default;

		float _timer = 0f;
		
		void Update()
		{
			float animationPhase = _timer / _animationData.period;
			int frameIndex = Mathf.FloorToInt(animationPhase) % _animationData.sprites.Length;

			_spriteRenderer.sprite = _animationData.sprites[frameIndex];

			if (_animationData.duration <= _timer)
			{
				Dispose();
			}
			
			_timer += Time.deltaTime;
		}
	}
}