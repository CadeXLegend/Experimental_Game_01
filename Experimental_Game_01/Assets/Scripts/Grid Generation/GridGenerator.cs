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

        [Header("Grid Information")]
        [SerializeField]
        private GridAssetSpawner gridAssetSpawner;
        [SerializeField]
        private GridAssetThemes.Theme gridTheme = GridAssetThemes.Theme.Forest;
        public GridAssetThemes.Theme GridTheme { get => gridTheme; }
        [SerializeField]
        private GridContainer gridContainer;

        [Header("Debugging")]
        [SerializeField]
        private bool showGridDebugLogs;

        PromisedAction pAction = new PromisedAction();
        private void Start()
        {
            pAction.ActionFailed += () => {
                Debug.Log(pAction.ErrorMessage);
            };
            pAction.ActionSucceeded += () => {
                Debug.Log("<color=blue><b>Grid successfully generated!</b></color>");
            };
            pAction.Call(() => { Generate(); });
        }

        public virtual void Generate()
        {
            GridGenerating?.Invoke();
            grid = new Grid((int)Camera.main.orthographicSize, Screen.width, Screen.height, showGridDebugLogs, gridContainer);
            gridContainer.gridSize = new Vector2(grid.Columns, grid.Rows);
            gridContainer.AssignedGrid = grid;
            gridContainer.AssignedGridAssets = gridAssetSpawner.GridAssets;
            gridAssetSpawner.SpawnAssets(grid, "Tile", gridContainer.transform, gridTheme);
            //BoundaryGenerator boundaryGen = new BoundaryGenerator();
            //boundaryGen.Generate(grid);
            GridGenerated?.Invoke();
        }
    }
}
