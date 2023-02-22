using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace LevelDesigner
{
	public class LevelListDialog : MonoBehaviour
	{
		public static string prefabPath => "LevelListDialog";
		
		[Inject] readonly CustomLevelList _customLevelList = null;

		[SerializeField] TMP_Text _header;
		[SerializeField] LevelItem _itemPrefab;
		[SerializeField] Color _evenItemColor;
		[SerializeField] Color _oddItemColor;
		[SerializeField] Transform _contentHolder;
		[SerializeField] SaveNewButton _saveNewButton;
		[SerializeField] GameObject _stubListItem;
		
		List<LevelItem> _items;
		bool _submitFile = false;
		string _selectedFileName = string.Empty;

		void OnEnable()
		{
			int index = 0;

			LevelItem CreateItem(string name)
			{
				var item = Instantiate(_itemPrefab, _contentHolder);

				item.Name = name;
				item.SetBackgroundColor(index++ % 2 == 0 ? _evenItemColor : _oddItemColor);
				item.ClickCallback = () =>
				{
					_submitFile = true;
					_selectedFileName = name;
				};
			
				return item;
			}
			
			_items = _customLevelList.LevelNames
				.Select(CreateItem)
				.ToList();

			_saveNewButton.OnSubmit += SubmitSaveNewFile;
		}

		void OnDisable()
		{
			foreach (var item in _items)
			{
				Destroy(item.gameObject);
			}

			_items = null;

			_saveNewButton.OnSubmit -= SubmitSaveNewFile;
		}

		void SubmitSaveNewFile(string fileName)
		{
			_submitFile = true;
			_selectedFileName = fileName;
		}

		public async UniTask GetSaveFile(LevelData levelData)
		{
			_saveNewButton.gameObject.SetActive(true);
			
			// activate stub so it takes up some space
			_stubListItem.SetActive(true);
			
			gameObject.SetActive(true);
			
			_header.text = "Save file";

			await UniTask.WaitUntil(() => _submitFile);
			await _customLevelList.WriteLevel(_selectedFileName, levelData, this.GetCancellationTokenOnDestroy());
			
			gameObject.SetActive(false);
		}
		
		public async UniTask<LevelData> GetLoadFile()
		{
			_saveNewButton.gameObject.SetActive(false);
			
			_stubListItem.SetActive(false);
			
			gameObject.SetActive(true);
			
			_header.text = "Load file";

			await UniTask.WaitUntil(() => _submitFile);
			var data = await _customLevelList.ReadLevel(_selectedFileName, this.GetCancellationTokenOnDestroy());
			
			gameObject.SetActive(false);

			return data;
		}
	}
}