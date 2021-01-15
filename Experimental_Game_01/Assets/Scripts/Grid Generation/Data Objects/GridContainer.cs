using UnityEngine;
using UnityEngine.EventSystems;

namespace Generation
{
    /// <summary>
    /// Data Object containing a Grid as its child.
    /// </summary>
    public class GridContainer : MonoBehaviour
    {
        [SerializeField] private string themeCycleKeycode, gridRegionToggleKeycode;
        public string ThemeCycleKeycode { get => themeCycleKeycode; }
        public string GridRegionToggleKeycode { get => gridRegionToggleKeycode;  }
        private KeyCode themekc, gridregionkc;
        public Grid AssignedGrid { get; set; }
        public GridAssets AssignedGridAssets { get; set; }

        [Header("Grid Information")]

        [Tooltip("X = Columns, Y = Rows")]
        [ReadOnly] public Vector2 gridSize;

        #region Debugging Stuff
        private int themeCycleCounter = 0;
        private int positionsOnGridCounter = -1;

        private void Start()
        {
            themekc = (KeyCode)System.Enum.Parse(typeof(KeyCode), themeCycleKeycode);
            gridregionkc = (KeyCode)System.Enum.Parse(typeof(KeyCode), gridRegionToggleKeycode);
        }

        private void Update()
        {
            //This is temporary debugging stuff
            if (Input.GetKeyDown(themekc))
            {
                themeCycleCounter = themeCycleCounter < AssignedGridAssets.Themes.Length - 1 ? themeCycleCounter + 1 : 0;
                GridAssetTheme.Theme currentTheme = (GridAssetTheme.Theme)themeCycleCounter;
                StartCoroutine(AssignedGrid.HotSwapTileThemeEnumerable(currentTheme));
            }

            if(Input.GetKeyDown(gridregionkc))
            {
                Tile.PositionOnGrid[] positionsOnGrid = (Tile.PositionOnGrid[])System.Enum.GetValues(typeof(Tile.PositionOnGrid));
                positionsOnGridCounter = positionsOnGridCounter < positionsOnGrid.Length - 1 ? positionsOnGridCounter + 1 : 0;
                AssignedGrid.HighlightSectionOfGrid(positionsOnGrid[positionsOnGridCounter]);
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
