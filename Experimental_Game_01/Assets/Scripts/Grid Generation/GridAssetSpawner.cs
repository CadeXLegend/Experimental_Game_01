﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    /// <summary>
    /// Spawns Assets onto the Grid from a GridAssets Object.
    /// </summary>
    public class GridAssetSpawner : MonoBehaviour
    {
        public delegate void GridAssetSpawningHandler();
        /// <summary>
        /// Called every time an asset is spawned.
        /// </summary>
        public event GridAssetSpawningHandler SpawningAssets;
        /// <summary>
        /// Called after all assets have been spawned.
        /// </summary>
        public event GridAssetSpawningHandler AssetsSpawned;

        [SerializeField]
        private GridAssets gridAssets;
        public GridAssets GridAssets { get => gridAssets; }

        public void Init(GridAssets _gridAssets)
        {
            gridAssets = _gridAssets;
        }

        /// <summary>
        /// Spawn assets into the scene based on the parameters received.
        /// </summary>
        /// <param name="grid">Data Object containing Grid information and functionality.</param>
        /// <param name="assetName">Name for each generated asset</param>
        /// <param name="gridContainer">Parent for the instantiated assets</param>
        public void SpawnAssets(Grid grid, string assetName, Transform gridContainer, GridAssetTheme.Theme theme)
        {
            for (int column = 0; column < grid.Columns; ++column)
            {
                for (int row = 0; row < grid.Rows; ++row)
                {
                    Tile t = grid.TileGrid[column, row];
                    Vector3 position = grid.CoordinatesToWorldPosition(column, row);
                    GameObject go = Instantiate(gridAssets.AssetPrefab, position, Quaternion.identity, gridContainer);
#if UNITY_EDITOR
                    //don't need to do this in build
                    go.name = $"{assetName} (X: {column}  Y:{row})";
#endif
                    SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                    //here we're transposing the Grid's initial tile with the new generated Tile attached to the GameObject!
                    //a simple information swap :)
                    //transpose begins here v
                    Tile goT = go.GetComponent<Tile>();
                    goT.TilePositionOnGrid = t.TilePositionOnGrid;
                    goT.CoordinatesOnGrid = t.CoordinatesOnGrid;
                    goT.spriteRenderer = goT.GetComponent<SpriteRenderer>();
                    grid.TileGrid[column, row] = goT;
                    //transpose ends here ^
                    #region Error Handling
                    GridAssetTheme[] themes = gridAssets.Themes;
                    if(themes[(int)theme] == null)
                        throw new System.Exception($"The {theme.ToString()} (attached to: {this.name}) is null!");
                    Sprite[] sprites = themes[(int)theme].Sprites;
                    if (sprites == null|| sprites.Length < 1)
                        throw new System.Exception($"There must be Sprites assigned to the GridAssets Themes (called from: {this.name})");
                    if ((int)t.TilePositionOnGrid > sprites.Length - 1)
                        throw new System.Exception($"You need at least 9 Sprites for a Theme to fill out every Tile's Position! (called from: {this.name})");
                    if(sprites[(int)t.TilePositionOnGrid] == null)
                        throw new System.Exception($"The Sprite meant for tile at position: {t.TilePositionOnGrid.ToString()} (attached to: {this.name}) is null!");
                    #endregion
                    sr.sprite = sprites[(int)t.TilePositionOnGrid];
                    SpawningAssets?.Invoke();
                }
            }
            AssetsSpawned?.Invoke();
        }

        /// <summary>
        /// Spawn tile decorations onto certain generated tiles as per parameters.
        /// </summary>
        /// <param name="grid">Data Object containing Grid information and functionality.</param>
        /// <param name="assetName">Name for each generated asset</param>
        /// <param name="gridContainer">Parent for the instantiated assets</param>
        public void SpawnTileDecorations(Grid grid, string assetName, Tile.PositionOnGrid positionOnGrid, GridAssetTheme.Theme theme, float spawnRate = 0.1f)
        {
            for (int column = 0; column < grid.Columns; ++column)
            {
                for (int row = 0; row < grid.Rows; ++row)
                {
                    Tile t = grid.TileGrid[column, row];
                    if (t.TilePositionOnGrid == positionOnGrid)
                    {
                        if (!(Random.Range(0.0f, 1.0f) > spawnRate))
                        {
                            Vector3 position = grid.CoordinatesToWorldPosition(column, row);
                            GameObject go = Instantiate(gridAssets.AssetPrefab, position, Quaternion.identity, t.transform);
#if UNITY_EDITOR
                            //don't need to do this in build
                            go.name = $"{assetName} (X: {column}  Y:{row})";
#endif
                            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                            sr.sprite = gridAssets.Themes[(int)theme].TileDecorations[0].Sprite;
                            go.transform.localScale *= gridAssets.Themes[(int)theme].TileDecorations[0].Size;
                            sr.sortingOrder = 1;
                            SpawningAssets?.Invoke();
                        }
                    }
                }
            }
            AssetsSpawned?.Invoke();
        }
    }
}
