using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generation
{
    /// <summary>
    /// Data Object containing Grid information and functionality.
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// All Tiles generated for the Grid.
        /// </summary>
        public Tile[,] TileGrid { get; }
        /// <summary>
        /// Corners of the Grid e.g. Top Left Corner, Top Right Corner, etc.
        /// </summary>
        public List<Tile> GridCorners { get; }
        /// <summary>
        /// The Grid's Parent Object.
        /// </summary>
        public GridContainer Container { get; }
        public float Vertical { get; }
        public int Horizontal { get; }
        public int Columns { get; }
        public int Rows { get; }

        /// <summary>
        /// Generate the Grid based off of the Camera and Screen Size.
        /// </summary>
        /// <param name="OrthographicSize">The Camera's Orthographic Size.</param>
        /// <param name="ScreenWidth">The Current Screen's Width.</param>
        /// <param name="ScreenHeight">The Current Screen's Height.</param>
        public Grid(int OrthographicSize, float ScreenWidth, float ScreenHeight, bool showGridDebugLogs, GridContainer _Container)
        {
            Vertical = OrthographicSize;
            Horizontal = (int)Mathf.Round(Vertical * ScreenWidth / ScreenHeight);
            Columns = Horizontal * 2;
            Rows = (int)Vertical * 2;
            TileGrid = new Tile[Columns, Rows];
            GridCorners = new List<Tile>();
            AssignCoordinatesAndPositionsToTiles();
            Container = _Container;

            if (showGridDebugLogs)
                Debug.Log($"<b>Grid Debug</b>\nOrthoSize: {OrthographicSize} (ScreenWidth: {ScreenWidth}, ScreenHeight: {ScreenHeight})\nVertical: {Vertical}\nHorizontal: {Horizontal}\nColumns: {Columns}\nRows: {Rows}");
        }

        public Grid(int _Columns, int _Rows, bool showGridDebugLogs, GridContainer _Container)
        {
            Columns = _Columns;
            Rows = _Rows;
            TileGrid = new Tile[Columns, Rows];
            GridCorners = new List<Tile>();
            AssignCoordinatesAndPositionsToTiles();
            Container = _Container;

            if (showGridDebugLogs)
                Debug.Log($"<b>Grid Debug</b>Tiles Generated: {Columns * Rows}\nColumns: {Columns}\nRows: {Rows}");
        }

        private void AssignCoordinatesAndPositionsToTiles()
        {
            Tile.PositionOnGrid gridCornersFlags =
                (
                    Tile.PositionOnGrid.TopLeftCorner    |
                    Tile.PositionOnGrid.TopRightCorner   |
                    Tile.PositionOnGrid.BottomLeftCorner |
                    Tile.PositionOnGrid.BottomRightCorner
                );
            Array.ForEach(TileGrid, (column, row) =>
            {
                Vector2 coordinatesOnGrid = new Vector2(column, row);
                Tile.PositionOnGrid tpOnG = SetupTilePositionsOnGrid(coordinatesOnGrid);
                TileGrid[column, row] = new Tile();
                TileGrid[column, row].Init(tpOnG, coordinatesOnGrid);
                Tile t = TileGrid[column, row];

                if (gridCornersFlags.HasFlag(t.TilePositionOnGrid)) GridCorners.Add(t);
            });
        }

        /// <summary>
        /// Sets up the reference for the Tile's position on the Grid.
        /// </summary>
        /// <param name="coordinatesOnGrid">The Vector2 Position of the Tile relative to the Grid.</param>
        /// <returns></returns>
        private Tile.PositionOnGrid SetupTilePositionsOnGrid(Vector2 coordinatesOnGrid)
        {
            int x = (int)coordinatesOnGrid.x;
            int y = (int)coordinatesOnGrid.y;

            if (y == Rows - 1 && x == 0)
                return Tile.PositionOnGrid.TopLeftCorner;
            else if (y == Rows - 1 && x != 0 && x != Columns - 1)
                return Tile.PositionOnGrid.TopEdgeColumn;
            else if (y == Rows - 1 && x == Columns - 1)
                return Tile.PositionOnGrid.TopRightCorner;
            else if (x == 0 && y != 0 && y != Rows - 1)
                return Tile.PositionOnGrid.LeftEdgeRow;
            else if (x == Columns - 1 && y != 0 && y != Rows - 1)
                return Tile.PositionOnGrid.RightEdgeRow;
            else if (x == 0 && y == 0)
                return Tile.PositionOnGrid.BottomLeftCorner;
            if (x != 0 && x != Columns - 1 && y == 0)
                return Tile.PositionOnGrid.BottomEdgeColumn;
            else if (x == Columns - 1 && y == 0)
                return Tile.PositionOnGrid.BottomRightCorner;
            else
                return Tile.PositionOnGrid.Center;
        }

        private Dictionary<Tile, Color> selectedTilesColours = new Dictionary<Tile, Color>();
        /// <summary>
        /// Highlight all Tiles found on the given Position of the Grid.
        /// </summary>
        /// <param name="PositionToHighlight">The Position of the Grid which you want to have Highlighted.</param>
        public virtual void HighlightSectionOfGrid(Tile.PositionOnGrid PositionToHighlight)
        {
            Tile[] flattened = TileGrid.Cast<Tile>().ToArray();
            int len = flattened.Length;
            foreach (var selectedTile in selectedTilesColours)
                selectedTile.Key.spriteRenderer.color = selectedTile.Value;
            selectedTilesColours.Clear();
            for (int i = 0; i < len; ++i)
            {
                Tile t = flattened[i];
                if (t.TilePositionOnGrid == PositionToHighlight)
                {
                    selectedTilesColours.Add(t, t.spriteRenderer.color);
                    t.spriteRenderer.color = Color.gray;
                }
            }
        }

        /// <summary>
        /// Swaps the current Tile Theme to whichever theme you give it.  Warning:  This will change the Tile's Sprites.
        /// </summary>
        /// <param name="theme">The Tile Theme you wish to activate.</param>
        public virtual void HotSwapTileTheme(GridAssetTheme.Theme theme)
        {
            foreach (Tile t in TileGrid)
            {
                t.spriteRenderer.sprite = Container.AssignedGridAssets.Themes[(int)theme].Sprites[t.TilePositionOnGrid.GetUnshiftedNumber()];
                if (t.transform.childCount > 0)
                    for (int i = 0; i < t.transform.childCount; ++i)
                    {
                        if (t.transform.GetChild(i).name.Contains("Decoration"))
                        {
                            SpriteRenderer r = t.transform.GetChild(i).GetComponent<SpriteRenderer>();
                            r.sprite = Container.AssignedGridAssets.Themes[(int)theme].TileDecorations[0].Sprite;
                        }
                    }
            }
        }

        /// <summary>
        /// Swaps the current Tile Theme to whichever theme you give it.  Warning:  This will change the Tile's Sprites.
        /// </summary>
        /// <param name="theme">The Tile Theme you wish to activate.</param>
        public virtual IEnumerator HotSwapTileThemeEnumerable(GridAssetTheme.Theme theme)
        {
            //keeping these var declarations out of the loop since 
            //there's no need for them to be re-created every loop
            //that's excessive allocation when they'll get cleaned
            //post-this function finishing execution
            GridAssetTheme gat;
            int posOnGridRaw;
            Sprite tileSprite;
            SpriteRenderer r;

            foreach (Tile t in TileGrid)
            {
                gat = Container.AssignedGridAssets.Themes[(int)theme];
                posOnGridRaw = t.TilePositionOnGrid.GetUnshiftedNumber() - 1;
                tileSprite = gat.Sprites[posOnGridRaw];
                t.spriteRenderer.sprite = tileSprite;
                if (t.transform.childCount > 0)
                    for (int i = 0; i < t.transform.childCount; ++i)
                    {
                        if (t.transform.GetChild(i).name.Contains("Decoration"))
                        {
                            r = t.transform.GetChild(i).GetComponent<SpriteRenderer>();
                            r.sprite = Container.AssignedGridAssets.Themes[(int)theme].TileDecorations[0].Sprite;
                        }
                    }
                yield return new WaitForSeconds(0.01f);
            }
        }

        public virtual IEnumerator BattleRoyaleHotSwapTileThemeEnumerable()
        {

            return null;
        }

        public virtual IEnumerator CircleHotSwapTileThemeEnumerable(TileNeighbour.NeighbourOrientation orientation)
        {
            return null;
        }

        /// <summary>
        /// Converts the Grid Object's position to the World Position equivalent.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual Vector3 CoordinatesToWorldPosition(int x, int y)
        {
            return new Vector3(x - (Horizontal - 0.5f), y - (Vertical - 0.5f));
        }

        public virtual Tile GetFirstUnoccupiedTile(Tile.PositionOnGrid position)
        {
            int tileLength = TileGrid.Length;
            foreach (Tile t in TileGrid)
            {
                if (t.TilePositionOnGrid == position)
                {
                    if (t.transform.childCount == 0)
                        return t;
                }
            }

            return null;
        }

        /// <summary>
        /// Brute-force recursion to get an unoccupied Tile at random.
        /// </summary>
        /// <param name="position">The position you want it from on the Grid.</param>
        /// <returns></returns>
        public virtual Tile GetRandomUnoccupiedTile(Tile.PositionOnGrid position)
        {
            //first, we get a random tile from the grid
            int randC = Random.Range(0, Columns);
            int randR = Random.Range(0, Rows);
            //we then assign it to a reference for readability
            Tile tileToTest = TileGrid[randC, randR];
            //does it have the position we asked for?
            if (tileToTest.TilePositionOnGrid == position)
                //if yes, does it have no children?  Thus, is it unoccupied?  If yes, return the tile, else, retry this method
                return tileToTest.transform.childCount == 0 ? tileToTest : GetRandomUnoccupiedTile(position);
            else
                //if no, retry with a different tile
                return GetRandomUnoccupiedTile(position);
        }

        public virtual List<Tile> GetAllUnoccupiedTiles()
        {
            List<Tile> UnoccupiedTiles = new List<Tile>();
            foreach (Tile t in TileGrid)
            {
                if (!t.IsOccupied)
                    UnoccupiedTiles.Add(t);
            }

            return UnoccupiedTiles;
        }
    }
}
