using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Generation;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class GridAssetSpawnerTests
    {
        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator theme_asset_is_null()
        {
            SceneManager.LoadScene(0);
            MapGenerator generator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
            generator.Generate();
            GridAssetSpawner spawner = generator.gameObject.GetComponent<GridAssetSpawner>();
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
            yield return new WaitForEndOfFrame();
            Assert.Throws<System.Exception>(() => generator.Generate(), $"The {generator.MapTheme.ToString()} (attached to: {spawner.name}) is null!");
        }

        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator theme_sprites_are_null()
        {
            yield return null;
        }

        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator theme_sprites_are_not_the_required_amount_of_9()
        {
            yield return null;
        }

        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator sprite_for_specific_tile_position_is_null()
        {
            yield return null;
        }
    }
}
