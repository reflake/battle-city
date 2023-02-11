﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

		void ClearLayer(int cellsPerUnit, Tilemap tilemap, Vector3 cursorLocation)
		{
			var cellPosition = tilemap.WorldToCell(cursorLocation);
			var nullBlocks = new TileBase[16];
			
			tilemap.SetTilesBlock(new BoundsInt(cellPosition, new Vector3Int(cellsPerUnit,cellsPerUnit,1)), nullBlocks);
		}

		void PlaceNormalBlock(int cellsPerUnit, Tilemap tilemap, Block block, Vector3 cursorLocation)
		{
			BoundsInt tilemapCellBounds = block.Tilemap.cellBounds;

			int count = block.Tilemap.GetTilesBlockNonAlloc(tilemapCellBounds, tiles);
			tilemapCellBounds.position = tilemap.WorldToCell(cursorLocation);

			if (count > cellsPerUnit * cellsPerUnit)
				throw new Exception($"Tiles amount must be less than {cellsPerUnit * cellsPerUnit}");

			for (int i = 0; i < count; i++)
			{
				if (tiles[i] != null && tiles[i].name.Contains("null_block"))
				{
					tiles[i] = null;
				}
			}

			tilemap.SetTilesBlock(tilemapCellBounds, tiles);
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
			
			void FillLayer(Type blockType, Tilemap layer, int cellsPerUnit)
			{
				if (blockType != fillBlock.Type)
					ClearLayer(cellsPerUnit, layer, cursorLocation);

				if (blockType == fillBlock.Type)
					PlaceNormalBlock(cellsPerUnit, layer, fillBlock, cursorLocation);
			}
							
			FillLayer(Type.Brick, brickLayer, 4);
			FillLayer(Type.Concrete, concreteLayer, 2);
			FillLayer(Type.Top, topLayer, 2);
			FillLayer(Type.Bottom, bottomLayer, 2);
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawCube(transform.position, Vector3.one);
		}
	}
}