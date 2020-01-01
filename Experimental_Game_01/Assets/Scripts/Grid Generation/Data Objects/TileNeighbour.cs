using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    /// <summary>
    /// Data Object containing Tile neighbour information for Tile related functionality.
    /// </summary>
    public class TileNeighbour : MonoBehaviour
    {
        public enum NeighbourOrientation
        {
            Left,
            Right,
            Up,
            Down,
        }

        public NeighbourOrientation neighbourOrientation;

        public Tile NeighbourTile { get; set; }
    }
}
