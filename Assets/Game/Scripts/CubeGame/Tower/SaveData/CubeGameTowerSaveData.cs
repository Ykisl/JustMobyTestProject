using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;


[Serializable]
public class CubeGameTowerSaveDataCube
{
    public string ModelId;
    public Vector2 Position;
}

[Serializable]
public class CubeGameTowerSaveData
{
    public List<CubeGameTowerSaveDataCube> Cubes = new List<CubeGameTowerSaveDataCube>();
}
