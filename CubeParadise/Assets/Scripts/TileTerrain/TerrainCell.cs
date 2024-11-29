using JKFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ʵ����
/// </summary>
public class TerrainCell
{
    #region ��̬����
    private static Vector3 forwardOffsetPos = new Vector3(0, 0.5f, 0.5f);
    private static Vector3 backOffsetPos = new Vector3(0, 0.5f, -0.5f);
    private static Vector3 leftOffsetPos = new Vector3(-0.5f, 0.5f, 0);
    private static Vector3 rightOffsetPos = new Vector3(0.5f, 0.5f, 0);
    private static Vector3 topOffsetPos = new Vector3(0,1,0);

    private static Vector3 forwardRotation = Vector3.zero;
    private static Vector3 backRotation = new Vector3(0, 180, 0);
    private static Vector3 leftRotation = new Vector3(0, 270, 0);
    private static Vector3 rightRotation = new Vector3(0, 90, 0);
    private static Vector3 topRotation = new Vector3(-90, 0, 0);
    #endregion

    private TileTerrain tileTerrain;
    private TileTerrainCellData cellData;
    public TileTerrainCellData CellData { get { return cellData; } }
    private TerrainTileConfigItem configItem;
    private Transform tileParent;

    private GameObject forwardObj;
    private GameObject backObj;
    private GameObject leftObj;
    private GameObject rightObj;
    private GameObject topObj;

    private bool showForward;
    private bool showBack;
    private bool showLeft;
    private bool showRight;
    private bool showTop;

    public void Init(TileTerrain tileTerrain, TileTerrainCellData cellData, TerrainTileConfigItem configItem, Transform tileParent)
    {
        this.tileTerrain = tileTerrain;
        this.cellData = cellData;
        this.configItem = configItem;
        this.tileParent = tileParent;
    }

    #region �����&����
    /// <summary>
    /// ���������
    /// </summary>
    /// <param name="createObj">�Ƿ���������</param>
    /// <param name="usePool">�Ƿ�ʹ�ö����</param>
    public void CheckAllFace(bool createObj, bool usePool)
    {
        CheckForward(createObj, usePool);
        CheckBack(createObj, usePool);
        CheckLeft(createObj, usePool);
        CheckRight(createObj, usePool);
        CheckTop(createObj, usePool);
    }

    private void CheckForward(bool createObj, bool usePool)
    {
        TerrainCell forwardCell = tileTerrain.GetForwardCell(cellData.Coordinate);
        showForward = forwardCell == null; // ǰ��û��cell ����Ҫ�Լ���ʾforward

        if (showForward)
        {
            if (createObj && forwardObj == null)
            {
                if (usePool) // ʹ�ö����
                {
                    forwardObj = PoolSystem.GetGameObject(configItem.forwardPrefab.name, tileParent);
                }

                if (forwardObj == null) // ����ʹ�ö���غ� forwardObj���ǿ� �����
                {
                    forwardObj = GameObject.Instantiate(configItem.forwardPrefab, tileParent);
                    forwardObj.name = configItem.forwardPrefab.name; // Unity��Instantiate()���� ���ɳ�����gameobject���ֺ��涼����Clone
                }

                forwardObj.transform.position = cellData.Position + forwardOffsetPos * tileTerrain.cellSize;
                forwardObj.transform.eulerAngles = forwardRotation;
            }
        }
        else
        {
            if (forwardObj != null)
            {
                if (usePool)
                {
                    PoolSystem.PushGameObject(forwardObj);
                }
                else
                {
                    GameObject.DestroyImmediate(forwardObj);
                }

                forwardObj = null;
            }
        }
    }

    private void CheckBack(bool createObj, bool usePool)
    {
        TerrainCell backCell = tileTerrain.GetBackCell(cellData.Coordinate);
        showBack = backCell == null; // ǰ��û��cell ����Ҫ�Լ���ʾforward

        if (showBack)
        {
            if (createObj && backObj == null)
            {
                if (usePool) // ʹ�ö����
                {
                    backObj = PoolSystem.GetGameObject(configItem.backPrefab.name, tileParent);
                }

                if (backObj == null) // ����ʹ�ö���غ� forwardObj���ǿ� �����
                {
                    backObj = GameObject.Instantiate(configItem.backPrefab, tileParent);
                    backObj.name = configItem.backPrefab.name; // Unity��Instantiate()���� ���ɳ�����gameobject���ֺ��涼����Clone
                }

                backObj.transform.position = cellData.Position + backOffsetPos * tileTerrain.cellSize;
                backObj.transform.eulerAngles = backRotation;
            }
        }
        else
        {
            if (backObj != null)
            {
                if (usePool)
                {
                    PoolSystem.PushGameObject(backObj);
                }
                else
                {
                    GameObject.DestroyImmediate(backObj);
                }

                backObj = null;
            }
        }
    }

