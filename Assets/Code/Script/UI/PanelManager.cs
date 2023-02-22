using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace UI
{
	public class PanelManager : MonoBehaviour
	{
		[SerializeField] Transform tierPrefab;
	
		[Inject] IPanelFactory _panelFactory;

		Dictionary<int, Transform> _tiersTransforms = new();
		Dictionary<string, object> _panels = new();

		public T CreatePanel<T>(string prefabPath, int tier, string id = "") where T : Object
		{
			prefabPath = $"Prefabs/{prefabPath}";
		
			if (!_tiersTransforms.ContainsKey(tier))
			{
				var newTierTransform = Instantiate(tierPrefab, transform);

				newTierTransform.name = $"Tier {tier}";
				newTierTransform.SetSiblingIndex(tier);
			
				_tiersTransforms[tier] = newTierTransform.transform;
			}

			var panel = _panelFactory.Create<T>(prefabPath, _tiersTransforms[tier]);
			
			if (string.IsNullOrEmpty(id))
			{
				_panels[id] = panel;
			}
			
			return panel;
		}

		public T UsePanel<T>(string prefabPath, int tier, string id) where T : Object
		{
			// TODO: check if prefab and tier are same
			
			if (_panels.ContainsKey(id))
			{
				return _panels[id] as T;
			}

			return CreatePanel<T>(prefabPath, tier);
		}
	}
}