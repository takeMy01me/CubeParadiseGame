using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TileTerrainData", menuName = "TileTerrain/TileTerrainData")]
public class TileTerrainData : ConfigBase
{
    public float cellSize = 1;
    public Vector3Int terrainSize = new Vector3Int(10,5,10);
    public TileTerrainCellData[,,] cellDatas; // null的部分意味着是没有地块的
    private TerrainCell[,,] cells;

#if UNITY_EDITOR
    public int terrainEditorPanelIndex = 0;
    public bool enablePreview = false;

    public void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
    }

    public void SetCellSize(float cellSize)
    {
        this.cellSize = cellSize;
        Save();
    }

    public void SetTerrainSize(Vector3Int terrainSize)
    {
        SetData(terrainSize);
        Save();
    }

    // Unity提供的方法
    private void Reset()
    {
        CreateDefaultData();
        Save();
    }

    public void CreateDefaultData()
    {
        cellDatas = new TileTerrainCellData[terrainSize.x, terrainSize.y, terrainSize.z];
        // 默认只有一层，所以 y=1，一共两层循环
        for (int x = 0; x < terrainSize.x; x++)
        {
            for (int z = 0; z < terrainSize.z; z++)
            {
                TileTerrainCellData cellData = new TileTerrainCellData();
                cellData.Init(0, new Vector3Int(x, 0, z));
                cellDatas[x, 0, z] = cellData;
            }
        }
    }

    public void SetData(Vector3Int terrainSize)
    {
        // 创建新的cellDatas，但要尽可能的复用之前的数据
        Vector3Int oldSize = this.terrainSize;
        TileTerrainCellData[,,] oldCellDatas = this.cellDatas;

        this.terrainSize = terrainSize;
        this.cellDatas = new TileTerrainCellData[terrainSize.x, terrainSize.y, terrainSize.z];

        for (int x = 0; x < terrainSize.x; x++)
        {
            for (int y = 0; y < terrainSize.y; y++)
            {
                for (int z = 0; z < terrainSize.z; z++)
                {
                    // 之前的要小，可复用
                    if (oldSize.x < terrainSize.x && oldSize.y < terrainSize.y && oldSize.z < terrainSize.z)
                    {
                        this.cellDatas[x,y,z] = oldCellDatas[x,y,z];
                    }
                    else
                    {
                        if (y == 0)
                        {
                            TileTerrainCellData cellData = new TileTerrainCellData();
                            cellData.Init(0, new Vector3Int(x, 0, z));
                            this.cellDatas[x, 0, z] = cellData;
                        }
                        
                    }
                }
            }
        }
    }
#endif
}

/// <summary>
/// 单个地形格子的数据 - 一个地块的数据
/// </summary>
public class TileTerrainCellData
{
    [SerializeField] private int configIndex;
    public int ConfigIndex { get => configIndex; }
    // 坐标
    [SerializeField] private Vector3Int coordinate;
    public Vector3Int Coordinate { get => coordinate; }

    [NonSerialized] private Vector3 position;
    public Vector3 Position { get => position; }

    public void Init(int configIndex, Vector3Int coordinate)
    {
        this.configIndex = configIndex;
        this.coordinate = coordinate;
    }

    public void InitPosition(float cellSize)
    {
        this.position = new Vector3(Coordinate.x * cellSize + cellSize / 2, Coordinate.y * cellSize, Coordinate.z * cellSize + cellSize / 2); // + cellSize/2 是为了让物体的中心点在中间，方便计算
    }
}
