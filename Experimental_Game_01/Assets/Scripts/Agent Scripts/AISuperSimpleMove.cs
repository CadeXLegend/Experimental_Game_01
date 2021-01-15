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
    [SerializeField]
    private Tile lastTileWasOn;
    [SerializeField]
    private Tile currentTile;
    [SerializeField]
    private Tile tileToMoveTo;
    [SerializeField]
    private GameObject targetPlayer;
    private int attemptsToMove = 0;
    private bool hasFoundTileToMoveTo = false;
    private float movementSpeed;
    private Transform detectionSphere;

    public void Init(Agent.Agent _parent, AgentConfig _config, Tile _currentTile)
    {
        config = _config;
        currentTile = _currentTile;
        parent = _parent;
        movementSpeed = config.MovementSpeed;
        detectionSphere = transform.GetChild(0).transform;
        detectionSphere.localScale *= parent.DetectionRange + 2;
    }

    List<GameObject> playerPositions = new List<GameObject>();
    private void LateUpdate()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(currentTile.transform.position, parent.DetectionRange);

        foreach(var col in cols)
        {
            if(col.CompareTag("Player"))
            {
                playerPositions.Add(col.gameObject);
            }
        }

        if (playerPositions.Count > 0)
        {
            targetPlayer = playerPositions.OrderBy(p => Vector2.Distance(p.transform.position, currentTile.transform.position)).First();
            playerPositions.Clear();
        }
        else
        {
            targetPlayer = currentTile.Neighbours.First(t => !t.NeighbourTile.IsOccupied && !t.NeighbourTile.IsOccupiedByPlayer).NeighbourTile.gameObject;
        }

        if (!parent.CanDoActions)
            return;

        if (attemptsToMove > 9)
        {
            hasFoundTileToMoveTo = false;
            attemptsToMove = 0;
            parent.ProcessAction();
        }

        if (tileToMoveTo)
        {
            Move(tileToMoveTo.transform.position);
            hasFoundTileToMoveTo = false;
            tileToMoveTo = null;
        }

        if (hasFoundTileToMoveTo)
            return;

        if (!tileToMoveTo)
            FindTileToMoveToUsingCurrentTile();
    }

    public virtual void Move(Vector2 direction)
    {
        if (!tileToMoveTo)
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
        if (!currentTile)
            return;
        if(!targetPlayer)
            return;

        List<TileNeighbour> neighbours = currentTile.Neighbours;
        var UnoccupiedTiles = neighbours.Where(n => !n.NeighbourTile.IsOccupied && !n.NeighbourTile.IsOccupiedByPlayer).ToArray();
        int unoccTilesLen = UnoccupiedTiles.Length;
        Tile chosenOne = null;
        Vector2 lowestDistanceScore = new Vector2(999, 999);
        Tile t;
        for (int i = 0; i < unoccTilesLen; ++i)
        {
            t = UnoccupiedTiles[i].NeighbourTile;
            //if(t.spriteRenderer.color != Color.green)
            //    t.spriteRenderer.color = Color.red;
            if (Vector2.Distance(t.transform.position, targetPlayer.transform.position)
              < Vector2.Distance(lowestDistanceScore, targetPlayer.transform.position))
            {
                lowestDistanceScore = t.transform.position;
                chosenOne = t;
            //    t.spriteRenderer.color = Color.green;
            }
        }

        if (!chosenOne)
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
