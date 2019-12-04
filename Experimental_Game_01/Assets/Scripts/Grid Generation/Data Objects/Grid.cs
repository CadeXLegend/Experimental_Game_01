using System.Collections.Generic;
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
            for (int column = 0; column < Columns; ++column)
            {
                for (int row = 0; row < Rows; ++row)
                {
                    Vector2 coordinatesOnGrid = new Vector2(column, row);
                    Tile.PositionOnGrid tpOnG = SetupTilePositionsOnGrid(coordinatesOnGrid);
                    TileGrid[column, row] = new Tile();
                    TileGrid[column, row].Init(tpOnG, coordinatesOnGrid);
                    Tile t = TileGrid[column, row];

                    if (t.TilePositionOnGrid == Tile.PositionOnGrid.TopLeftCorner) GridCorners.Add(t);
                    if (t.TilePositionOnGrid == Tile.PositionOnGrid.BottomLeftCorner) GridCorners.Add(t);
                    if (t.TilePositionOnGrid == Tile.PositionOnGrid.TopRightCorner) GridCorners.Add(t);
                    if (t.TilePositionOnGrid == Tile.PositionOnGrid.BottomRightCorner) GridCorners.Add(t);
                }
            }
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

        /// <summary>
        /// Highlight all Tiles found on the given Position of the Grid.
        /// </summary>
        /// <param name="PositionToHighlight">The Position of the Grid which you want to have Highlighted.</param>
        public virtual void HighlightSectionOfGrid(Tile.PositionOnGrid PositionToHighlight)
        {
            foreach(Tile t in TileGrid)
            {
                if(t.TilePositionOnGrid == PositionToHighlight)
                {
                    t.spriteRenderer.color = Color.blue;
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
                t.spriteRenderer.sprite = Container.AssignedGridAssets.Themes[(int)theme].Sprites[(int)t.TilePositionOnGrid];
                if(t.transform.childCount > 0)
                    for(int i = 0; i < t.transform.childCount; ++i)
                    {
                        SpriteRenderer r = t.transform.GetChild(i).GetComponent<SpriteRenderer>();
                        r.sprite = Container.AssignedGridAssets.Themes[(int)theme].TileDecorations[0].Sprite;
                    }                       
            }
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
            foreach(Tile t in TileGrid)
            {
                if(t.TilePositionOnGrid == position)
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
    }
}
