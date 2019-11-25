using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    /// <summary>
    /// A Generic 2D Boundary Generator.
    /// </summary>
    public class BoundaryGenerator : MonoBehaviour, IGenerator
    {
        /// <summary>
        /// Not implemented.
        /// </summary>
        public void Generate()
        {
            throw new System.NotImplementedException();
        }

        public void Generate(Grid grid)
        {
            Transform topLeft = null, bottomLeft = null, topRight = null, bottomRight = null;
            foreach(Tile t in grid.GridCorners)
            {
                if (t.TilePositionOnGrid == Tile.PositionOnGrid.TopLeftCorner) topLeft = t.GetComponent<Transform>();
                if (t.TilePositionOnGrid == Tile.PositionOnGrid.BottomLeftCorner) bottomLeft = t.GetComponent<Transform>();
                if (t.TilePositionOnGrid == Tile.PositionOnGrid.TopRightCorner) topRight = t.GetComponent<Transform>();
                if (t.TilePositionOnGrid == Tile.PositionOnGrid.BottomRightCorner) bottomRight = t.GetComponent<Transform>();
            }

            BoxCollider2D collider = new BoxCollider2D();
            Vector2 centreOfCorners = (topLeft.transform.position + topRight.transform.position) / 2;
            collider.transform.position = topLeft.transform.position;
            collider.size = new Vector2(
                topRight.transform.position.x,
                topLeft.transform.localScale.y);
        }
    }
}
