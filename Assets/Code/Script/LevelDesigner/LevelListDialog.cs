using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LevelDesigner
{
	public class LevelListDialog : MonoBehaviour
	{
		public static string prefabPath => "LevelListDialog";
		
		[Inject] readonly CustomLevelList _customLevelList = null;
		[Inject] readonly PanelManager _panelManager = null;

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
		bool _savingFile = false;
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
			UpdateList(_customLevelList.LevelNames);

			_customLevelList.OnListUpdated += UpdateList;
		}

		void UpdateList(IReadOnlyList<string> listOfLevels)
		{
			Clear();
			
			int index = 0;

			LevelItem CreateItem(string fileName)
			{
				var item = Instantiate(_itemPrefab, _contentHolder);

				item.Name = fileName;
				item.SetBackgroundColor(index++ % 2 == 0 ? _evenItemColor : _oddItemColor);
				item.SelectedCallback = () =>
				{
					if (_savingFile)
					{
						SubmitSaveNewFile(fileName);
					}
					else
					{
						SubmitLoad(fileName);
					}
				};
				item.DeleteCallback = () => AskForDeletion(fileName);

				return item;
			}

			_items = listOfLevels
				.Select(CreateItem)
				.ToList();
		}

		void OnDisable()
		{
			Clear();

			_cts.Cancel();

			_customLevelList.OnListUpdated -= UpdateList;
		}

		void Clear()
		{
			if (_items != null)
			{
				foreach (var item in _items)
				{
					Destroy(item.gameObject);
				}

				_items = null;
			}
		}

		void Open()
		{
			gameObject.SetActive(true);
		}
		
		void Close()
		{
			gameObject.SetActive(false);
		}
		
		async void SubmitSaveNewFile(string fileName)
		{
			if (_customLevelList.LevelNames.Contains(fileName))
			{
				var permissionGranted = await AskForPermissionToOverwrite(fileName);

				if (!permissionGranted)
				{
					return;
				}
			}
			_submitFile = true;
			_selectedFileName = fileName;
		}

		void SubmitLoad(string fileName)
		{
			_submitFile = true;
			_selectedFileName = fileName;
		}

		async UniTask<bool> AskForPermissionToOverwrite(string fileName)
		{
			var builder = _panelManager.MessageBoxBuilder<AcceptOptions>("MessageBox", 3);

			var messageBox = builder
				.Description($"File {fileName} already exists. Do you want to overwrite it?")
				.AddOption(AcceptOptions.OK, "Overwrite")
				.AddOption(AcceptOptions.Cancel, "Cancel")
				.DefaultOptions(AcceptOptions.Cancel)
				.Build();

			return (await messageBox.AsyncShow()) == AcceptOptions.OK;
		}

		async void AskForDeletion(string fileName)
		{
			var permissionGranted = await AskForPermissionToDelete(fileName);

			if (permissionGranted)
			{
				_customLevelList.DeleteLevel(fileName);
			}
		}

		async UniTask<bool> AskForPermissionToDelete(string fileName)
		{
			var builder = _panelManager.MessageBoxBuilder<AcceptOptions>("MessageBox", 3);

			var messageBox = builder
				.Description($"Do you really want to delete file {fileName}?")
				.AddOption(AcceptOptions.OK, "Delete")
				.AddOption(AcceptOptions.Cancel, "Cancel")
				.DefaultOptions(AcceptOptions.Cancel)
				.Build();

			return (await messageBox.AsyncShow()) == AcceptOptions.OK;
		}

		public async UniTask GetSaveFile(LevelData levelData)
		{
			_header.text = "Save file";
			
			_cts = new CancellationTokenSource();
			
			SetSaveNewButton(true);
			Open();

			await UniTask.WaitUntil(() => _submitFile, cancellationToken: _cts.Token);
			await _customLevelList.WriteLevel(_selectedFileName, levelData, _cts.Token);
			
			gameObject.SetActive(false);
		}

		public async UniTask<LevelData> GetLoadFile()
		{
			_header.text = "Load file";
			
			_cts = new CancellationTokenSource();
			
			SetSaveNewButton(false);
			Open();

			await UniTask.WaitUntil(() => _submitFile, cancellationToken: _cts.Token);
			var data = await _customLevelList.ReadLevel(_selectedFileName, _cts.Token);
			
			Close();

			return data;
		}

		void SetSaveNewButton(bool visible)
		{
			_savingFile = visible;
			_saveNewButton.gameObject.SetActive(visible);
			_stubListItem.SetActive(visible);
		}
	}
}