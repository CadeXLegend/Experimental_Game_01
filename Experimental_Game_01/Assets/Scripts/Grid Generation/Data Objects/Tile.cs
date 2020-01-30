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
        [SerializeField]
        private List<TileNeighbour> neighbours;
        public List<TileNeighbour> Neighbours { get => neighbours; }
        [Header("Tile Information")]
        [SerializeField] [ReadOnly] private PositionOnGrid tilePositionOnGrid;
        public PositionOnGrid TilePositionOnGrid { get => tilePositionOnGrid; set => tilePositionOnGrid = value; }

        [SerializeField] [ReadOnly] private Vector2 coordinateesOnGrid;
        public Vector2 CoordinatesOnGrid { get => coordinateesOnGrid; set => coordinateesOnGrid = value; }

        public SpriteRenderer spriteRenderer { get; set; }

        [SerializeField] private bool highlighted;
        public bool Highlighted { get => highlighted; }
        [SerializeField] private bool selected;
        public bool Selected { get => selected; }
        [SerializeField] private bool isOccupied;
        public bool IsOccupied { get => isOccupied; }
        [SerializeField] private bool isOccupiedByPlayer;
        public bool IsOccupiedByPlayer { get => isOccupiedByPlayer; }

        public void Init(PositionOnGrid _TilePositionOnGrid)
        {
            TilePositionOnGrid = _TilePositionOnGrid;
        }
        public void Init(PositionOnGrid _TilePositionOnGrid, Vector2 _CoordinatesOnGrid)
        {
            TilePositionOnGrid = _TilePositionOnGrid;
            CoordinatesOnGrid = _CoordinatesOnGrid;
        }

        private void Update()
        {
            isOccupied = transform.childCount == 0 ? false : true;
            if(isOccupied)
                isOccupiedByPlayer = transform.GetChild(0).tag == "Player";
        }

        public void AssignNeighbours(List<TileNeighbour> _neighbours)
        {
            neighbours = _neighbours;
        }

        TileNeighbour.NeighbourOrientation nonoFlag =
        (
        TileNeighbour.NeighbourOrientation.TopLeft    |
        TileNeighbour.NeighbourOrientation.TopRight   |
        TileNeighbour.NeighbourOrientation.BottomLeft |
        TileNeighbour.NeighbourOrientation.BottomRight
        );


        Dictionary<Tile, Color32> lastSelectedTiles = new Dictionary<Tile, Color32>();
        public virtual void VisualizeNeighbours()
        {
            if (neighbours == null || neighbours.Count < 1) return;

            if (lastSelectedTiles.ContainsKey(this))
                return;

            foreach (TileNeighbour n in neighbours)
            {
                Tile t = n.NeighbourTile;

                if (nonoFlag.HasFlag(n.neighbourOrientation))
                    continue;

                if (!t.IsOccupied)
                {
                    if (!lastSelectedTiles.ContainsKey(t))
                        lastSelectedTiles.Add(t, t.spriteRenderer.color);
                    t.highlighted = true;
                    t.spriteRenderer.color = new Color32(255, 255, 255, 100);
                }
                else
                {
                    if (t.transform.GetChild(0).tag == "Enemy")
                    {
                        if (!lastSelectedTiles.ContainsKey(t))
                            lastSelectedTiles.Add(t, t.spriteRenderer.color);
                        t.highlighted = true;
                        t.spriteRenderer.color = new Color32(255, 0, 0, 200);
                    }
                    else if (t.transform.GetChild(0).tag == "Resource")
                    {
                        if (!lastSelectedTiles.ContainsKey(t))
                            lastSelectedTiles.Add(t, t.spriteRenderer.color);
                        t.highlighted = true;
                        t.spriteRenderer.color = new Color32(0, 255, 0, 200);
                    }
                    else if (t.transform.GetChild(0).tag == "Player")
                    {
                        if (!lastSelectedTiles.ContainsKey(t))
                            lastSelectedTiles.Add(t, t.spriteRenderer.color);
                        t.highlighted = true;
                        t.spriteRenderer.color = new Color32(255, 255, 255, 100);
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
