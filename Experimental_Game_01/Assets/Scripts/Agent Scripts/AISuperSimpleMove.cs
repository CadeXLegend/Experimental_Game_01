using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agent;
using Generation;
using System.Linq;

public class AISuperSimpleMove : MonoBehaviour, IMover
{
    private Agent.Agent parent;
    private AgentConfig config;
    private Tile lastTileWasOn;
    private Tile currentTile;
    private Tile tileToMoveTo;
    private int attemptsToMove = 0;
    private bool hasFoundTileToMoveTo = false;
    private float movementSpeed;

    public void Init(Agent.Agent _parent, AgentConfig _config, Tile _currentTile)
    {
        config = _config;
        currentTile = _currentTile;
        parent = _parent;
        movementSpeed = config.MovementSpeed;
    }

    private void LateUpdate()
    {
        if (!parent.CanDoActions)
            return;

        if (attemptsToMove > 9)
        {
            hasFoundTileToMoveTo = false;
            attemptsToMove = 0;
            parent.ProcessAction();
        }

        if (tileToMoveTo != null)
            Move(tileToMoveTo.transform.position);

        if (hasFoundTileToMoveTo)
            return;

        FindTileToMoveToUsingCurrentTile();
    }

    public virtual void Move(Vector2 direction)
    {
        if (tileToMoveTo == null)
            return;

        if (Vector2.Distance(transform.position, direction) < 0.01f)
        {
            currentTile = tileToMoveTo;
            transform.parent = tileToMoveTo.transform;
            hasFoundTileToMoveTo = false;
            parent.ProcessAction();
            return;
        }

        transform.position = Vector2.LerpUnclamped(transform.position, direction, movementSpeed * Time.deltaTime);
    }

    private void FindTileToMoveToUsingCurrentTile()
    {
        if (currentTile == null)
            return;

        List<TileNeighbour> neighbours = currentTile.Neighbours;
        var UnoccupiedTiles = neighbours.Where(n => !n.NeighbourTile.IsOccupied);
        Tile chosenOne = null;
        Vector2 lowestDistanceScore = new Vector2(999, 999);
        GameObject player = GameObject.Find("Player");
        foreach (var n in UnoccupiedTiles)
        {
            Tile t = n.NeighbourTile;
            if (Vector2.Distance(t.transform.position, player.transform.position) < Vector2.Distance(lowestDistanceScore, player.transform.position))
            {
                lowestDistanceScore = t.transform.position;
                chosenOne = t;
            }
        }

        if (chosenOne == null)
        {
            attemptsToMove++;
            return;
        }

        lastTileWasOn = currentTile;
        tileToMoveTo = chosenOne;
        hasFoundTileToMoveTo = true;
        return;
    }
}
