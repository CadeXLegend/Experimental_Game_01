using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agent;
using Generation;

public class AISuperSimpleMove : MonoBehaviour, IMover
{
    private AgentConfig config;
    private Tile lastTileWasOn;
    private Tile currentTile;
    private Tile tileToMoveTo;

    public void Init(AgentConfig _config, Tile _currentTile)
    {
        config = _config;
        currentTile = _currentTile;

        TurnTicker.OnTick += MoveSuperSimpleApproach;
    }

    private void MoveSuperSimpleApproach()
    {
        Move(FindPosToMoveToUsingCurrentTile());
    }

    public virtual void Move(Vector2 direction)
    {
        if (tileToMoveTo == null)
            return;

        transform.position = Vector3.Lerp(transform.position, direction, 1f);
        currentTile = tileToMoveTo;
        transform.parent = tileToMoveTo.transform;
    }

    private Vector2 FindPosToMoveToUsingCurrentTile()
    {
        if (currentTile == null)
            return transform.position;

        List<Tile> neighbours = currentTile.Neighbours;

        Vector2 lowestDistanceScore = new Vector2(999, 999);
        GameObject player = GameObject.Find("Player");
       // foreach(Tile t in neighbours)
       // {
        //    if(Vector2.Distance(t.transform.position, player.transform.position) < Vector2.Distance(lowestDistanceScore, player.transform.position))
        //        lowestDistanceScore = t.transform.position;
        //}

        //return lowestDistanceScore;

        Tile chosenOne = neighbours[new System.Random().Next(0, neighbours.Count)];
        if (chosenOne.transform.childCount > 0)
            return transform.position;

        lastTileWasOn = currentTile;
        tileToMoveTo = chosenOne;
        return tileToMoveTo.transform.position;
    }
}
