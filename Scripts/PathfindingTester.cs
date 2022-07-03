using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Pathfinding;

public class PathfindingTester : MonoBehaviour
{
    public Pathfinder PathfindingManager;
    public PathFollower PathAgent;

    public int2 StartPos;
    public int2 EndPos;

    public void DoPathFollow(Vector3 worldPos)
    {
        int2 startPos;
        int2 endPos;

        startPos.x = (int)math.round((PathAgent.CurrentPos().x - PathfindingManager.gridData.worldOrigin.x) /PathfindingManager.gridData.cellSize.x);
        startPos.y = (int)math.round((PathAgent.CurrentPos().y - PathfindingManager.gridData.worldOrigin.y) /PathfindingManager.gridData.cellSize.y);

        endPos.x = (int)math.round((worldPos.x - PathfindingManager.gridData.worldOrigin.x) /PathfindingManager.gridData.cellSize.x);
        endPos.y = (int)math.round((worldPos.y - PathfindingManager.gridData.worldOrigin.y) /PathfindingManager.gridData.cellSize.y);

        PathfindingManager.PathFindAStar(startPos, endPos, PathAgent);
    }

    void DoPathFollow(Vector2Int startPos, Vector2Int endPos)
    {
        PathfindingManager.PathFindAStar(new int2(startPos.x, startPos.y), new int2(endPos.x, endPos.y), PathAgent);
    }

    [ContextMenu("Do Path Follow")]
    void DoPathFollow()
    {
        PathfindingManager.PathFindAStar(StartPos, EndPos, PathAgent);
    }

}
