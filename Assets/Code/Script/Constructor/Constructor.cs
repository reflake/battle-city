using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ModestTree;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Zenject;

namespace LevelDesigner
{
	public class Constructor : MonoBehaviour
	{
		[SerializeField] SpriteRenderer cursorSprite;
		
		[Space]
		
		[SerializeField] Tilemap brickLayer;
		[SerializeField] Tilemap concreteLayer;
		[SerializeField] Tilemap topLayer;
		[SerializeField] Tilemap bottomLayer;
		[SerializeField] GameObject blocksPrefab;
		
		[Space]
		
		[SerializeField] TileList tileData;

		[Inject] readonly BattleField _battleField;
		
		bool _holdPaint = false;
		bool samePlace = false;
		int blockCycle = 0;
		Block[] _blocks;
		ConstructorControls _controls;
		TileBase[] tiles = new TileBase[16];

		void Awake()
		{
			_controls = new ConstructorControls();
			
			_controls.Enable();

			MoveCursorInput(_controls.Cursor.Move, 400, 150);
			PaintBlockInput(_controls.Cursor.Paint);
			
			_blocks = blocksPrefab.GetComponentsInChildren<Block>();

			cursorSprite.DOFade(0f, 0.66f)
				// Flicker Easing
				.SetEase((time, duration, _, __) => time > duration * .5f ? 1f : 0f)
				.SetLoops(-1);
		}

		void MoveCursorInput(InputAction inputAction, int holdDelay, int holdInterval)
		{
			Vector2 moveDirection = Vector2.zero;
			CancellationTokenSource cts = null;
			CancellationToken token;

			async UniTaskVoid MoveCursorCycle(CancellationToken token)
			{
				bool needHold = true;

				await UniTask.Yield();

				while (!token.IsCancellationRequested)
				{
					samePlace = false;
					Vector2 newPosition = transform.position + (Vector3)moveDirection;

					// Restrict cursor bounds
					var boundsWithoutPadding = _battleField.Bounds;
					boundsWithoutPadding.Expand(-1f);
					
					newPosition = boundsWithoutPadding.ClosestPoint(newPosition);
					
					// Move to new position
					transform.position = newPosition;

					OnCursorMoved();

					await UniTask.Delay(needHold ? holdDelay : holdInterval, DelayType.Realtime);

					needHold = false;
				}
			}

			inputAction.started += (ctx) =>
			{
				cts = new CancellationTokenSource();
				token = cts.Token;
				moveDirection = ctx.ReadValue<Vector2>();

				Task.Run(() => MoveCursorCycle(token), token);
			};
			inputAction.performed += (ctx) => moveDirection = ctx.ReadValue<Vector2>();
			inputAction.canceled += (_) => cts.Cancel();
		}

		void PaintBlockInput(InputAction inputAction)
		{
			inputAction.performed += (_) =>
			{
				// Cycle through all types of blocks
				if (samePlace)
					blockCycle = (int)Mathf.Repeat(blockCycle + _.ReadValue<float>(), _blocks.Length);
				else
					samePlace = true;

				_holdPaint = true;
				
				PaintBlock();
			};
			
			inputAction.canceled += (_) => _holdPaint = false;
		}
		
		void OnCursorMoved()
		{
			if (_holdPaint)

				PaintBlock();
		}
		
		void PaintBlock()
		{
			Vector3 brushOffset = new(-0.45f, -.45f);
			Vector3 cursorLocation = transform.position + brushOffset;
			var fillBlock = _blocks[blockCycle];

			foreach (var layerType in Enum.GetValues(typeof(LayerType))
											.Cast<LayerType>())
			{
				if (layerType == LayerType.Null)
					continue;
				
				var tileLayer = LayerTilemapFromType(layerType);
				int cellsPerUnit = LayerPower(layerType);
				
				ClearLayer(cellsPerUnit, tileLayer, cursorLocation);
			}

			PlaceNormalBlock(fillBlock, cursorLocation);
		}

