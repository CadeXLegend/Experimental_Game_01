using UnityEngine;
using CustomExtensions;

namespace Generation
{
    /// <summary>
    /// A Generic Grid Generator.
    /// </summary>
    public class GridGenerator : MonoBehaviour, IGenerator
    {
        public delegate void GridGenerationHandler();
        /// <summary>
        /// This activates before the Grid has been Generated.
        /// </summary>
        public event GridGenerationHandler GridGenerating;
        /// <summary>
        /// This activates after the Grid has been Generated.
        /// </summary>
        public event GridGenerationHandler GridGenerated;

        private Grid grid;
        public Grid Grid { get => grid; }

        [Header("Grid Information")]
        [SerializeField]
        private GridAssetSpawner spawner;
        [SerializeField]
        private GridAssetTheme.Theme gridTheme;
        public GridAssetTheme.Theme GridTheme { get => gridTheme; }
        [SerializeField]
        private GridContainer gridContainer;
        public GridContainer GridContainer { get => gridContainer; }

        [Header("Debugging")]
        [SerializeField]
        private bool showGridDebugLogs;

        public void Init(GridAssetTheme.Theme theme)
        {
            gridTheme = theme;
        }

        public void Init(GridAssetSpawner _spawner, GridContainer container)
        {
            spawner = _spawner;
            gridContainer = container;
        }

        public void Init(GridAssetSpawner _spawner, GridContainer container, GridAssetTheme.Theme theme)
        {
            spawner = _spawner;
            gridContainer = container;
            gridTheme = theme;
        }

        public virtual void Generate()
        {
            if (GridContainer == null)
                throw new UnassignedReferenceException($"Grid Container is not assigned (on {this.name})");
            if (spawner == null)
                throw new UnassignedReferenceException($"Grid Asset Spawner is not assigned (on {this.name})");

            GridGenerating?.Invoke();
            grid = new Grid((int)Camera.main.orthographicSize, Screen.width, Screen.height, showGridDebugLogs, gridContainer);
            gridContainer.gridSize = new Vector2(grid.Columns, grid.Rows);
            gridContainer.AssignedGrid = grid;
            gridContainer.AssignedGridAssets = spawner.GridAssets;
            spawner.SpawnAssets(grid, "Tile", gridContainer.transform, gridTheme);
            spawner.SpawnTileDecorations(grid, "Tile Decoration", Tile.PositionOnGrid.Center, gridTheme);
            GridGenerated?.Invoke();
        }
    }
}
