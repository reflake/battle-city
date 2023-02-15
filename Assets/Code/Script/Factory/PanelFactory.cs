using UnityEngine;

using Zenject;

public class PanelFactory : IPanelFactory
{
	[Inject] DiContainer _container;

	public T Create<T>(string prefabPath, Transform parentTransform) where T : Object
	{
		var gameObject = Resources.Load<T>(prefabPath);

		return _container.InstantiatePrefabForComponent<T>(gameObject, parentTransform);
	}
}
