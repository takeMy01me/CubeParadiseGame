using JKFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 格子实体类
/// </summary>
public class TerrainCell
{
    #region 静态配置
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

    #region 检查面&生成
    /// <summary>
    /// 检查所有面
    /// </summary>
    /// <param name="createObj">是否生成物体</param>
    /// <param name="usePool">是否使用对象池</param>
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
        showForward = forwardCell == null; // 前方没有cell 则需要自己显示forward

        if (showForward)
        {
            if (createObj && forwardObj == null)
            {
                if (usePool) // 使用对象池
                {
                    forwardObj = PoolSystem.GetGameObject(configItem.forwardPrefab.name, tileParent);
                }

                if (forwardObj == null) // 避免使用对象池后 forwardObj还是空 的情况
                {
                    forwardObj = GameObject.Instantiate(configItem.forwardPrefab, tileParent);
                    forwardObj.name = configItem.forwardPrefab.name; // Unity的Instantiate()方法 生成出来的gameobject名字后面都带了Clone
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
        showBack = backCell == null; // 前方没有cell 则需要自己显示forward

        if (showBack)
        {
            if (createObj && backObj == null)
            {
                if (usePool) // 使用对象池
                {
                    backObj = PoolSystem.GetGameObject(configItem.backPrefab.name, tileParent);
                }

                if (backObj == null) // 避免使用对象池后 forwardObj还是空 的情况
                {
                    backObj = GameObject.Instantiate(configItem.backPrefab, tileParent);
                    backObj.name = configItem.backPrefab.name; // Unity的Instantiate()方法 生成出来的gameobject名字后面都带了Clone
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
        showLeft = leftCell == null; // 前方没有cell 则需要自己显示forward

        if (showLeft)
        {
            if (createObj && leftObj == null)
            {
                if (usePool) // 使用对象池
                {
                    leftObj = PoolSystem.GetGameObject(configItem.leftPrefab.name, tileParent);
                }

                if (leftObj == null) // 避免使用对象池后 forwardObj还是空 的情况
                {
                    leftObj = GameObject.Instantiate(configItem.leftPrefab, tileParent);
                    leftObj.name = configItem.leftPrefab.name; // Unity的Instantiate()方法 生成出来的gameobject名字后面都带了Clone
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
        showRight = rightCell == null; // 前方没有cell 则需要自己显示forward

        if (showRight)
        {
            if (createObj && rightObj == null)
            {
                if (usePool) // 使用对象池
                {
                    rightObj = PoolSystem.GetGameObject(configItem.rightPrefab.name, tileParent);
                }

                if (rightObj == null) // 避免使用对象池后 forwardObj还是空 的情况
                {
                    rightObj = GameObject.Instantiate(configItem.rightPrefab, tileParent);
                    rightObj.name = configItem.rightPrefab.name; // Unity的Instantiate()方法 生成出来的gameobject名字后面都带了Clone
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
        showTop = topCell == null; // 前方没有cell 则需要自己显示forward

        if (showTop)
        {
            if (createObj && topObj == null)
            {
                if (usePool) // 使用对象池
                {
                    topObj = PoolSystem.GetGameObject(configItem.topPrefab.name, tileParent);
                }

                if (topObj == null) // 避免使用对象池后 forwardObj还是空 的情况
                {
                    topObj = GameObject.Instantiate(configItem.topPrefab, tileParent);
                    topObj.name = configItem.topPrefab.name; // Unity的Instantiate()方法 生成出来的gameobject名字后面都带了Clone
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
