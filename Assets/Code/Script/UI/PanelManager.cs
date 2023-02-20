﻿using System.Collections.Generic;
using UI;
using UnityEngine;
using Zenject;

namespace UI
{
	public class PanelManager : MonoBehaviour
	{
		[SerializeField] Transform tierPrefab;
	
		[Inject] IPanelFactory _panelFactory;

		Dictionary<int, Transform> _tiersTransforms = new();

		public T CreatePanel<T>(string prefabPath, int tier) where T : Object
		{
			prefabPath = $"Prefabs/{prefabPath}";
		
			if (!_tiersTransforms.ContainsKey(tier))
			{
				var newTierTransform = Instantiate(tierPrefab, transform);

				newTierTransform.name = $"Tier {tier}";
				newTierTransform.SetSiblingIndex(tier);
			
				_tiersTransforms[tier] = newTierTransform.transform;
			}
		
			return _panelFactory.Create<T>(prefabPath, _tiersTransforms[tier]);
		}
	}
}