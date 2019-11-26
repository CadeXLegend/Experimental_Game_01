using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using Generation;
namespace Tests
{
    public class GridTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void generate_grid_with_no_errors()
        {
            // Use the Assert class to test conditions
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
            GridContainer container = GameObject.FindObjectOfType<GridContainer>();
            Generation.Grid grid;
            Assert.DoesNotThrow(()=> grid = new Generation.Grid((int)Camera.main.orthographicSize, Screen.width, Screen.height, true, container));
        }

        // A Test behaves as an ordinary method
        [Test]
        public void highlight_center_of_grid()
        {
            // Use the Assert class to test conditions
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
            GridContainer container = GameObject.FindObjectOfType<GridContainer>();
            GridGenerator gridGenerator = GameObject.FindObjectOfType<GridGenerator>();
            gridGenerator.Generate();

            gridGenerator.Grid.HighlightSectionOfGrid(Tile.PositionOnGrid.Center);

            foreach (Tile t in gridGenerator.Grid.TileGrid)
                if (t.TilePositionOnGrid == Tile.PositionOnGrid.Center)
                    Assert.AreEqual(expected: Color.blue, actual: t.spriteRenderer.color);
        }

        // A Test behaves as an ordinary method
        [Test]
        public void hot_swap_tile_theme_to_eldritch_from_forest()
        {
            // Use the Assert class to test conditions
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
            GridContainer container = GameObject.FindObjectOfType<GridContainer>();
            GridGenerator gridGenerator = GameObject.FindObjectOfType<GridGenerator>();
            gridGenerator.Init(GridAssetTheme.Theme.Forest);
            gridGenerator.Generate();

            List<Sprite> renderedSprites = new List<Sprite>();
            foreach (Tile t in gridGenerator.Grid.TileGrid)
                renderedSprites.Add(t.spriteRenderer.sprite);

            gridGenerator.Grid.HotSwapTileTheme(GridAssetTheme.Theme.Eldritch);

            foreach (Tile t in gridGenerator.Grid.TileGrid)
                CollectionAssert.DoesNotContain(collection: renderedSprites, actual: t);

        }
    }
}
