﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;

public class GuardScript : ICharacterScript
{
    public string FullName
    {
        get { return "Guard " + Name; }
    }

    public GameObject target;
    public Coordinate DestinationCoordinate;

    // Use this for initialization
    void Awake()
    {
        UnitReady();

        Type = UnitType.Enemy;

        AttackStat = 20;
        DefenseStat = 20;
        Health = 100;
        MovementStat = 5;

        Name = GameObjects.GameManager.GetGuardName();
        NameCanvas.transform.FindChild("NamePlate").GetComponent<Text>().text = FullName;

        var substanceMaterial =
            transform.FindChild("Base_Character").FindChild("Cylinder_001").GetComponent<SkinnedMeshRenderer>().material as ProceduralMaterial;

        substanceMaterial.SetProceduralEnum("ShoeSize", Random.Range(0, 3));
        substanceMaterial.SetProceduralEnum("Hair", Random.Range(3, 5));
        substanceMaterial.SetProceduralEnum("HairColor", Random.Range(0, 3));
        substanceMaterial.SetProceduralEnum("FacialHairStyle", Random.Range(0, 5));

        substanceMaterial.SetProceduralBoolean("Armband", true);
        substanceMaterial.SetProceduralBoolean("Coat", true);
        substanceMaterial.SetProceduralEnum("SkinTone", 1);

        Random.InitState(Random.Range(0, 10000).GetHashCode());

        substanceMaterial.RebuildTextures();

    }

    public WallLocation? GetRotation()
    {
        var angle = transform.eulerAngles.y;

        // South
        if (angle <= 185 && angle >= 175)
        {
            return WallLocation.Lower;
        }

        // Southeast
        if (angle <= 125 && angle >= 115)
        {
            return WallLocation.LowerRight;
        }

        // Northeast
        if (angle <= 65 && angle >= 55)
        {
            return WallLocation.UpperRight;
        }

        // North
        if (angle <= 5 && angle >= 355)
        {
            return WallLocation.Upper;
        }

        // Northwest
        if (angle <= 305 && angle >= 295)
        {
            return WallLocation.UpperLeft;
        }

        // Southwest
        if (angle <= 245 && angle >= 235)
        {
            return WallLocation.LowerLeft;
        }

        return null;
    }

    public void ScanForPlayers()
    {
        var scannedCoordinates = GameObjects.GridGenerator.GetAllNeighbors(currentLocation, true);
        var coordinatesToScan = GameObjects.GridGenerator.GetNeighbors(currentLocation, true);

        var direction = GetRotation();
        if (direction.HasValue)
        {
            var origin = currentLocation;
            for (int i = 0; i < 3; i++)
            {
                var tileInDirection = GameObjects.GridGenerator.GetNeighborInDirection(origin, direction.Value);

                var tempNeighbors = GameObjects.GridGenerator.GetNeighbors(tileInDirection, true);

                // If coordinate is not already scanned, add to scan list
                coordinatesToScan.AddRange(tempNeighbors.Where(neighbor => !scannedCoordinates.Contains(neighbor)));

                // Add all neighbors
                scannedCoordinates.AddRange(GameObjects.GridGenerator.GetAllNeighbors(tileInDirection, true));

                origin = tileInDirection;
            }

            foreach (var coordinate in coordinatesToScan)
            {
                var tileScript = GameObjects.GridGenerator.GetTileAtCoordinates(coordinate).GetComponent<HexTile>();

                if (tileScript.OccupierType == UnitType.Friendly)
                {
                    target = tileScript.Occupier;
                    DestinationCoordinate = tileScript.Coordinate;
                    Debug.Log("Book 'em, Danno.");
                }

                //tileScript.highlighted = true;
                //tileScript.UpdateMaterial();
            }
        }

    }

    void LateUpdate()
    {
        UpdateNamePlate();
    }
}
