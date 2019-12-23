using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    /// <summary>
    /// Data Object containing Tile information for Grid related functionality.
    /// </summary>
    public class Tile : MonoBehaviour
    {
        [Flags]
        public enum PositionOnGrid
        {
            TopLeftCorner =         1 << 0,              // 1
            TopEdgeColumn =         1 << 1,              // 2
            TopRightCorner =        1 << 2,              // 4
            LeftEdgeRow =           1 << 3,              // 8
            Center =                1 << 4,              // 16
            RightEdgeRow =          1 << 5,              // 32
            BottomLeftCorner =      1 << 6,              // 64
            BottomEdgeColumn =      1 << 7,              // 128
            BottomRightCorner =     1 << 8,              // 256
        }

        //the properties with [SerializeField] [ReadOnly] are for debugging & information purposes in the inspector
        private List<Tile> neighbours;
        public List<Tile> Neighbours { get => neighbours; }
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

        public void AssignNeighbours(List<Tile> _neighbours)
        {
            neighbours = _neighbours;
        }

        Dictionary<Tile, Color32> lastSelectedTiles = new Dictionary<Tile, Color32>();
        private void OnMouseDown()
        {
            if (neighbours == null || neighbours.Count < 1) return;

            foreach (Tile n in neighbours)
            {
                lastSelectedTiles.Add(n, n.spriteRenderer.color);
                n.spriteRenderer.color = new Color32(255, 0, 255, 100);
            }
            lastSelectedTiles.Add(this, spriteRenderer.color);
            spriteRenderer.color = new Color32(255, 0, 0, 100);
        }
        private void OnMouseUp()
        {
            if (lastSelectedTiles == null || lastSelectedTiles.Count < 1) return;

            foreach (KeyValuePair<Tile, Color32> pair in lastSelectedTiles)
                pair.Key.spriteRenderer.color = pair.Value;
            lastSelectedTiles.Clear();
        }
    }
}
