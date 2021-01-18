using UnityEngine;
using CustomExtensions;
using MyBox;

namespace Generation
{
    /// <summary>
    /// A 2D Map Generator using a Grid.
    /// </summary>
    public class MapGenerator : MonoBehaviour, IGenerator
    {
        public enum MapType
        {
            FittedToScreen,
            Custom,
        }
        public enum MapCameraPivot
        {
            Center,
            Custom,
        }

        public delegate void MapGenerationEvents();
        /// <summary>
        /// This activates before the Grid has been Generated.
        /// </summary>
        public event MapGenerationEvents OnMapGenerating;
        /// <summary>
        /// This activates after the Grid has been Generated.
        /// </summary>
        public event MapGenerationEvents OnMapGenerated;

        private Grid grid;
        public Grid Grid { get => grid; }

        [Header("Grid Information")]
        [SerializeField]
        private GridAssetSpawner assetSpawner;
        [SerializeField]
        private GridContainer mapContainer;
        public GridContainer MapContainer { get => mapContainer; }
        [SerializeField]
        private GridAssetTheme.Theme mapTheme;
        public GridAssetTheme.Theme MapTheme { get => mapTheme; }
        [SerializeField]
        private MapType mapType;
        public MapType _MapType { get => mapType; }
        [SerializeField]
        [ConditionalField("mapType", false, MapType.Custom)]
        private MapCameraPivot mapCameraPivot;
        public MapCameraPivot _MapCameraPivot { get => mapCameraPivot; }
        [SerializeField]
        [ConditionalField("mapCameraPivot", false, MapCameraPivot.Custom)]
        private Vector2 customPivot;
        public Vector2 CustomPivot { get => customPivot; }
        [SerializeField]
        [ConditionalField("mapType", false, MapType.Custom)]
        private Vector2 mapSize;
        public Vector2 MapSize { get => mapSize; }

        [Header("Debugging")]
        [SerializeField]
        private bool showGridDebugLogs;

        public void Init(GridAssetTheme.Theme theme)
        {
            mapTheme = theme;
        }

        public void Init(GridAssetSpawner _spawner, GridContainer container)
        {
            assetSpawner = _spawner;
            mapContainer = container;
        }

        public void Init(GridAssetSpawner _spawner, GridContainer container, GridAssetTheme.Theme theme)
        {
            assetSpawner = _spawner;
            mapContainer = container;
            mapTheme = theme;
        }

        public virtual void Generate()
        {
            if (MapContainer == null)
                throw new UnassignedReferenceException($"Grid Container is not assigned (on {this.name})");
            if (assetSpawner == null)
                throw new UnassignedReferenceException($"Grid Asset Spawner is not assigned (on {this.name})");

            GameActionsLogger.instance.LogAction("World is being generated");
            OnMapGenerating?.Invoke();
            switch (mapType)
            {
                case MapType.FittedToScreen:
                    grid = new Grid((int)Camera.main.orthographicSize, Screen.width, Screen.height, showGridDebugLogs, mapContainer);
                    break;
                case MapType.Custom:
                    grid = new Grid((int)MapSize.x, (int)MapSize.y, showGridDebugLogs, mapContainer);
                    break;
            }
            mapContainer.gridSize = new Vector2(grid.Columns, grid.Rows);
            mapContainer.AssignedGrid = grid;
            mapContainer.AssignedGridAssets = assetSpawner.GridAssets;
            assetSpawner.SpawnAssetsAsTileToFillGrid(grid, "Tile", mapContainer.transform, mapTheme);
            assetSpawner.SpawnTileDecorations(grid, "Tile Decoration", Tile.PositionOnGrid.Center, mapTheme);
            //assetSpawner.SpawnAssetGeneric();
            if (mapType == MapType.Custom)
                switch (mapCameraPivot)
                {
                    case MapCameraPivot.Center:
                        Camera.main.transform.position = grid.TileGrid[grid.Columns / 2, grid.Rows / 2].transform.position;
                        break;
                    case MapCameraPivot.Custom:
                        Camera.main.transform.position = customPivot;
                        break;
                }
            GameActionsLogger.instance.LogAction("World generated successfully");
            OnMapGenerated?.Invoke();
        }
    }
}
