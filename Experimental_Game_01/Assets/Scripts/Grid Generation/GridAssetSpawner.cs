using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAssetSpawner : MonoBehaviour
{
    [SerializeField]
    private GridAssets gridAssets;

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
            }
        }
    }
}
