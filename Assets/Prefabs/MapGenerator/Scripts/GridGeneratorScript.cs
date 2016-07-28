﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class GridGeneratorScript : MonoBehaviour {
    // Public members
    public int Width = 8;
    public int Height = 8;
    public GameObject TilePrefab;
    public GameObject CratePrefab;
    public GameObject GuardPrefab;
    public GameObject EliteGuardPrefab;

    public Texture GrassTexture;
    public Texture DirtTexture;
    public Texture StoneTexture;
    public Texture ConcreteTexture;
    public Texture WoodTexture;

    public void ResetWalls()
    {
        GameObjects.WallManager.Walls.ForEach(DestroyImmediate);
        tiles.ForEach(tile => tile.GetComponent<HexTile>().SpawnWalls());

    }

    

    public GameObject MainSpawnIndicator;
    public GameObject SpawnTwoIndicator;
    public GameObject SpawnThreeIndicator;
    public GameObject SpawnFourIndicator;
    public GameObject SpawnFiveIndicator;
    public GameObject CrateIndicator;
    public GameObject GuardIndicator;

    [HideInInspector]
    public Coordinate badCoordinate = new Coordinate(-999, -999);

    // Class properties
    public List<GameObject> tiles
    {
        get
        {
            return GameObject.FindGameObjectsWithTag("GameTile").ToList();
        }
    }

    #region Spawn Tile Properties

    [SerializeField]
    [HideInInspector]
    private Coordinate _mainSpawn;
    private bool _mainSpawnSet = false;
    public Coordinate MainSpawn
    {
        get
        {
            if (!_mainSpawnSet)
            {
                _mainSpawn = tiles.First(x => x.GetComponent<HexTile>().Contents == TileContents.MainSpawn).GetComponent<HexTile>().Coordinate;
                _mainSpawnSet = true;
            }

            return _mainSpawn;
        }
    }

    [SerializeField]
    [HideInInspector]
    private Coordinate _spawnTwo;
    private bool _spawnTwoSet = false;
    public Coordinate SpawnTwo
    {
        get
        {
            if (tiles.All(x => x.GetComponent<HexTile>().Contents != TileContents.SpawnTwo))
                return new Coordinate(-1, -1);

            if (!_spawnTwoSet)
            {
                _spawnTwo = tiles.First(x => x.GetComponent<HexTile>().Contents == TileContents.SpawnTwo).GetComponent<HexTile>().Coordinate;
                _spawnTwoSet = true;
            }

            return _spawnTwo;
        }
    }

    [SerializeField]
    [HideInInspector]
    private Coordinate _spawnThree;
    private bool _spawnThreeSet = false;
    public Coordinate SpawnThree
    {
        get
        {
            if (tiles.All(x => x.GetComponent<HexTile>().Contents != TileContents.SpawnThree))
                return new Coordinate(-1, -1);

            if (!_spawnThreeSet)
            {
                _spawnThree = tiles.First(x => x.GetComponent<HexTile>().Contents == TileContents.SpawnThree).GetComponent<HexTile>().Coordinate;
                _spawnThreeSet = true;
            }

            return _spawnThree;
        }
    }

    [SerializeField]
    [HideInInspector]
    private Coordinate _spawnFour;
    private bool _spawnFourSet = false;
    public Coordinate SpawnFour
    {
        get
        {
            if (tiles.All(x => x.GetComponent<HexTile>().Contents != TileContents.SpawnFour))
                return new Coordinate(-1, -1);

            if (!_spawnFourSet)
            {
                _spawnFour = tiles.First(x => x.GetComponent<HexTile>().Contents == TileContents.SpawnFour).GetComponent<HexTile>().Coordinate;
                _spawnFourSet = true;
            }

            return _spawnFour;
        }
    }

    [SerializeField]
    [HideInInspector]
    private Coordinate _spawnFive;
    private bool _spawnFiveSet = false;
    public Coordinate SpawnFive
    {
        get
        {
            if (tiles.All(x => x.GetComponent<HexTile>().Contents != TileContents.SpawnFive))
                return new Coordinate(-1, -1);

            if (!_spawnFiveSet)
            {
                _spawnFive = tiles.First(x => x.GetComponent<HexTile>().Contents == TileContents.SpawnFive).GetComponent<HexTile>().Coordinate;
                _spawnFiveSet = true;
            }

            return _spawnFive;
        }
    }

    #endregion

    public List<Coordinate> GetNeighbors(Coordinate target, bool traversibleOnly = false)
    {
        var neighbors = GetNeighborsDirections(target, traversibleOnly);

        return (from neighbor in neighbors where !GameObjects.WallManager.WallExistsBetween(neighbor.Value, target) select neighbor.Value).ToList();
    }

    public List<Coordinate> GetAllNeighbors(Coordinate target, bool traversibleOnly = false)
    {
        var neighbors = GetNeighborsDirections(target, traversibleOnly);

        return neighbors.Select(neighbor => neighbor.Value).ToList();
    }

    public List<KeyValuePair<WallLocation, Coordinate>> GetNeighborsDirections(Coordinate target, bool traversibleOnly = false)
    {
        var neighbors = new List<KeyValuePair<WallLocation, Coordinate>>();

        var upper = GetTileAtCoordinates(target.q, target.r + 1);
        if (upper != null && (upper.GetComponent<HexTile>().Passable || (upper.GetComponent<HexTile>().Traversible && traversibleOnly)))
            neighbors.Add(new KeyValuePair<WallLocation, Coordinate>(WallLocation.Upper, upper.GetComponent<HexTile>().Coordinate));

        var lower = GetTileAtCoordinates(target.q, target.r - 1);
        if (lower != null && (lower.GetComponent<HexTile>().Passable || (lower.GetComponent<HexTile>().Traversible && traversibleOnly)))
            neighbors.Add(new KeyValuePair<WallLocation, Coordinate>(WallLocation.Lower, lower.GetComponent<HexTile>().Coordinate));

        if (target.q % 2 == 0)
        {
            var leftUpper = GetTileAtCoordinates(target.q - 1, target.r);
            if (leftUpper != null && (leftUpper.GetComponent<HexTile>().Passable || (leftUpper.GetComponent<HexTile>().Traversible && traversibleOnly)))
                neighbors.Add(new KeyValuePair<WallLocation, Coordinate>(WallLocation.UpperLeft, leftUpper.GetComponent<HexTile>().Coordinate));

            var leftLower = GetTileAtCoordinates(target.q - 1, target.r - 1);
            if (leftLower != null && (leftLower.GetComponent<HexTile>().Passable || (leftLower.GetComponent<HexTile>().Traversible && traversibleOnly)))
                neighbors.Add(new KeyValuePair<WallLocation, Coordinate>(WallLocation.LowerLeft, leftLower.GetComponent<HexTile>().Coordinate));

            var rightUpper = GetTileAtCoordinates(target.q + 1, target.r);
            if (rightUpper != null && (rightUpper.GetComponent<HexTile>().Passable || (rightUpper.GetComponent<HexTile>().Traversible && traversibleOnly)))
                neighbors.Add(new KeyValuePair<WallLocation, Coordinate>(WallLocation.UpperRight, rightUpper.GetComponent<HexTile>().Coordinate));

            var rightLower = GetTileAtCoordinates(target.q + 1, target.r - 1);
            if (rightLower != null && (rightLower.GetComponent<HexTile>().Passable || (rightLower.GetComponent<HexTile>().Traversible && traversibleOnly)))
                neighbors.Add(new KeyValuePair<WallLocation, Coordinate>(WallLocation.LowerRight, rightLower.GetComponent<HexTile>().Coordinate));
        }
        else
        {
            var leftUpper = GetTileAtCoordinates(target.q - 1, target.r + 1);
            if (leftUpper != null && (leftUpper.GetComponent<HexTile>().Passable || (leftUpper.GetComponent<HexTile>().Traversible && traversibleOnly)))
                neighbors.Add(new KeyValuePair<WallLocation, Coordinate>(WallLocation.UpperLeft, leftUpper.GetComponent<HexTile>().Coordinate));

            var leftLower = GetTileAtCoordinates(target.q - 1, target.r);
            if (leftLower != null && (leftLower.GetComponent<HexTile>().Passable  || (leftLower.GetComponent<HexTile>().Traversible && traversibleOnly)))
                neighbors.Add(new KeyValuePair<WallLocation, Coordinate>(WallLocation.LowerLeft, leftLower.GetComponent<HexTile>().Coordinate));

            var rightUpper = GetTileAtCoordinates(target.q + 1, target.r + 1);
            if (rightUpper != null && (rightUpper.GetComponent<HexTile>().Passable || (rightUpper.GetComponent<HexTile>().Traversible && traversibleOnly)))
                neighbors.Add(new KeyValuePair<WallLocation, Coordinate>(WallLocation.UpperRight, rightUpper.GetComponent<HexTile>().Coordinate));

            var rightLower = GetTileAtCoordinates(target.q + 1, target.r);
            if (rightLower != null && (rightLower.GetComponent<HexTile>().Passable || (rightLower.GetComponent<HexTile>().Traversible && traversibleOnly)))
                neighbors.Add(new KeyValuePair<WallLocation, Coordinate>(WallLocation.LowerRight, rightLower.GetComponent<HexTile>().Coordinate));
        }

        return neighbors;
    }

    public Coordinate GetNeighborInDirection(Coordinate origin, WallLocation direction)
    {
        var neighbors = GetNeighborsDirections(origin);

        var neighbor = neighbors.Where(n => n.Key == direction).ToList();

        if (neighbor.Count == 0 || neighbor.Count > 1)
        {
            Coordinate coordinate = origin;

            if (direction == WallLocation.Upper)
            {
                coordinate.r += 1;
            }
            else if (direction == WallLocation.Lower)
            {
                
                coordinate.r -= 1;
            }
            else
            {
                if (origin.q%2 == 0)
                {
                    switch (direction)
                    {
                        default:
                        case WallLocation.UpperLeft:
                            coordinate.q -= 1;
                            break;
                        case WallLocation.LowerRight:
                            coordinate.q += 1;
                            coordinate.r -= 1;
                            break;
                        case WallLocation.UpperRight:
                            coordinate.q += 1;
                            break;
                        case WallLocation.LowerLeft:
                            coordinate.q -= 1;
                            coordinate.r -= 1;
                            break;
                    }
                }
                else
                {
                    switch (direction)
                    {
                        default:
                        case WallLocation.UpperLeft:
                            coordinate.q -= 1;
                            coordinate.r += 1;
                            break;
                        case WallLocation.LowerRight:
                            coordinate.q += 1;
                            break;
                        case WallLocation.UpperRight:
                            coordinate.q += 1;
                            coordinate.r += 1;
                            break;
                        case WallLocation.LowerLeft:
                            coordinate.q -= 1;
                            break;
                    }
                }
            }

            return coordinate;
        }

        return neighbor[0].Value;
    }

    public List<Coordinate> CalculateRoute(Coordinate start, Coordinate end, bool traversibleOnly = false, int moves = -1)
    {
        var cameFrom = new Dictionary<Coordinate, Coordinate>();
        var costSoFar = new Dictionary<Coordinate, double>();
        var searchGraph = new PriorityQueue<Coordinate>();
        searchGraph.Enqueue(start, 0);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (searchGraph.Count > 0)
        {
            var current = searchGraph.Dequeue();

            if ((moves != -1 && costSoFar[current] > moves))
                return null;
            else if (current.Equals(end))
                return BuildPath(cameFrom, current, start, moves);

            foreach (var next in GetNeighbors(current, traversibleOnly))
            {
                double newCost = costSoFar[current] + GetTileAtCoordinates(current).GetComponent<HexTile>().Weight;
                if (!costSoFar.ContainsKey(next)
                    || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    double priority = newCost + 1;
                    searchGraph.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        return null;
    }

    private List<Coordinate> BuildPath(Dictionary<Coordinate, Coordinate> cameFrom, Coordinate current, Coordinate start, int moves = -1)
    {
        var path = new List<Coordinate>();
        path.Add(current);

        while (cameFrom.Keys.Contains(current) && current != start)
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        path.Remove(start);

        return path;
    }

    public void GenerateGrid()
    {
        Debug.Log(string.Format("Generating {0}x{1} grid...", Width, Height));
        ClearGrid();

        Vector3 position = Vector3.zero;

        
        for (int q = 0; q < Width; q++)
        {
            int logicalRow = 0;
            int qOffset = q >> 1;

            for (int r = -qOffset; r < Height - qOffset; r++)
            {
                var distance = 1f;
                position.x = distance * 3.0f / 2.0f * q;
                position.z = distance * Mathf.Sqrt(3.0f) * (r + q / 2.0f);

                var newTile = Instantiate(TilePrefab, position, Quaternion.identity) as GameObject;
                var newTileScript = newTile.GetComponent<HexTile>();

                newTileScript.Coordinate.q = q;
                newTileScript.Coordinate.r = logicalRow++;
                newTile.transform.parent = this.transform;
                newTile.tag = "GameTile";

                newTileScript.UpdateMaterial();
                newTileScript.UpdateRotation();
            } 
        }
        Debug.Log("Generation of grid complete.");
    }

    public GameObject GetTileAtCoordinates(Coordinate coordinates)
    {
        return GetTileAtCoordinates(coordinates.q, coordinates.r);
    }

    public GameObject GetTileAtCoordinates(int q, int r)
    {
        foreach (var tile in tiles)
        {
            HexTile tileScript = tile.GetComponent<HexTile>();

            if (tileScript.Coordinate.q == q && tileScript.Coordinate.r == r)
                return tile;
        }
        return null;
    }

    public void ClearGrid()
    {
        Debug.Log("Clearing tiles...");

        foreach(GameObject tile in tiles)
            DestroyImmediate(tile);

        Debug.Log("Tiles cleared.");
    }

    public void UpdateWalls()
    {
        tiles.ForEach(tile =>
        {
            var type = WallType.WoodWall;
            var destinationType = WallType.BrickWall;

            var tileScript = tile.GetComponent<HexTile>();
            if (tileScript.UpperLeftWall == type) tileScript.UpperLeftWall = destinationType;
            if (tileScript.UpperRightWall == type) tileScript.UpperRightWall = destinationType;
            if (tileScript.UpperWall == type) tileScript.UpperWall = destinationType;
            if (tileScript.LowerLeftWall == type) tileScript.LowerLeftWall = destinationType;
            if (tileScript.LowerRightWall == type) tileScript.LowerRightWall = destinationType;
            if (tileScript.LowerWall == type) tileScript.LowerWall = destinationType;
        });

        ResetWalls();
    }
}
