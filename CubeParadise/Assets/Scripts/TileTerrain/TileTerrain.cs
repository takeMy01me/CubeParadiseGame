using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// �ؿ����
/// </summary>
public class TileTerrain : MonoBehaviour
{
    [LabelText("�ؿ�����"), OnValueChanged("OnConifgOrDataSet")]
    public TileTerrainTileConfig terrainTileConfig;
    [LabelText("��������"), OnValueChanged("OnConifgOrDataSet")]
    public TileTerrainData terrainData;
    [SerializeField] public Transform testMousePosition;

    private TileTerrainCellData[,,] cellDatas => terrainData.cellDatas;
    private TerrainCell[,,] cells;

    public float cellSize => terrainData.cellSize;


    #region ����
    public void CreateAllCell()
    {
        if (terrainTileConfig != null && terrainData != null && cellDatas != null)
        {
            cells = new TerrainCell[terrainData.terrainSize.x, terrainData.terrainSize.y, terrainData.terrainSize.z];
            // ��������ĸ����࣬������ݣ����ǲ����ᴴ����Ϸ����
            for (int x = 0; x < terrainData.terrainSize.x; x++)
            {
                for (int y = 0; y < terrainData.terrainSize.y; y++)
                {
                    for (int z = 0; z < terrainData.terrainSize.z; z++)
                    {
                        TileTerrainCellData cellData = cellDatas[x, y, z];

                        if (cellData != null)
                        {
                            cellData.InitPosition(cellSize);
                            TerrainCell cell = new TerrainCell();
                            
                            cell.Init(this, cellData, terrainTileConfig.tileConfigList[cellData.ConfigIndex], this.transform);
                            cells[x, y, z] = cell;
                        }
                    }
                }
            }
            // ����ֱ���ڴ˻��ƣ���Ϊ����ʱ���������ȫ������
        }
    }

    public TerrainCell GetCell(int x, int y, int z)
    {
        if (x < 0 || x >= terrainData.terrainSize.x 
            || y < 0 || y >= terrainData.terrainSize.y
            || z < 0 || z >= terrainData.terrainSize.z)
        {
            return null;
        }

        return cells[x, y, z];
    }

    public TerrainCell GetCell(Vector3Int coordinate)
    {
        return GetCell(coordinate.x, coordinate.y, coordinate.z);
    }

    public TerrainCell GetForwardCell(Vector3Int coordinate)
    {
        return GetCell(coordinate.x, coordinate.y, coordinate.z + 1);
    }

    public TerrainCell GetBackCell(Vector3Int coordinate)
    {
        return GetCell(coordinate.x, coordinate.y, coordinate.z - 1);
    }

    public TerrainCell GetLeftCell(Vector3Int coordinate)
    {
        return GetCell(coordinate.x - 1, coordinate.y, coordinate.z);
    }

    public TerrainCell GetRightCell(Vector3Int coordinate)
    {
        return GetCell(coordinate.x + 1, coordinate.y, coordinate.z);
    }

    public TerrainCell GetTopCell(Vector3Int coordinate)
    {
        return GetCell(coordinate.x, coordinate.y + 1, coordinate.z);
    }

    public TerrainCell GetDownCell(Vector3Int coordinate)
    {
        return GetCell(coordinate.x, coordinate.y - 1, coordinate.z);
    }

    public TerrainCell GetCellByWorldPosition(Vector3 worldPosition)
    {
        return GetCell(GetCellCoordByWorldPosition(worldPosition));
    }

    public Vector3Int GetCellCoordByWorldPosition(Vector3 worldPosition)
    {
        float offset = terrainData.cellSize / 2;
        int x = Mathf.Clamp(Mathf.RoundToInt((worldPosition.x / terrainData.cellSize) - offset), 0, terrainData.terrainSize.x - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt((worldPosition.y / terrainData.cellSize) - offset), 0, terrainData.terrainSize.y - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt((worldPosition.z / terrainData.cellSize) - offset), 0, terrainData.terrainSize.z - 1);

        return new Vector3Int(x, y, z);
    }

    #endregion

    #region RegionForEditor
#if UNITY_EDITOR
    private Action onConifgOrDataSetAction;

    public void SetOnTileConfigSetAction(Action onConifgOrDataSetAction)
    {
        this.onConifgOrDataSetAction = onConifgOrDataSetAction;
    }

    private void OnConifgOrDataSet()
    {
        onConifgOrDataSetAction?.Invoke();
    }

    public void CleanCellsForEditor()
    {
        // TODO: Ŀǰ���ý�Ϊ�����ķ�ʽɾ������
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public void CreateAllCellGameObjectForEditor()
    {
        CleanCellsForEditor();
        // ��������Cell����Ϸ����
        for (int x = 0; x < terrainData.terrainSize.x; x++)
        {
            for (int y = 0; y < terrainData.terrainSize.y; y++)
            {
                for (int z = 0; z < terrainData.terrainSize.z; z++)
                {
                    TerrainCell cell = cells[x, y, z];
                    if (cell != null)
                    {
                        cells[x, y, z].CheckAllFace(true, false);
                    }
                }
            }
        }
    }


    private Action onDrawGizmos;

    public void SetOnDrawGizmos(Action onDrawGizmos)
    {
        this.onDrawGizmos = onDrawGizmos;
    }

    // Gizmos����غ����������OnGizmos�Ļص��У���������ͨ��ί�еķ�ʽȥ����
    private void OnDrawGizmosSelected()
    {
        onDrawGizmos?.Invoke();
        onDrawGizmos = null; // Editor SetAction�Ժ� ��߾ͻ����
    }
#endif
    #endregion
}
