using UnityEngine;

namespace Generation
{
    /// <summary>
    /// Data Object containing Grid information and functionality.
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// The coordinates for the Grid.
        /// </summary>
        public float[,] Coordinates { get; }
        public float Vertical { get; }
        public int Horizontal { get; }
        public int Columns { get; }
        public int Rows { get; }

        /// <summary>
        /// Generate the Grid based off of the Camera and Screen Size.
        /// </summary>
        /// <param name="OrthographicSize">The Camera's Orthographic Size</param>
        /// <param name="ScreenWidth">The Current Screen's Width</param>
        /// <param name="ScreenHeight">The Current Screen's Height</param>
        public Grid(int OrthographicSize, float ScreenWidth, float ScreenHeight, bool showGridDebugLogs)
        {
            Vertical = OrthographicSize;
            Horizontal = (int)Mathf.Round(Vertical * ScreenWidth / ScreenHeight);
            Columns = Horizontal * 2;
            Rows = (int)Vertical * 2;
            Coordinates = new float[Columns, Rows];
            AssignCoordinateValues();

            if (showGridDebugLogs)
                Debug.Log($"<b>Grid Debug</b>\nOrthoSize: {OrthographicSize} (ScreenWidth: {ScreenWidth}, ScreenHeight: {ScreenHeight})\nVertical: {Vertical}\nHorizontal: {Horizontal}\nColumns: {Columns}\nRows: {Rows}");
        }

        private void AssignCoordinateValues()
        {
            for (int column = 0; column < Columns; ++column)
            {
                for (int row = 0; row < Rows; ++row)
                {
                    Coordinates[column, row] = Random.Range(0f, 1f);
                }
            }
        }

        public virtual int IsEdge(int x, int y)
        {
            if (y == Rows - 1 && x == 0)
                return 0;
            else if (y == Rows - 1 && x != 0 && x != Columns - 1)
                return 1;
            else if (y == Rows - 1 && x == Columns - 1)
                return 2;
            else if (x == 0 && y != 0 && y != Rows - 1)
                return 3;
            else if (x == Columns - 1 && y != 0 && y != Rows - 1)
                return 5;
            else if (x == 0 && y == 0)
                return 6;
            if (x != 0 && x != Columns - 1 && y == 0)
                return 7;
            else if (x == Columns - 1 && y == 0)
                return 8;
            else
                return 4;
        }

        public virtual Vector3 CoordinatesToWorldPosition(int x, int y)
        {
            return new Vector3(x - (Horizontal - 0.5f), y - (Vertical - 0.5f));
        }
    }
}