    private void CheckLeft(bool createObj, bool usePool)
    {
        TerrainCell leftCell = tileTerrain.GetLeftCell(cellData.Coordinate);
        showLeft = leftCell == null; // ǰ��û��cell ����Ҫ�Լ���ʾforward

        if (showLeft)
        {
            if (createObj && leftObj == null)
            {
                if (usePool) // ʹ�ö����
                {
                    leftObj = PoolSystem.GetGameObject(configItem.leftPrefab.name, tileParent);
                }

                if (leftObj == null) // ����ʹ�ö���غ� forwardObj���ǿ� �����
                {
                    leftObj = GameObject.Instantiate(configItem.leftPrefab, tileParent);
                    leftObj.name = configItem.leftPrefab.name; // Unity��Instantiate()���� ���ɳ�����gameobject���ֺ��涼����Clone
                }

                leftObj.transform.position = cellData.Position + leftOffsetPos * tileTerrain.cellSize;
                leftObj.transform.eulerAngles = leftRotation;
            }
        }
        else
        {
            if (leftObj != null)
            {
                if (usePool)
                {
                    PoolSystem.PushGameObject(leftObj);
                }
                else
                {
                    GameObject.DestroyImmediate(leftObj);
                }

                leftObj = null;
            }
        }
    }

    private void CheckRight(bool createObj, bool usePool)
    {
        TerrainCell rightCell = tileTerrain.GetRightCell(cellData.Coordinate);
        showRight = rightCell == null; // ǰ��û��cell ����Ҫ�Լ���ʾforward

        if (showRight)
        {
            if (createObj && rightObj == null)
            {
                if (usePool) // ʹ�ö����
                {
                    rightObj = PoolSystem.GetGameObject(configItem.rightPrefab.name, tileParent);
                }

                if (rightObj == null) // ����ʹ�ö���غ� forwardObj���ǿ� �����
                {
                    rightObj = GameObject.Instantiate(configItem.rightPrefab, tileParent);
                    rightObj.name = configItem.rightPrefab.name; // Unity��Instantiate()���� ���ɳ�����gameobject���ֺ��涼����Clone
                }

                rightObj.transform.position = cellData.Position + rightOffsetPos * tileTerrain.cellSize;
                rightObj.transform.eulerAngles = rightRotation;
            }
        }
        else
        {
            if (rightObj != null)
            {
                if (usePool)
                {
                    PoolSystem.PushGameObject(rightObj);
                }
                else
                {
                    GameObject.DestroyImmediate(rightObj);
                }

                rightObj = null;
            }
        }
    }

    private void CheckTop(bool createObj, bool usePool)
    {
        TerrainCell topCell = tileTerrain.GetTopCell(cellData.Coordinate);
        showTop = topCell == null; // ǰ��û��cell ����Ҫ�Լ���ʾforward

        if (showTop)
        {
            if (createObj && topObj == null)
            {
                if (usePool) // ʹ�ö����
                {
                    topObj = PoolSystem.GetGameObject(configItem.topPrefab.name, tileParent);
                }

                if (topObj == null) // ����ʹ�ö���غ� forwardObj���ǿ� �����
                {
                    topObj = GameObject.Instantiate(configItem.topPrefab, tileParent);
                    topObj.name = configItem.topPrefab.name; // Unity��Instantiate()���� ���ɳ�����gameobject���ֺ��涼����Clone
                }

                topObj.transform.position = cellData.Position + topOffsetPos * tileTerrain.cellSize;
                topObj.transform.eulerAngles = topRotation;
            }
        }
        else
        {
            if (topObj != null)
            {
                if (usePool)
                {
                    PoolSystem.PushGameObject(topObj);
                }
                else
                {
                    GameObject.DestroyImmediate(topObj);
                }

                topObj = null;
            }
        }
    }

    #endregion
}
