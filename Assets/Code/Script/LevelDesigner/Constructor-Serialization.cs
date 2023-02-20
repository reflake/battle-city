using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelDesigner
{
	public partial class Constructor
	{
		[SerializeField] TileList tileData;
		
		const int matrixElements = 16;
		
		public LevelData GetLevelData()
		{
			// Find and write individual tile names
			var usedTilesCount = tilemap.GetUsedTilesCount();
			var usedTiles = new TileBase[usedTilesCount];

			tilemap.GetUsedTilesNonAlloc(usedTiles);

			var int2name = new Dictionary<int, string>();
			var name2int = new Dictionary<string, int>();

			int2name.Add(0, "null");

			int index = 1;

			foreach (var tile in usedTiles)
			{
				name2int[tile.name] = index;
				int2name[index++] = tile.name;
			}

			// Prepare tilemaps for serialization
			tilemap.CompressBounds();

			// Find all tiles on layer and serialize as index of name
			var bounds = tilemap.cellBounds;
			var tilesArray = tilemap.GetTilesBlock(bounds);
			var filledTileAmount = tilesArray.Count(x => x != null);
			var transformsArray = new float[filledTileAmount * matrixElements];
			
			var tilesData = tilesArray
				.Select(tile => tile != null ? name2int[tile.name] : 0)
				.ToArray();

			ReadTransforms(bounds, transformsArray);

			return new LevelData
			{
				tileMapIds = int2name.Keys.ToArray(),
				tileMapNames = int2name.Values.ToArray(),
				boundsX = bounds.x,
				boundsY = bounds.y,
				boundsZ = bounds.z,
				boundsW = bounds.size.x,
				boundsH = bounds.size.y,
				boundsD = bounds.size.z,
				tilesData = tilesData,
				transformsData = transformsArray
			};
		}

		private void ReadTransforms(BoundsInt bounds, float[] arr)
		{
			IntPtr temporaryMemoryPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Matrix4x4>());
			bool structureCreated = false;
			int index = 0;

			// Write all transforms for tiles
			try
			{
				foreach (var pos in bounds.allPositionsWithin)
				{
					if (tilemap.HasTile(pos))
					{
						var mat = tilemap.GetTransformMatrix(pos);

						Marshal.StructureToPtr(mat, temporaryMemoryPtr, false);
						structureCreated = true;
						
						Marshal.Copy(temporaryMemoryPtr, arr, index, matrixElements);
						
						Marshal.DestroyStructure<Matrix4x4>(temporaryMemoryPtr);
						structureCreated = false;

						index += matrixElements;
					}
				}
			}
			finally
			{
				if (structureCreated)
					Marshal.DestroyStructure<Matrix4x4>(temporaryMemoryPtr);

				Marshal.FreeHGlobal(temporaryMemoryPtr);
			}
		}

		public void LoadLevelData(LevelData levelData)
		{
			// Unload current map
			tilemap.ClearAllTiles();

			// List all names of used tiles
			var tileByName = new Dictionary<int, TileBase>();

			for (int i = 0; i < levelData.tileMapNames.Length; i++)
			{
				var tileId = levelData.tileMapIds[i];
				var tileName = levelData.tileMapNames[i];

				if (tileName == "null")
				{
					tileByName.Add(tileId, null);
				}
				else
				{
					var tileRef = tileData.tiles.Single(x => x.name == tileName);

					tileByName.Add(tileId, tileRef);
				}
			}

			// Load new map
			var bounds = new BoundsInt(
				levelData.boundsX, levelData.boundsY, levelData.boundsZ,
				levelData.boundsW, levelData.boundsH, levelData.boundsD);

			var tiles = levelData.tilesData
				.Select(tileId => tileByName[tileId])
				.ToArray();

			tilemap.SetTilesBlock(bounds, tiles);

			SetTransforms(bounds, levelData.transformsData);
		}

		private void SetTransforms(BoundsInt bounds, float[] transforms)
		{
			IntPtr temporaryMemoryPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Matrix4x4>());
			int index = 0;

			try
			{
				foreach (var pos in bounds.allPositionsWithin)
				{
					if (tilemap.HasTile(pos))
					{
						Marshal.Copy(transforms, index, temporaryMemoryPtr, matrixElements);
					
						var matrix = Marshal.PtrToStructure<Matrix4x4>(temporaryMemoryPtr);
						tilemap.SetTransformMatrix(pos, matrix);

						index += matrixElements;
					}
				}
			}
			finally
			{
				Marshal.FreeHGlobal(temporaryMemoryPtr);
			}
		}
	}
}