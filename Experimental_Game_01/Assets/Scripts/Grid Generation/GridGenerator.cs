using UnityEngine;
using CustomExtensions;

namespace Generation
{
    /// <summary>
    /// A Generic Grid Generator.
    /// </summary>
    public class GridGenerator : MonoBehaviour, IGridGenerator
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
        [SerializeField]
        private GridAssetSpawner gridAssetSpawner;

        [SerializeField]
        private Transform gridContainer;

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
            grid = new Grid((int)Camera.main.orthographicSize, Screen.width, Screen.height, showGridDebugLogs);
            gridAssetSpawner.SpawnAssets(grid, "Tile", gridContainer);
            GridGenerated?.Invoke();
        }
    }
}
