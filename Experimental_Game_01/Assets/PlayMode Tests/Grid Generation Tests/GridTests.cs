using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Generation;

namespace Tests
{
    public class GridTests
    {
        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator generate_grid_with_no_errors()
        {
            // Use the Assert class to test conditions
            GridContainer container = GameObject.FindObjectOfType<GridContainer>();
            Generation.Grid grid;
            yield return new WaitForSeconds(1f);
            Assert.DoesNotThrow(()=> grid = new Generation.Grid((int)Camera.main.orthographicSize, Screen.width, Screen.height, true, container));
        }

        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator highlight_center_of_grid()
        {
            // Use the Assert class to test conditions
            GridContainer container = GameObject.FindObjectOfType<GridContainer>();
            MapGenerator gridGenerator = GameObject.FindObjectOfType<MapGenerator>();
            gridGenerator.Generate();

            gridGenerator.Grid.HighlightSectionOfGrid(Tile.PositionOnGrid.Center);
            yield return new WaitForSeconds(1f);
            foreach (Tile t in gridGenerator.Grid.TileGrid)
                if (t.TilePositionOnGrid == Tile.PositionOnGrid.Center)
                    Assert.AreEqual(expected: Color.blue, actual: t.spriteRenderer.color);
        }

        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator hot_swap_tile_theme_to_eldritch_from_forest()
        {
            // Use the Assert class to test conditions
            GridContainer container = GameObject.FindObjectOfType<GridContainer>();
            MapGenerator gridGenerator = GameObject.FindObjectOfType<MapGenerator>();
            gridGenerator.Init(GridAssetTheme.Theme.Forest);
            gridGenerator.Generate();

            List<Sprite> renderedSprites = new List<Sprite>();
            foreach (Tile t in gridGenerator.Grid.TileGrid)
                renderedSprites.Add(t.spriteRenderer.sprite);

            gridGenerator.Grid.HotSwapTileTheme(GridAssetTheme.Theme.Eldritch);
            yield return new WaitForSeconds(1f);
            foreach (Tile t in gridGenerator.Grid.TileGrid)
                CollectionAssert.DoesNotContain(collection: renderedSprites, actual: t);

        }
    }
}
