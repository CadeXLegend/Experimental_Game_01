using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Generic Grid Generator.
/// </summary>
public class GridGenerator : MonoBehaviour, IGridGenerator
{
    public delegate void GridGeneratorEvents();
    /// <summary>
    /// This activates before the Grid has been Generated.
    /// </summary>
    public event GridGeneratorEvents GridGenerating;
    /// <summary>
    /// This activates after the Grid has been Generated.
    /// </summary>
    public event GridGeneratorEvents GridGenerated;

    private Grid grid;
    [SerializeField]
    private GridAssetSpawner gridAssetSpawner;

    [SerializeField]
    private Transform gridContainer;

    [SerializeField]
    private bool showGridDebugLogs;
    private void Start()
    {
        Generate();
    }

    public virtual void Generate()
    {
        GridGenerating?.Invoke();
        grid = new Grid((int)Camera.main.orthographicSize, Screen.width, Screen.height, showGridDebugLogs);
        gridAssetSpawner.SpawnAssets(grid, "Tile", gridContainer);
        GridGenerated?.Invoke();
    }
}
