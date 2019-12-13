using UnityEngine;
using UnityEngine.EventSystems;

namespace Generation
{
    /// <summary>
    /// Data Object containing a Grid as its child.
    /// </summary>
    public class GridContainer : MonoBehaviour
    {
        public Grid AssignedGrid { get; set; }
        public GridAssets AssignedGridAssets { get; set; }

        [Header("Grid Information")]

        [Tooltip("X = Columns, Y = Rows")]
        [ReadOnly] public Vector2 gridSize;

        #region Debugging Stuff
        private int themeCycleCounter = 0;
        private int positionsOnGridCounter = -1;
        private void Update()
        {
            //This is temporary debugging stuff
            if (Input.GetKeyDown(KeyCode.Space))
            {
                themeCycleCounter = themeCycleCounter < AssignedGridAssets.Themes.Length - 1 ? themeCycleCounter + 1 : 0;
                GridAssetTheme.Theme currentTheme = (GridAssetTheme.Theme)themeCycleCounter;
                AssignedGrid.HotSwapTileTheme(currentTheme);
            }

            if(Input.GetKeyDown(KeyCode.Q))
            {
                System.Array positionsOnGrid = System.Enum.GetValues(typeof(Tile.PositionOnGrid));
                positionsOnGridCounter = positionsOnGridCounter < positionsOnGrid.Length - 1 ? positionsOnGridCounter + 1 : 0;
                AssignedGrid.HighlightSectionOfGrid((Tile.PositionOnGrid)positionsOnGridCounter);
            }
        }
        #endregion

        public virtual void ClearGrid()
        {
            foreach (Transform t in transform)
                Destroy(t.gameObject);
        }
    }
}
