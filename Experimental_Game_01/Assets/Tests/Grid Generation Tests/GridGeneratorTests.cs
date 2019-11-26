using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Generation;
using UnityEditor.SceneManagement;

namespace Tests
{
    public class GridGeneratorTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void grid_container_is_not_assigned()
        {
            #region Arrange
            // Use the Assert class to test conditions
            GridGenerator generator = new GameObject().AddComponent<GridGenerator>();
            GridAssetSpawner spawner = new GameObject().AddComponent<GridAssetSpawner>();
            generator.Init(spawner, null);
            #endregion

            #region Act
            var exception = Assert.Throws<UnassignedReferenceException>(() => generator.Generate());
            #endregion

            #region Assert
            Assert.That(exception.Message, Does.Contain($"Grid Container is not assigned (on {generator.name})"));
            #endregion
        }

        [Test]
        public void grid_spawner_is_not_assigned()
        {
            #region Arrange
            // Use the Assert class to test conditions
            GridGenerator generator = new GameObject().AddComponent<GridGenerator>();
            GridContainer container = new GameObject().AddComponent<GridContainer>();
            generator.Init(null, container);
            #endregion

            #region Act
            var exception = Assert.Throws<UnassignedReferenceException>(() => generator.Generate());
            #endregion

            #region Assert
            Assert.That(exception.Message, Does.Contain($"Grid Asset Spawner is not assigned (on {generator.name})"));
            #endregion  
        }

        [Test]
        public void initialize_grid_testing_theme_getter_binding()
        {
            #region Arrange
            // Use the Assert class to test conditions
            GridGenerator generator = new GameObject().AddComponent<GridGenerator>();
            generator.Init(null, null, GridAssetTheme.Theme.Desert);
            #endregion

            Assert.AreEqual(generator.GridTheme, GridAssetTheme.Theme.Desert);
        }

        [Test]
        public void grid_generation_events_firing()
        {
            #region Arrange
            GridAssetTheme[] themes = new GridAssetTheme[1];
            themes[0] = ScriptableObject.CreateInstance<GridAssetTheme>();
            themes[0].SelectedTheme = 0;
            themes[0].TileDecorations = new TileDecoration[1];
            themes[0].TileDecorations[0] = ScriptableObject.CreateInstance<TileDecoration>();
            themes[0].Sprites = new Sprite[9];
            for(int i = 0; i < themes[0].Sprites.Length; ++i)
                themes[0].Sprites[i] = Sprite.Create(new Texture2D(10, 10), new Rect(), new Vector2(0, 0));
            GridAssets assets = new GameObject().AddComponent<GridAssets>();
            GameObject assetPrefab = new GameObject();
            assetPrefab.AddComponent<SpriteRenderer>();
            assetPrefab.AddComponent<Tile>();
            assetPrefab.GetComponent<Tile>().TilePositionOnGrid = 0;
            assets.Init(assetPrefab, themes);
            GridAssetSpawner spawner = new GameObject().AddComponent<GridAssetSpawner>();
            spawner.Init(assets);
            GridContainer container = new GameObject().AddComponent<GridContainer>();
            GridGenerator generator = new GameObject().AddComponent<GridGenerator>();
            generator.Init(spawner, container, 0);
            bool gridGenerating = false;
            bool gridGenerated = false;
            generator.GridGenerating += () => { gridGenerating = true; };
            generator.GridGenerated += () => { gridGenerated = true; };
            #endregion

            Assert.DoesNotThrow(() => generator.Generate());
            Assert.True(gridGenerating);
            Assert.True(gridGenerated);
            Assert.NotNull(generator.Grid);
        }
    }
}
