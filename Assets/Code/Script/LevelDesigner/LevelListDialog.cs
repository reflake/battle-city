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
		
		List<LevelItem> _items;
		bool _fileSelected = false;
		string _selectedFile = string.Empty;

		void Start()
		{
			int index = 0;

			LevelItem CreateItem(string name)
			{
				var item = Instantiate(_itemPrefab, _contentHolder);

				item.Name = name;
				item.SetBackgroundColor(index++ % 2 == 0 ? _evenItemColor : _oddItemColor);
				item.ClickCallback = () =>
				{
					_fileSelected = true;
					_selectedFile = name;
				};
			
				return item;
			}
			
			_items = _customLevelList.LevelNames
				.Select(CreateItem)
				.ToList();
		}

		public async UniTask<string> GetSaveFile(LevelData levelData)
		{
			_header.text = "Save file";

			throw new NotImplementedException();

			// await _customLevelList.WriteLevel(fileName, data, this.GetCancellationTokenOnDestroy());
		}
		
		public async UniTask<LevelData> GetLoadFile()
		{
			_header.text = "Load file";

			await UniTask.WaitUntil(() => _fileSelected);
			var data = await _customLevelList.ReadLevel(_selectedFile, this.GetCancellationTokenOnDestroy());
			
			Destroy(this.gameObject);

			return data;
		}
	}
}