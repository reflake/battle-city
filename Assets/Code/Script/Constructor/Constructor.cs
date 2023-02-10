using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace LevelDesigner
{
	public class Constructor : MonoBehaviour
	{
		[SerializeField] Tilemap brickLayer;
		[SerializeField] GameObject blocksPrefab;

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

			MoveCursorInput(_controls.Cursor.Move, 400, 180);
			PaintBlockInput(_controls.Cursor.Paint);
			
			_blocks = blocksPrefab.GetComponentsInChildren<Block>();
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
					transform.position += (Vector3)moveDirection;

					if (_holdPaint)

						PaintBlock();

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
		
		void PaintBlock()
		{
			Vector3 brushOffset = new(-0.45f, -.45f);
			Vector3 cursorLocation = transform.position + brushOffset;
			var block = _blocks[blockCycle];

			if (block.Type != Type.Brick)
			{
				ClearLayer(brickLayer, cursorLocation);
			}
			
			if (block.Type == Type.Brick)
			{
				PlaceNormalBlock(brickLayer, block, cursorLocation);
			}
		}

		void ClearLayer(Tilemap tilemap, Vector3 cursorLocation)
		{
			var cellPosition = tilemap.WorldToCell(cursorLocation);
			var nullBlocks = new TileBase[16];
			
			tilemap.SetTilesBlock(new BoundsInt(cellPosition, new Vector3Int(4,4,1)), nullBlocks);
		}

		void PlaceNormalBlock(Tilemap tilemap, Block block, Vector3 cursorLocation)
		{
			BoundsInt tilemapCellBounds = block.Tilemap.cellBounds;

			int count = block.Tilemap.GetTilesBlockNonAlloc(tilemapCellBounds, tiles);
			tilemapCellBounds.position = tilemap.WorldToCell(cursorLocation);

			if (count > 16)
				throw new Exception("Tiles amount must be less than 16");

			for (int i = 0; i < count; i++)
			{
				if (tiles[i] != null && tiles[i].name.Contains("null_block"))
				{
					tiles[i] = null;
				}
			}

			tilemap.SetTilesBlock(tilemapCellBounds, tiles);
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawCube(transform.position, Vector3.one);
		}
	}
}