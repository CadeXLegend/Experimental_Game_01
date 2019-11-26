using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Generation;

namespace Tests
{
    public class GridAssetSpawnerTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void theme_asset_is_null()
        {
            UnityEngine.SceneManagement.Scene openscene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
            GridGenerator generator = GameObject.FindObjectOfType<GridGenerator>();
            GridAssetSpawner spawner = GameObject.FindObjectOfType<GridAssetSpawner>();
            GridAssets assets = new GameObject().AddComponent<GridAssets>();
            GameObject assetPrefab = new GameObject();
            GridAssetTheme[] themes = new GridAssetTheme[3];
            for (int i = 0; i < themes.Length; ++i)
            {
                themes[i] = ScriptableObject.CreateInstance<GridAssetTheme>();
                themes[i].SelectedTheme = GridAssetTheme.Theme.Forest;
            }
            themes[0].Sprites = new Sprite[9];
            for (int i = 0; i < themes[0].Sprites.Length; ++i)
                themes[0].Sprites[i] = Sprite.Create(new Texture2D(10, 10), new Rect(), Vector2.down);
            assetPrefab.AddComponent<Tile>();
            assets.Init(assetPrefab, themes);
            spawner.Init(assets);
            Assert.Throws<System.Exception>(() => generator.Generate(), $"The {generator.GridTheme.ToString()} (attached to: {spawner.name}) is null!");
        }

        // A Test behaves as an ordinary method
        [Test]
        public void theme_sprites_are_null()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        }

        // A Test behaves as an ordinary method
        [Test]
        public void theme_sprites_are_not_the_required_amount_of_9()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        }

        // A Test behaves as an ordinary method
        [Test]
        public void sprite_for_specific_tile_position_is_null()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        }
    }
}
