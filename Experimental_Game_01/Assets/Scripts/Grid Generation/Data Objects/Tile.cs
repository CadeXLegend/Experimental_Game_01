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

        public enum TileType
        {
            Walkable, Unwalkable, Resource, Enemy, Mystery, NPC, Player
        };

        //the properties with [SerializeField] [ReadOnly] are for debugging & information purposes in the inspector
        [SerializeField]
        private List<TileNeighbour> neighbours;
        public List<TileNeighbour> Neighbours { get => neighbours; }
        public Tile Child, Parent;
        [Header("Tile Information")]
        [SerializeField] [ReadOnly] private PositionOnGrid tilePositionOnGrid;
        public PositionOnGrid TilePositionOnGrid { get => tilePositionOnGrid; set => tilePositionOnGrid = value; }
        [SerializeField] private TileType tileType;
        public TileType Type { get => tileType; set => tileType = value; }

        [SerializeField] [ReadOnly] private Vector2 coordinateesOnGrid;
        public Vector2 CoordinatesOnGrid { get => coordinateesOnGrid; set => coordinateesOnGrid = value; }

        public SpriteRenderer spriteRenderer { get; set; }

        [SerializeField] private bool highlighted;
        public bool Highlighted { get => highlighted; }
        [SerializeField] private bool selected;
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

        private void Update()
        {
            OccupancyChecker();
        }

        private void OnTransformChildrenChanged()
        {
            if (transform.childCount < 1)
            {
                Child = null;
                return;
            }

            Child = transform.GetChild(0).GetComponent<Tile>();
        }

        private void OnTransformParentChanged()
        {
            if (!transform.parent)
            {
                Parent = null;
                return;
            }

            Parent = transform.parent.GetComponent<Tile>();
        }

        public void OccupancyChecker()
        {
            if (Parent)
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    -1);

            if (Highlighted && Parent)
                Parent.spriteRenderer.material.SetColor("_Emission", TileSettings.instance.DefaultTileColour);

            if (!Child) return;
            if (Child.Type == TileType.Player)
                spriteRenderer.material.SetColor("_Emission", TileSettings.instance.PlayerTileColour);
        }

        public void AssignNeighbours(List<TileNeighbour> _neighbours)
        {
            neighbours = _neighbours;
        }

        public virtual void VisualizeNeighbours()
        {
            if (Neighbours == null || Neighbours.Count < 1) return;

            foreach (TileNeighbour n in Neighbours)
            {
                Tile nt = n.NeighbourTile;
                nt.highlighted = true;
                if(!nt.Child)
                {
                    nt.spriteRenderer.material.SetColor("_Emission", TileSettings.instance.WalkableTileColour);
                    continue;
                }
                switch (nt.Child.tileType)
                {
                    case TileType.Resource:
                        nt.spriteRenderer.material.SetColor("_Emission", TileSettings.instance.ResourceTileColour);
                        break;
                    case TileType.Enemy:
                        nt.spriteRenderer.material.SetColor("_Emission", TileSettings.instance.EnemyTileColour);
                        break;
                    case TileType.Player:
                        nt.spriteRenderer.material.SetColor("_Emission", TileSettings.instance.PlayerTileColour);
                        break;
                }
            }
        }

        public virtual void StopVisualizingNeighbours()
        {
            foreach (var n in neighbours)
            {
                n.NeighbourTile.highlighted = false;
                n.NeighbourTile.selected = false;
                n.NeighbourTile.spriteRenderer.material.SetColor("_Emission", TileSettings.instance.DefaultTileColour);
            }
            spriteRenderer.material.SetColor("_Emission", TileSettings.instance.DefaultTileColour);
            highlighted = false;
            selected = false;
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
            switch (tileType)
            {
                case TileType.Resource:
                    if (!Parent) return;
                    if (!Parent.Highlighted) return;
                    selected = true;
                    Agents.Agent agent = null;
                    foreach (var neighbour in Parent.Neighbours)
                        if(neighbour.NeighbourTile.Child)
                            if (neighbour.NeighbourTile.Child.Type == TileType.Player)
                                agent = neighbour.NeighbourTile.transform.GetChild(0).GetComponent<Agents.Agent>();
                    int loot = UnityEngine.Random.Range(1, 2);
                    ActionButtons.instance.SetGatherNodeAndAmount("lumber", loot);
                    ActionButtons.instance.SetGatherNodeRef(gameObject, agent);
                    Parent.spriteRenderer.material.SetColor("_Emission", TileSettings.instance.WalkableTileColour);
                    break;
                case TileType.Enemy:
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
            switch (tileType)
            {
                case TileType.Resource:
                    ActionButtons.instance.EnableActionButton(_actionType: ActionButtons.ActionType.Gather);
                    break;
                case TileType.Enemy:
                    ActionButtons.instance.EnableActionButton(_actionType: ActionButtons.ActionType.Attack);
                    break;
                case TileType.Mystery:
                    ActionButtons.instance.EnableActionButton(_actionType: ActionButtons.ActionType.Investigate);
                    break;
                case TileType.NPC:
                    ActionButtons.instance.EnableActionButton(_actionType: ActionButtons.ActionType.Talk);
                    break;
            }
        }
        private void SetActionButtonOff()
        {
            switch (tileType)
            {
                case TileType.Resource:
                    ActionButtons.instance.DisableActionButton(_actionType: ActionButtons.ActionType.Gather);
                    break;
                case TileType.Enemy:
                    ActionButtons.instance.DisableActionButton(_actionType: ActionButtons.ActionType.Attack);
                    break;
                case TileType.Mystery:
                    ActionButtons.instance.DisableActionButton(_actionType: ActionButtons.ActionType.Investigate);
                    break;
                case TileType.NPC:
                    ActionButtons.instance.DisableActionButton(_actionType: ActionButtons.ActionType.Talk);
                    break;
            }
        }
    }
}
