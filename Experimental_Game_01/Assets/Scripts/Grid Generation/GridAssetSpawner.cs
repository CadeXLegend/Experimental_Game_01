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

        /// <summary>
        /// Spawn assets into the scene based on the parameters received.
        /// </summary>
        /// <param name="grid">Data Object containing Grid information</param>
        /// <param name="assetName">Name for each generated asset</param>
        /// <param name="gridContainer">Parent for the instantiated assets</param>
        public void SpawnAssets(Grid grid, string assetName, Transform gridContainer)
        {
            for (int column = 0; column < grid.Columns; ++column)
            {
                for (int row = 0; row < grid.Rows; ++row)
                {
                    Vector3 position = grid.CoordinatesToWorldPosition(column, row);
                    GameObject go = Instantiate(gridAssets.AssetPrefab, position, Quaternion.identity, gridContainer);
                    go.name = $"{assetName} (X: {column}  Y:{row})";
                    SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                    sr.sprite = gridAssets.Sprites[grid.IsEdge(column, row)];
                    SpawningAssets?.Invoke();
                }
            }
            AssetsSpawned?.Invoke();
        }
    }
}