		Tilemap LayerTilemapFromType(LayerType layerType)
		{
			return layerType switch
			{
				LayerType.Brick => brickLayer,
				LayerType.Concrete => concreteLayer,
				LayerType.Top => topLayer,
				LayerType.Bottom => bottomLayer,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		int LayerPower(LayerType layerType)
		{
			return layerType switch
			{
				LayerType.Brick => 4,
				LayerType.Concrete => 2,
				LayerType.Top => 2,
				LayerType.Bottom => 2,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		void ClearLayer(int cellsPerUnit, Tilemap tilemap, Vector3 cursorLocation)
		{
			var cellPosition = tilemap.WorldToCell(cursorLocation);
			var nullBlocks = new TileBase[cellsPerUnit * cellsPerUnit];
			var blockBounds = new BoundsInt(cellPosition, new Vector3Int(cellsPerUnit,cellsPerUnit,1));
			
			tilemap.SetTilesBlock(blockBounds, nullBlocks);
		}

		void PlaceNormalBlock(Block block, Vector3 cursorLocation)
		{
			if (block.LayerType == LayerType.Null)
				return;
			
			var layerTilemap = LayerTilemapFromType(block.LayerType);
			var cellsPerUnit = LayerPower(block.LayerType);
			int count = block.GetBlockTilesNonAlloc(tiles);

			if (count > cellsPerUnit * cellsPerUnit)
				throw new Exception($"Tiles amount must be less than {cellsPerUnit * cellsPerUnit}");

			// TODO: better explanation
			var blockBounds = block.Tilemap.cellBounds;
			
			blockBounds.position = layerTilemap.WorldToCell(cursorLocation);

			layerTilemap.SetTilesBlock(blockBounds, tiles);
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawCube(transform.position, Vector3.one);
		}

		public SaveData GetSerializedData()
		{
			var layerTilemaps = Enum.GetValues(typeof(LayerType))
				.Cast<LayerType>()
				.Where(x => x != LayerType.Null)
				.ToArray();

			// Find and write individual tile names
			var usedTiles = tileData.tiles;
			var int2name = new Dictionary<int, string>();
			var name2int = new Dictionary<string, int>();

			int2name.Add(0, "null");

			int index = 1;
				
			foreach (var tile in usedTiles)
			{
				name2int[tile.name] = index;
				int2name[index++] = tile.name;
			}
			
			var saveLayerDatas = layerTilemaps.Select(layerType =>
			{
				var layerTilemap = LayerTilemapFromType(layerType);
				
				// Prepare tilemaps for serialization
				layerTilemap.CompressBounds();

				// Find all tiles on layer and serialize as index of name
				var bounds = layerTilemap.cellBounds;
				var tilesArray = layerTilemap.GetTilesBlock(bounds);
				
				var tilesData = tilesArray
					.Select(tile => tile != null ? name2int[tile.name] : 0)
					.ToArray();

				return new SaveLayerData
				{
					layerType = layerType,
					boundsX = bounds.x,
					boundsY = bounds.y,
					boundsZ = bounds.z,
					boundsW = bounds.size.x,
					boundsH = bounds.size.y,
					boundsD = bounds.size.z,
					tilesData = tilesData
				};
			});

			return new SaveData
			{
				int2tileName = int2name,
				layers = saveLayerDatas.ToArray()
			};
		}

		public void LoadLevel(SaveData saveData)
		{
			// Unload current map
			foreach (var layerType in Enum.GetValues(typeof(LayerType))
											.Cast<LayerType>())
			{
				if (layerType == LayerType.Null)
					continue;
				
				var layerTilemap = LayerTilemapFromType(layerType);
				
				layerTilemap.ClearAllTiles();
			}
			
			// List all possibly used tiles
			var tilesByName = tileData.tiles.ToDictionary(x => x.name);

			tilesByName.Add("null", null);
			
			// Load new map
			foreach (var saveLayerData in saveData.layers)
			{
				var bounds = new BoundsInt(
					saveLayerData.boundsX, saveLayerData.boundsY, saveLayerData.boundsZ,
					saveLayerData.boundsW, saveLayerData.boundsH, saveLayerData.boundsD);

				var layerTilemap = LayerTilemapFromType(saveLayerData.layerType);
				var tiles = saveLayerData.tilesData
					.Select(integerIdentifier =>
					{
						var tileName = saveData.int2tileName[integerIdentifier];
						var tileRef = tilesByName[tileName];

						return tileRef;

					})
					.ToArray();
				
				layerTilemap.SetTilesBlock(bounds, tiles);
			}
		}
	}
}