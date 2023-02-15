using System.Collections.Generic;
using UnityEngine;
using Zenject;

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
			
			_tiersTransforms[tier] = newTierTransform.transform;
		}
		
		return _panelFactory.Create<T>(prefabPath, _tiersTransforms[tier]);
	}
}