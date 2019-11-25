using System.Collections;
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

        /// <summary>
        /// Spawn assets into the scene based on the parameters received.
        /// </summary>
        /// <param name="grid">Data Object containing Grid information and functionality.</param>
        /// <param name="assetName">Name for each generated asset</param>
        /// <param name="gridContainer">Parent for the instantiated assets</param>
        public void SpawnAssets(Grid grid, string assetName, Transform gridContainer, GridAssetThemes.Theme theme)
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
                    sr.sprite = gridAssets.Themes[(int)theme].Sprites[(int)t.TilePositionOnGrid];
                    SpawningAssets?.Invoke();
                }
            }
            AssetsSpawned?.Invoke();
        }
    }
}
