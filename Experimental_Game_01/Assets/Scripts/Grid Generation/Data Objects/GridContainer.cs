using UnityEngine;

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
        private void Update()
        {
            //This is temporary debugging stuff
            if (Input.GetKeyDown(KeyCode.Space))
            {
                themeCycleCounter = themeCycleCounter < AssignedGridAssets.Themes.Length - 1 ? themeCycleCounter + 1 : 0;
                GridAssetTheme.Theme currentTheme = (GridAssetTheme.Theme)themeCycleCounter;
                AssignedGrid.HotSwapTileTheme(currentTheme);
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
