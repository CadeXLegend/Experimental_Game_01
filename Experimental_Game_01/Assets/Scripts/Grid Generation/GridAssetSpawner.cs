using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generation
{
    /// <summary>
    /// Spawns Assets onto the Grid from a GridAssets Object.
    /// </summary>
    public class GridAssetSpawner : MonoBehaviour
    {
        public delegate void GridAssetSpawningHandler();
        /// <summary>
        /// Called every time an asset is spawned.
        /// </summary>
        public event GridAssetSpawningHandler SpawningAssets;
        /// <summary>
        /// Called after all assets have been spawned.
        /// </summary>
        public event GridAssetSpawningHandler AssetsSpawned;

        [SerializeField]
        private GridAssets gridAssets;
        public GridAssets GridAssets { get => gridAssets; }

        public void Init(GridAssets _gridAssets)
        {
            gridAssets = _gridAssets;
        }

        /// <summary>
        /// Spawn your own asset directly onto a Tile.
        /// </summary>
        /// <param name="asset">The asset to spawn</param>
        /// <param name="assetName">The name the asset will have post-spawn</param>
        /// <param name="t">The Tile to spawn the asset onto</param>
        public virtual GameObject SpawnAssetGeneric(GameObject asset, string assetName, Tile t)
        {
            if (t.transform.childCount > 0)
            {
                throw new UnityException("You cannot spawn an asset on an already occupied tile!");
            }

            GameObject go = Instantiate(asset, t.transform.position, Quaternion.identity, t.transform);
            go.name = $"{assetName}";
            SpawningAssets?.Invoke();
            AssetsSpawned?.Invoke();
            return go;
        }

        /// <summary>
        /// Spawn your own asset directly onto a Tile.
        /// </summary>
        /// <param name="asset">The asset to spawn</param>
        /// <param name="assetName">The name the asset will have post-spawn</param>
        /// <param name="t">The Tile to spawn the asset onto</param>
        public virtual GameObject SpawnAssetGeneric(GameObject asset, string assetName, Tile t, bool spawnOntoRandomTileNeighbour)
        {
            Transform chosenTransform = t.transform;
            if (spawnOntoRandomTileNeighbour)
                chosenTransform = t.Neighbours[new System.Random().Next(0, t.Neighbours.Count)].NeighbourTile.transform;

            if (chosenTransform.childCount > 0)
            {
                throw new UnityException("You cannot spawn an asset on an already occupied tile!");
            }

            GameObject go = Instantiate(asset, chosenTransform.position, Quaternion.identity, chosenTransform);
            go.name = $"{assetName}";
            SpawningAssets?.Invoke();
            AssetsSpawned?.Invoke();
            return go;
        }

        /// <summary>
        /// Spawn your own asset directly onto a Tile on the Grid.
        /// </summary>
        /// <param name="grid">Grid to Spawn asset onto.</param>
        /// <param name="assetName">Name of your asset to be displayed in the heirarchy.</param>
        /// <param name="positionOnGrid">What position on the Grid you would like your asset to be spawned onto (Tile specific).</param>
        /// <param name="parent">The spawned asset's parent object.</param>
        public virtual GameObject SpawnAssetGeneric(Grid grid, GameObject asset, string assetName, Tile.PositionOnGrid positionOnGrid)
        {
            Tile t = grid.GetRandomUnoccupiedTile(positionOnGrid);
            if (!(t.TilePositionOnGrid == positionOnGrid))
                return null;
            GameObject go = Instantiate(asset, t.transform.position, Quaternion.identity, t.transform);
            go.name = $"{assetName}";
            SpawningAssets?.Invoke();
            AssetsSpawned?.Invoke();
            return go;
        }

        /// <summary>
        /// Spawn assets into the scene as Tiles, based on the parameters received.
        /// </summary>
        /// <param name="grid">Data Object containing Grid information and functionality.</param>
        /// <param name="assetName">Name for each generated asset</param>
        /// <param name="gridContainer">Parent for the instantiated assets</param>
        public virtual List<GameObject> SpawnAssetsAsTileToFillGrid(Grid grid, string assetName, Transform gridContainer, GridAssetTheme.Theme theme)
        {
            List<GameObject> goArray = new List<GameObject>();
            Array.ForEach(grid.TileGrid, (column, row) =>
            {
                Tile t = grid.TileGrid[column, row];
                Vector3 position = grid.CoordinatesToWorldPosition(column, row);
                GameObject go = Instantiate(gridAssets.AssetPrefab, position, Quaternion.identity, gridContainer);
#if UNITY_EDITOR
                //don't need to do this in build
                go.name = $"{assetName} (X: {column}  Y:{row})";
#endif
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                //here we're transposing the Grid's initial tile with the new generated Tile attached to the GameObject!
                //a simple information swap :)
                //transpose begins here v
                Tile goT = go.GetComponent<Tile>();
                goT.TilePositionOnGrid = t.TilePositionOnGrid;
                goT.CoordinatesOnGrid = t.CoordinatesOnGrid;
                goT.spriteRenderer = goT.GetComponent<SpriteRenderer>();
                //transpose ends here ^
                int tilePosOnGrid = t.TilePositionOnGrid.GetUnshiftedNumber() - 1;
                #region Error Handling
                GridAssetTheme[] themes = gridAssets.Themes;
                if (themes[(int)theme] == null)
                    throw new System.Exception($"The {theme.ToString()} (attached to: {this.name}) is null!");
                Sprite[] sprites = themes[(int)theme].Sprites;
                if (sprites == null || sprites.Length < 1)
                    throw new System.Exception($"There must be Sprites assigned to the GridAssets Themes (called from: {this.name})");
                if (tilePosOnGrid > sprites.Length - 1)
                    throw new System.Exception($"You need at least 9 Sprites for a Theme to fill out every Tile's Position! (called from: {this.name})");
                if (sprites[tilePosOnGrid] == null)
                    throw new System.Exception($"The Sprite meant for tile at position: {t.TilePositionOnGrid.ToString()} (attached to: {this.name}) is null!");
                #endregion
                sr.sprite = sprites[tilePosOnGrid];
                grid.TileGrid[column, row] = goT;
                goArray.Add(go);
                SpawningAssets?.Invoke();
            });

            Array.ForEach(grid.TileGrid, (column, row) =>
            {
                #region Neighbour Setting
                List<TileNeighbour> neighbours = new List<TileNeighbour>();
                //right one
                if (!(column + 1 > grid.Columns - 1))
                {
                    TileNeighbour tn = new TileNeighbour();
                    tn.NeighbourTile = grid.TileGrid[column + 1, row]; //right
                    tn.neighbourOrientation = TileNeighbour.NeighbourOrientation.Right;
                    neighbours.Add(tn); //right
                }
                //left one
                if (!(column - 1 < 0))
                {
                    TileNeighbour tn = new TileNeighbour();
                    tn.NeighbourTile = grid.TileGrid[column - 1, row]; //left
                    tn.neighbourOrientation = TileNeighbour.NeighbourOrientation.Left;
                    neighbours.Add(tn);
                }
                //down one
                if (!(row - 1 < 0))
                {
                    TileNeighbour tn = new TileNeighbour();
                    tn.NeighbourTile = grid.TileGrid[column, row - 1]; //down
                    tn.neighbourOrientation = TileNeighbour.NeighbourOrientation.Down;
                    neighbours.Add(tn);
                }
                //up one
                if (!(row + 1 > grid.Rows - 1))
                {
                    TileNeighbour tn = new TileNeighbour();
                    tn.NeighbourTile = grid.TileGrid[column, row + 1]; //up
                    tn.neighbourOrientation = TileNeighbour.NeighbourOrientation.Up;
                    neighbours.Add(tn);
                }
                //up one, left one (top left)
                if (!(column - 1 < 0) && !(row + 1 > grid.Rows - 1))
                {
                    TileNeighbour tn = new TileNeighbour();
                    tn.NeighbourTile = grid.TileGrid[column - 1, row + 1]; //left 1 and up 1
                    tn.neighbourOrientation = TileNeighbour.NeighbourOrientation.TopLeft;
                    neighbours.Add(tn);
                }
                //up one, right one (top right)
                if (!(column + 1 > grid.Columns - 1) && !(row + 1 > grid.Rows - 1))
                {
                    TileNeighbour tn = new TileNeighbour();
                    tn.NeighbourTile = grid.TileGrid[column + 1, row + 1]; //right 1 and up 1
                    tn.neighbourOrientation = TileNeighbour.NeighbourOrientation.TopRight;
                    neighbours.Add(tn);
                }
                //down one, left one (bottom left)
                if (!(row - 1 < 0) && !(column - 1 < 0))
                {
                    TileNeighbour tn = new TileNeighbour();
                    tn.NeighbourTile = grid.TileGrid[column - 1, row - 1]; //left 1 and down 1
                    tn.neighbourOrientation = TileNeighbour.NeighbourOrientation.BottomLeft;
                    neighbours.Add(tn);
                }
                //down one, right one (bottom right)
                if (!(row - 1 < 0) && !(column + 1 > grid.Columns - 1))
                {
                    TileNeighbour tn = new TileNeighbour();
                    tn.NeighbourTile = grid.TileGrid[column + 1, row - 1]; //right 1 and down 1
                    tn.neighbourOrientation = TileNeighbour.NeighbourOrientation.BottomRight;
                    neighbours.Add(tn);
                }

                grid.TileGrid[column, row].AssignNeighbours(neighbours);
                #endregion
            });
            AssetsSpawned?.Invoke();

            return goArray;
        }

        /// <summary>
        /// Spawn your own decoration directly onto a Tile.
        /// </summary>
        /// <param name="asset">The asset to spawn</param>
        /// <param name="assetName">The name the asset will have post-spawn</param>
        /// <param name="t">The Tile to spawn the asset onto</param>
        public virtual GameObject SpawnSingleTileDecoration(GridAssetTheme.Theme theme, string assetName, Tile t)
        {
            if (t.transform.childCount > 0)
            {
                throw new UnityException("You cannot spawn an asset on an already occupied tile!");
            }

            SpawningAssets?.Invoke();
            GameObject go = Instantiate(gridAssets.AssetPrefab, t.transform.position, Quaternion.identity, t.transform);
#if UNITY_EDITOR
            //don't need to do this in build
            go.name = $"{assetName} (Custom)";
#endif
            go.tag = "Resource";
            BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
            collider.size = gridAssets.Themes[(int)theme].TileDecorations[0].ColliderSize;
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = gridAssets.Themes[(int)theme].TileDecorations[0].Sprite;
            go.transform.localScale *= gridAssets.Themes[(int)theme].TileDecorations[0].SpriteScale;
            sr.sortingOrder = 1;
            AssetsSpawned?.Invoke();
            return go;
        }

        /// <summary>
        /// Spawn tile decorations onto certain generated tiles as per parameters.
        /// </summary>
        /// <param name="grid">Data Object containing Grid information and functionality.</param>
        /// <param name="assetName">Name for each generated asset</param>
        /// <param name="gridContainer">Parent for the instantiated assets</param>
        public virtual List<GameObject> SpawnTileDecorations(Grid grid, string assetName, Tile.PositionOnGrid positionOnGrid, GridAssetTheme.Theme theme, float spawnRate = 0.1f)
        {
            List<GameObject> goArray = new List<GameObject>();

            Array.ForEach(grid.TileGrid, (column, row) =>
            {
                Tile t = grid.TileGrid[column, row];
                if (!(t.TilePositionOnGrid == positionOnGrid))
                    return;
                if ((Random.Range(0.0f, 1.0f) > spawnRate))
                    return;

                Vector3 position = grid.CoordinatesToWorldPosition(column, row);
                GameObject go = Instantiate(gridAssets.AssetPrefab, position, Quaternion.identity, t.transform);
#if UNITY_EDITOR
                //don't need to do this in build
                go.name = $"{assetName} (X: {column}  Y:{row})";
#endif
                go.tag = "Resource";
                BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
                collider.size = gridAssets.Themes[(int)theme].TileDecorations[0].ColliderSize;
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                sr.sprite = gridAssets.Themes[(int)theme].TileDecorations[0].Sprite;
                go.transform.localScale *= gridAssets.Themes[(int)theme].TileDecorations[0].SpriteScale;
                sr.sortingOrder = 1;
                SpawningAssets?.Invoke();
                goArray.Add(go);
            });
            AssetsSpawned?.Invoke();
            return goArray;
        }
    }
}
