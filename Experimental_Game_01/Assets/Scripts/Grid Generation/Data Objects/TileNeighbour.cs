using System;
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
        [Flags]
        public enum NeighbourOrientation
        {
            Left =              1 << 0,              // 1
            Right =             1 << 1,              // 2
            Up =                1 << 2,              // 4
            Down =              1 << 3,              // 8
            TopLeft =           1 << 4,              // 16
            TopRight =          1 << 5,              // 32
            BottomLeft =        1 << 6,              // 64
            BottomRight =       1 << 7,              // 128
        }

        public NeighbourOrientation neighbourOrientation;

        public Tile NeighbourTile { get; set; }
    }
}
