﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

    [SerializeField] Waypoint startWaypoint, endWaypoint;

    Dictionary<Vector2Int, Waypoint> grid = new Dictionary<Vector2Int, Waypoint>();
    Queue<Waypoint> queue = new Queue<Waypoint>();
    bool isRunning = true;
    Waypoint searchCenter;
    List<Waypoint> path = new List<Waypoint>();

    Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public List<Waypoint> GetPath()
    {
        LoadBlocks();
        ColorStartAndEnd();
        BreadthFirstSearch();
        CreatePath();
        return path;
    }

    private void CreatePath()
    {
        path.Add(endWaypoint);
        Waypoint previous = endWaypoint.exploredFrom;

        while(previous != startWaypoint)
        {
            path.Add(previous);
            previous = previous.exploredFrom;
        }

        path.Add(startWaypoint);
        path.Reverse();
    }

    private void BreadthFirstSearch()
    {
        queue.Enqueue(startWaypoint);

        while(queue.Count > 0 && isRunning)
        {
            searchCenter = queue.Dequeue();
            HaltIfEndFound();
            ExploreNeighbors();
            searchCenter.isExplored = true;
        }
    }

    private void HaltIfEndFound()
    {
        if(searchCenter == endWaypoint)
        {
            print("End found");
            isRunning = false;
        }
    }

    private void ExploreNeighbors()
    {
        if (!isRunning) { return; }

        foreach(Vector2Int direction in directions)
        {
            Vector2Int neighborCoordinates = searchCenter.GetGridPos() + direction;
            if(grid.ContainsKey(neighborCoordinates))
            {
                QueueNewNeighbors(neighborCoordinates);
            }
        }
    }

    private void QueueNewNeighbors(Vector2Int neighborCoordinates)
    {
        Waypoint neighbor = grid[neighborCoordinates];
        if (neighbor.isExplored || queue.Contains(neighbor))
        {
            return;
        }
        else
        {
            queue.Enqueue(neighbor);
            neighbor.exploredFrom = searchCenter;
        }
    }

    private void ColorStartAndEnd()
    {
        startWaypoint.SetTopColor(Color.green);
        endWaypoint.SetTopColor(Color.red);
    }

    private void LoadBlocks()
    {
        var waypoints = FindObjectsOfType<Waypoint>();
        foreach(Waypoint waypoint in waypoints)
        {
            var gridPos = waypoint.GetGridPos();
            if (grid.ContainsKey(gridPos))
            {
                Debug.Log("Overlapping block " + waypoint);
            }
            else
            {
                grid.Add(gridPos, waypoint);
            }
        }
        print("Loaded " + grid.Count + " blocks");
    }
}
