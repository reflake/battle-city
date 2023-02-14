using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelDesigner
{
	public partial class Constructor
	{
		[SerializeField] TileList tileData;
		
		public LevelData GetSerializedData()
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

			var tilesData = tilesArray
				.Select(tile => tile != null ? name2int[tile.name] : 0)
				.ToArray();

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
			};
		}

		public void LoadLevel(LevelData levelData)
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
		}
	}
}