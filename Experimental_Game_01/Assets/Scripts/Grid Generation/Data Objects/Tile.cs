using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.GameFoundation;
using UnityEngine.EventSystems;

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
            Default =               0,                   // 0
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
        public Tile Child, Parent;
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
            OccupancyChecker();
        }

        public void OccupancyChecker()
        {
            if (Parent) 
                transform.position = new Vector3(
                    transform.position.x, 
                    transform.position.y, 
                    -1);

            isOccupied = transform.childCount == 0 ? false : true;
            if (!IsOccupied)
            {
                if(Highlighted && Parent)
                    Parent.spriteRenderer.color = new Color32(255, 255, 255, 100);
                isOccupiedByPlayer = false;
                return;
            }
            isOccupiedByPlayer = transform.GetChild(0).CompareTag("Player");
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

                //this is for if we want 4-directional movement
                //rather than 8-directional movement
                //if (nonoFlag.HasFlag(n.neighbourOrientation))
                //    continue;

                //if(isOccupiedByPlayer || t.isOccupiedByPlayer)
                //    t.spriteRenderer.color = new Color32(255, 255, 255, 200);

                if (!t.IsOccupied)
                {
                    if (!lastSelectedTiles.ContainsKey(t))
                        lastSelectedTiles.Add(t, t.spriteRenderer.color);
                    t.highlighted = true;
                    t.spriteRenderer.color = new Color32(113, 255, 10, 200);
                }
                else
                {
                    if (!lastSelectedTiles.ContainsKey(t))
                        lastSelectedTiles.Add(t, t.spriteRenderer.color);
                    t.highlighted = true;
                    switch (t.transform.GetChild(0).tag)
                    {
                        case "Enemy":
                            t.spriteRenderer.color = new Color32(255, 108, 0, 200);
                            break;
                        case "Resource":
                            t.spriteRenderer.color = new Color32(0, 255, 0, 200);
                            break;
                        //case "Player":
                        //    t.spriteRenderer.color = new Color32(0, 0, 255, 100);
                        //    break;
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

        private void OnMouseOver()
        {
            if (Highlighted || Parent && Parent.Highlighted)
            {
                ActionButtons.instance.SetActionButtonPosition(Camera.main.WorldToScreenPoint(transform.position));
                ActionButtons.instance.SetCursorVisibility(true);
            }
            if (!Parent) return;
            if (!Parent.Highlighted) return;
            if (!ActionButtons.instance.IsActive) return;
            SetActionButtonOn();
        }

        private void OnMouseExit()
        {
            if (!ActionButtons.instance.IsActive) return;
            SetActionButtonOff();
            ActionButtons.instance.SetCursorVisibility(false);
        }

        private void OnMouseUp()
        {
            switch (tag)
            {
                case "Resource":
                    if (!Parent) return;
                    if (!Parent.Highlighted) return;
                    selected = true;
                    Agents.Agent agent = null;
                    foreach (var neighbour in Parent.Neighbours)
                        if (neighbour.NeighbourTile.IsOccupiedByPlayer)
                            agent = neighbour.NeighbourTile.transform.GetChild(0).GetComponent<Agents.Agent>();
                    int loot = UnityEngine.Random.Range(1, 2);
                    ActionButtons.instance.SetGatherNodeAndAmount("lumber", loot);
                    ActionButtons.instance.SetGatherNodeRef(gameObject, agent);
                    Parent.spriteRenderer.color = new Color32(113, 255, 10, 200);
                    break;
                case "Enemy":
                    selected = true;
                    break;
                default:
                    if (!highlighted) return;
                    selected = true;
                    ActionButtons.instance.SetCursorVisibility(false);
                    break;
            }
        }

        private void SetActionButtonOn()
        {
            switch (tag)
            {
                case "Resource":
                    ActionButtons.instance.EnableActionButton(_actionType: ActionButtons.ActionType.Gather);
                    break;
                case "Enemy":
                    ActionButtons.instance.EnableActionButton(_actionType: ActionButtons.ActionType.Attack);
                    break;
                case "Investigate":
                    ActionButtons.instance.EnableActionButton(_actionType: ActionButtons.ActionType.Investigate);
                    break;
                case "Talk":
                    ActionButtons.instance.EnableActionButton(_actionType: ActionButtons.ActionType.Talk);
                    break;
            }
        }
        private void SetActionButtonOff()
        {
            switch (tag)
            {
                case "Resource":
                    ActionButtons.instance.DisableActionButton(_actionType: ActionButtons.ActionType.Gather);
                    break;
                case "Enemy":
                    ActionButtons.instance.DisableActionButton(_actionType: ActionButtons.ActionType.Attack);
                    break;
                case "Investigate":
                    ActionButtons.instance.DisableActionButton(_actionType: ActionButtons.ActionType.Investigate);
                    break;
                case "Talk":
                    ActionButtons.instance.DisableActionButton(_actionType: ActionButtons.ActionType.Talk);
                    break;
            }
        }
    }
}
