using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;
using UnityEngine.UI;

public class GenerateResources : MonoBehaviour
{
    [SerializeField]
    private MapGenerator mapGenerator;
    [SerializeField]
    private GridAssets assets;
    [SerializeField]
    private GridAssetSpawner spawner;
    [SerializeField]
    private int generationRadius, localResourceTolerance;
    [SerializeField]
    private Slider radiusSlider, toleranceSlider;

    private void Update()
    {
        generationRadius = (int)radiusSlider.value;
        localResourceTolerance = (int)toleranceSlider.value;
    }

    public virtual void Generate()
    {
        bool succeeded = TryGenerate();

        if (!succeeded)
            TryGenerate();
        else
            tilesAttempted.Clear();

        radiusCovered = 0;
        resourceCountWithinRadiusCovered = 0;
    }

    private List<Tile> tilesAttempted = new List<Tile>();
    private bool TryGenerate()
    {
        List<Tile> unoccupiedTiles = mapGenerator.Grid.GetAllUnoccupiedTiles();
        if (tilesAttempted.Count == unoccupiedTiles.Count)
            return true;

        Tile chosenTile = unoccupiedTiles[Random.Range(0, unoccupiedTiles.Count)];
        Tile tileToGenerateOn = FindUnoccupiedTileToGenerateOn(chosenTile.Neighbours);

        if (tileToGenerateOn != null)
        {
            spawner.SpawnSingleTileDecoration(mapGenerator.MapTheme, "Druid Resource", chosenTile);
            Debug.Log($"<color=green><b>Druid Spawned Resource on: {chosenTile.name}</b></color>");
            return true;
        }

        tilesAttempted.Add(chosenTile);
        return false;
    }

    private List<Tile> potentialTilesToSpawnOnto = new List<Tile>();
    private int radiusCovered = 0;
    private int resourceCountWithinRadiusCovered = 0;
    private Tile FindUnoccupiedTileToGenerateOn(List<TileNeighbour> currentNeighbourDepth)
    {
        if (radiusCovered >= generationRadius)
        {
            if (resourceCountWithinRadiusCovered > localResourceTolerance)
                return null;

            return potentialTilesToSpawnOnto[Random.Range(0, potentialTilesToSpawnOnto.Count)];
        }

        radiusCovered++;
        foreach (TileNeighbour n in currentNeighbourDepth)
        {
            if (n.NeighbourTile.Child)
            {
                if (n.NeighbourTile.Type == Tile.TileType.Resource)
                    resourceCountWithinRadiusCovered++;
            }
            else
            {
                if (!potentialTilesToSpawnOnto.Contains(n.NeighbourTile))
                    potentialTilesToSpawnOnto.Add(n.NeighbourTile);
            }

            if (resourceCountWithinRadiusCovered > localResourceTolerance)
                return null;

            FindUnoccupiedTileToGenerateOn(n.NeighbourTile.Neighbours);
        }

        return potentialTilesToSpawnOnto[Random.Range(0, potentialTilesToSpawnOnto.Count)];
    }
}
