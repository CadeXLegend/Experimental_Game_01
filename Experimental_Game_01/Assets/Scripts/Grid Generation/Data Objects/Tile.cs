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

        private bool highlighted;
        public bool Highlighted { get => highlighted; }
        private bool selected;
        public bool Selected { get => selected; }

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
        public virtual void VisualizeNeighbours()
        {
            if (neighbours == null || neighbours.Count < 1) return;
            if (lastSelectedTiles.ContainsKey(this))
                return;

            foreach (Tile n in neighbours)
            {
                if (n.transform.childCount == 0)
                {
                    lastSelectedTiles.Add(n, n.spriteRenderer.color);
                    n.highlighted = true;
                    n.spriteRenderer.color = new Color32(255, 0, 255, 100);
                }
                else
                {
                    if (n.transform.GetChild(0).tag == "Enemy")
                    {
                        lastSelectedTiles.Add(n, n.spriteRenderer.color);
                        n.highlighted = true;
                        n.spriteRenderer.color = new Color32(255, 0, 0, 200);
                    }
                }
            }
            lastSelectedTiles.Add(this, spriteRenderer.color);
        }

        public virtual void StopVisualizingNeighbours()
        {
            if (lastSelectedTiles == null || lastSelectedTiles.Count < 1) return;

            foreach (KeyValuePair<Tile, Color32> pair in lastSelectedTiles)
            {
                pair.Key.highlighted = false;
                pair.Key.selected = false;
                pair.Key.spriteRenderer.color = pair.Value;
            }
            lastSelectedTiles.Clear();
        }

        private void OnMouseDown()
        {
            if (highlighted)
                selected = true;
        }
    }
}
