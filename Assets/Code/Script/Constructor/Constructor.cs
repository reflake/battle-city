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
		[SerializeField] Tilemap brickLayer;
		[SerializeField] Tilemap concreteLayer;
		[SerializeField] Tilemap topLayer;
		[SerializeField] Tilemap bottomLayer;
		[SerializeField] GameObject blocksPrefab;

		[Inject] readonly BattleField _battleField;
		
		bool _holdPaint = false;
		bool samePlace = false;
		int blockCycle = 0;
		Block[] _blocks;
		ConstructorControls _controls;
		TileBase[] tiles = new TileBase[16];
		Dictionary<Vector2Int, SaveDataBlock> _savedDataBlocks = new();

		void Awake()
		{
			_controls = new ConstructorControls();
			
			_controls.Enable();

			MoveCursorInput(_controls.Cursor.Move, 400, 180);
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
					var newPosition = transform.position + (Vector3)moveDirection;

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
			inputAction.started += (_) =>
			{
				// Cycle through all types of blocks
				if (samePlace)
					blockCycle = (blockCycle + 1) % _blocks.Length;
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

			{
				PlaceNormalBlock(fillBlock, cursorLocation);
				SerializeBlock(fillBlock, cursorLocation);
			}
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
			var nullBlocks = new TileBase[16];
			
			tilemap.SetTilesBlock(new BoundsInt(cellPosition, new Vector3Int(cellsPerUnit,cellsPerUnit,1)), nullBlocks);
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

		void SerializeBlock(Block block, Vector3 cursorLocation)
		{
			// Store to save data
			Vector2Int blockPosition = _battleField.GetCell(cursorLocation);
			
			if (block.LayerType == LayerType.Null)
			{
				_savedDataBlocks.Remove(blockPosition);
				return;
			}
			
			var cellsPerUnit = LayerPower(block.LayerType);
			int count = block.GetBlockTilesNonAlloc(tiles);

			/*int bitInformation = tiles
				.Take(count)
				.Zip(Enumerable.Range(0, count), (tile, bitIndex) => (tile, bitIndex))
				.Where(t => t.tile != null)
				.Sum(t => 1 << t.bitIndex);*/

			/*_savedDataBlocks[blockPosition] = new SaveDataBlock
			{
				blockName = block.Name,
				blockPositionX = blockPosition.x,
				blockPositionY = blockPosition.y,
				blockPower = cellsPerUnit,
				bitInformation = bitInformation,
				layerType = block.LayerType
			};*/

			_savedDataBlocks[blockPosition] = new SaveDataBlock
			{
				blockIndex = _blocks.IndexOf(block),
				blockPositionX = blockPosition.x,
				blockPositionY = blockPosition.y,
			};
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawCube(transform.position, Vector3.one);
		}

		public SaveDataBlock[] GetSerializedData() => _savedDataBlocks.Values.ToArray();

		public void LoadLevel(SaveDataBlock[] data)
		{
			foreach (var layerType in Enum.GetValues(typeof(LayerType))
				.Cast<LayerType>())
			{
				if (layerType == LayerType.Null)
					continue;
				
				var tileLayer = LayerTilemapFromType(layerType);
				
				tileLayer.ClearAllTiles();
			}

			_savedDataBlocks = data.ToDictionary(x => new Vector2Int(x.blockPositionX, x.blockPositionY));

			foreach (var block in data)
			{
				PlaceNormalBlock(
					_blocks[block.blockIndex], 
					new Vector3(block.blockPositionX, block.blockPositionY) + Vector3.one * .5f);
			}
		}
	}
}