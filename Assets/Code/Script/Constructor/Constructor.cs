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

			_controls.Cursor.Move.started += MoveCursorInput;

			_controls.Cursor.Paint.started += PaintBlockInput;
			
			_blocks = blocksPrefab.GetComponentsInChildren<Block>();
		}

		void MoveCursorInput(InputAction.CallbackContext context)
		{
			int holdDelay = 400;
			int holdInterval = 100;
			
			Vector3 moveDirection = context.ReadValue<Vector2>();

			var cts = new CancellationTokenSource();
			var token = cts.Token;

			async UniTaskVoid MoveCursorCycle()
			{
				bool needHold = true;
				
				await UniTask.Yield();
				
				samePlace = false;
				
				while (!token.IsCancellationRequested)
				{
					await UniTask.Yield();

					transform.position += moveDirection;
					
					if (_holdPaint)
						
						PaintBlock();

					await UniTask.Delay(needHold ? holdDelay : holdInterval, DelayType.Realtime);

					needHold = false;
				}
			}

			Task.Run(() => MoveCursorCycle(), token);

			void Performed(InputAction.CallbackContext callbackContext)
			{
				moveDirection = callbackContext.ReadValue<Vector2>();
			}
			
			void Canceled(InputAction.CallbackContext callbackContext)
			{
				cts.Cancel();
				
				context.action.performed -= Performed;
				context.action.canceled -= Canceled;
			}
			
			context.action.performed += Performed;
			context.action.canceled += Canceled;
		}

		void PaintBlockInput(InputAction.CallbackContext context)
		{
			if (samePlace)
				
				blockCycle = (blockCycle + 1) % _blocks.Length;
			else
			
				samePlace = true;

			_holdPaint = true;

			PaintBlock();

			void Cancel(InputAction.CallbackContext _)
			{
				_holdPaint = false;
				
				context.action.canceled -= Cancel;
			}

			context.action.canceled += Cancel;
		}

		void PaintBlock()
		{
			var brushOffset = new Vector3(-0.45f, -.45f);
			var block = _blocks[blockCycle];

			BoundsInt tilemapCellBounds = block.Tilemap.cellBounds;

			block.Tilemap.GetTilesBlockNonAlloc(tilemapCellBounds, tiles);
			tilemapCellBounds.position = brickLayer.WorldToCell(transform.position + brushOffset);

			for (int i = 0; i < tiles.Length; i++)
			{
				if (tiles[i] != null && tiles[i].name.Contains("null_block"))
				{
					tiles[i] = null;
				}
			}

			brickLayer.SetTilesBlock(tilemapCellBounds, tiles);
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawCube(transform.position, Vector3.one);
		}
	}
}