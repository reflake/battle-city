using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UI;
using UI.MessageBox;
using UI.MessageBox.Generic;
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

		public Builder<T> MessageBoxBuilder<T>(string prefabPath, int tier) where T : Enum
		{
			void Show(MessageBox<T> messageBoxInstance, AnswerDelegate answerDelegate)
			{
				var panel = CreatePanel<MessageBoxPanel>(prefabPath, tier);

				panel.Setup(messageBoxInstance, answerDelegate);
			}
			
			var builder = new Builder<T>(Show);

			return builder;
		}
	}
}