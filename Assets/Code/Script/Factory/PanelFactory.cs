using System;
using UnityEngine;

using Zenject;
using Object = UnityEngine.Object;

public class PanelFactory : IPanelFactory
{
	[Inject] DiContainer _container;

	public T Create<T>(string prefabPath, Transform parentTransform) where T : Object
	{
		var gameObject = Resources.Load<T>(prefabPath);

		if (gameObject == null)

			throw new Exception($"Panel prefab not found: {prefabPath}");

		return _container.InstantiatePrefabForComponent<T>(gameObject, parentTransform);
	}
}
