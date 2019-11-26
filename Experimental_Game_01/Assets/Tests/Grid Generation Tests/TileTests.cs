using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Generation;

namespace Tests
{
    public class TileTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void tile_init_successfully_binds_data_no_errors()
        {
            // Use the Assert class to test conditions
            Tile t1 = new GameObject().AddComponent<Tile>();
            Tile t2 = new GameObject().AddComponent<Tile>();
            t1.Init(Tile.PositionOnGrid.BottomEdgeColumn);
            t2.Init(Tile.PositionOnGrid.BottomEdgeColumn, new Vector2(0, 1));

            Assert.That(t1.TilePositionOnGrid == Tile.PositionOnGrid.BottomEdgeColumn);
            Assert.That(t2.CoordinatesOnGrid == new Vector2(0, 1));
        }
    }
}
