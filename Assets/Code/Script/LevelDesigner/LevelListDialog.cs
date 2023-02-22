using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
		[SerializeField] Button _closeButton;
		[SerializeField] Button _backgroundButton;
		
		List<LevelItem> _items;
		bool _submitFile = false;
		string _selectedFileName = string.Empty;
		CancellationTokenSource _cts = default;

		void Awake()
		{
			_saveNewButton.OnSubmit += SubmitSaveNewFile;
			_closeButton.onClick.AddListener(Close);
			_backgroundButton.onClick.AddListener(Close);
		}

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

		}

		void OnDisable()
		{
			foreach (var item in _items)
			{
				Destroy(item.gameObject);
			}

			_items = null;
			
			_cts.Cancel();
		}

		void Open()
		{
			gameObject.SetActive(true);
		}
		
		void Close()
		{
			gameObject.SetActive(false);
		}
		
		void SubmitSaveNewFile(string fileName)
		{
			_submitFile = true;
			_selectedFileName = fileName;
		}

		public async UniTask GetSaveFile(LevelData levelData)
		{
			_header.text = "Save file";
			
			_cts = new CancellationTokenSource();
			
			_saveNewButton.gameObject.SetActive(true);
			_stubListItem.SetActive(true);
			
			Open();

			await UniTask.WaitUntil(() => _submitFile, cancellationToken: _cts.Token);
			await _customLevelList.WriteLevel(_selectedFileName, levelData, _cts.Token);
			
			gameObject.SetActive(false);
		}
		
		public async UniTask<LevelData> GetLoadFile()
		{
			_header.text = "Load file";
			
			_cts = new CancellationTokenSource();
			
			_saveNewButton.gameObject.SetActive(false);
			_stubListItem.SetActive(false);
			
			Open();

			await UniTask.WaitUntil(() => _submitFile, cancellationToken: _cts.Token);
			var data = await _customLevelList.ReadLevel(_selectedFileName, _cts.Token);
			
			Close();

			return data;
		}
	}
}