using UnityEngine;

namespace UI
{
	public interface IPanelFactory
	{
		T Create<T>(string prefabPath, Transform parentTransform) where T : Object;
	}
}