using System;
using UnityEngine;

namespace Generation
{
    /// <summary>
    /// Data Object containing Tile information for Grid related functionality.
    /// </summary>
    public class Tile : MonoBehaviour
    {
        public enum PositionOnGrid
        {
            TopLeftCorner = 0,
            TopEdgeColumn = 1,
            TopRightCorner = 2,
            LeftEdgeRow = 3,
            Center = 4,
            RightEdgeRow = 5,
            BottomLeftCorner = 6,
            BottomEdgeColumn = 7,
            BottomRightCorner = 8,
        }

        //the properties with [SerializeField] [ReadOnly] are for debugging & information purposes in the inspector
        [Header("Tile Information")]
        [SerializeField] [ReadOnly] private PositionOnGrid tilePositionOnGrid;
        public PositionOnGrid TilePositionOnGrid { get => tilePositionOnGrid; set => tilePositionOnGrid = value; }

        [SerializeField] [ReadOnly] private Vector2 coordinateesOnGrid;
        public Vector2 CoordinatesOnGrid { get => coordinateesOnGrid; set => coordinateesOnGrid = value; }

        public SpriteRenderer spriteRenderer { get; set; }

        public void Init(PositionOnGrid _TilePositionOnGrid)
        {
            TilePositionOnGrid = _TilePositionOnGrid;
        }
        public void Init(PositionOnGrid _TilePositionOnGrid, Vector2 _CoordinatesOnGrid)
        {
            TilePositionOnGrid = _TilePositionOnGrid;
            CoordinatesOnGrid = _CoordinatesOnGrid;
        }
    }
}
