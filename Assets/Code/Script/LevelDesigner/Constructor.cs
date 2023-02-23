using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Gameplay;
using UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ModestTree;
using Project;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Zenject;

namespace LevelDesigner
{
	public partial class Constructor : MonoBehaviour
	{
		[SerializeField] SpriteRenderer cursorSprite;
		
		[Space]
		
		[SerializeField] Tilemap tilemap;
		[SerializeField] GameObject blocksPrefab;

		[Inject] readonly BattleField _battleField;
		[Inject] readonly PanelManager _panelManager;
		[Inject] readonly CustomLevelContext _customLevelContext = null;
		
		bool _holdPaint = false;
		bool samePlace = false;
		int blockCycle = 0;
		Block[] _blocks;
		ConstructorControls _controls;
		TileBase[] tiles = new TileBase[16];
		Matrix4x4[] transforms = new Matrix4x4[16];
		ConstructorPanel _constructorPanel;
		
		public void Activate()
		{
			gameObject.SetActive(true);
		}
		
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

			if (_customLevelContext.HasCustomLevelForNewGame)
			{
				LoadLevelData(_customLevelContext.GetNewGameLevel());
			}
			else
			{
				LoadChunk("Base");
			}
		}

		void OnEnable()
		{
			_constructorPanel = _panelManager.CreatePanel<ConstructorPanel>(ConstructorPanel.prefabPath, 0);
		}

		void OnDisable()
		{
			if (_constructorPanel != null)
				Destroy(_constructorPanel.gameObject);
		}

		void OnDestroy()
		{
			_controls.Dispose();
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

			ClearLayer(4, tilemap, cursorLocation);
			PlaceNormalBlock(4, fillBlock, cursorLocation);
		}

		void ClearLayer(int cellsPerUnit, Tilemap tilemap, Vector3 cursorLocation)
		{
			var cellPosition = tilemap.WorldToCell(cursorLocation);
			var nullBlocks = new TileBase[cellsPerUnit * cellsPerUnit];
			var blockBounds = new BoundsInt(cellPosition, new Vector3Int(cellsPerUnit,cellsPerUnit,1));
			
			tilemap.SetTilesBlock(blockBounds, nullBlocks);
		}

		void PlaceNormalBlock(int cellsPerUnit, Block block, Vector3 cursorLocation)
		{
			if (block.IsNullBlock)
				return;
			
			int count = block.GetBlockTilesNonAlloc(tiles, transforms);
			
			if (count > cellsPerUnit * cellsPerUnit)
				throw new Exception($"Tiles amount must be less than {cellsPerUnit * cellsPerUnit}");

			// TODO: better explanation
			var blockBounds = block.Tilemap.cellBounds;
			
			blockBounds.position = tilemap.WorldToCell(cursorLocation);

 			tilemap.SetTilesBlock(blockBounds, tiles);
            
            int index = 0;
			
            for (int j = blockBounds.yMin; j < blockBounds.yMax; j++)
            for (int i = blockBounds.xMin; i < blockBounds.xMax; i++)
            {
	            tilemap.SetTransformMatrix(new Vector3Int(i, j, 0), transforms[index++]);
            }
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawCube(transform.position, Vector3.one);
		}

		void LoadChunk(string fileName)
		{
			var asset = Resources.Load<TextAsset>($"Level/{fileName}");
			using var file = new MemoryStream(asset.bytes);
			IFormatter formatter = new BinaryFormatter();

			var data = formatter.Deserialize(file) as LevelData;

			LoadLevelData(data);
		}

		public void SetNewGameCustomLevel()
		{
			var customLevelData = GetLevelData();

			_customLevelContext.SetNewGameLevel(customLevelData);
		}
	}
}