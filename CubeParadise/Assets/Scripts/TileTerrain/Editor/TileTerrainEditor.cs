using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Sirenix.OdinInspector.Editor;
using UnityEditor.UIElements;
using System;

[CustomEditor(typeof(TileTerrain))] // 特性: 接管Inspector的绘制
public class TileTerrainEditor : OdinEditor
{
    [MenuItem("GameObject/TileTerrain")]
    private static void CreateTileTerrainObject()
    {
        GameObject tileTerrain = new GameObject("TileTerrain");
        tileTerrain.AddComponent<TileTerrain>();
        Selection.activeGameObject = tileTerrain;
    }

    // 脚本面板赋值
    public VisualTreeAsset editorUIAsset;
    /// <summary>
    /// root 包含两部分
    /// 1. odin绘制的部分
    /// 2. 从editorUIAsset实例化出来的部分
    /// </summary>
    private VisualElement root; 

    private TileTerrain terrain { get { return (TileTerrain)target; } }

    private VisualElement editorUIInstance;
    private TileTerrainTileConfig curTileConfig;

    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        IMGUIContainer container = new IMGUIContainer(() =>
        {
            base.DrawDefaultInspector();
        });
        root.Add(container);

        // 保存一下当前的配置
        curTileConfig = terrain.terrainTileConfig;
        // 监听 如果用户又设置了配置
        terrain.SetOnTileConfigSetAction(OnConfigOrDataSet);

        // 如果配置不存在的话 直接返回odin渲染部分即可
        if (terrain.terrainTileConfig == null || terrain.terrainData == null)
        {
            return root;
        }

        DrawEditor();
        return root; 
    }

    protected override void OnDisable()
    {
        if (terrain.terrainData != null)
        {
            terrain.terrainData.Save();
        }

        editorUIInstance = null;
        CleanTilePrefabPreview();

        base.OnDisable();
    }

    
    private void DrawEditor()
    {
        if (editorUIInstance != null)
        {
            if (editorUIInstance.parent == root)
            {
                root.Remove(editorUIInstance);
            }
            editorUIInstance = null;
        }
         
        // 验证绘制的可行性
        if (terrain.terrainTileConfig != null && terrain.terrainData != null)
        {
            editorUIInstance = editorUIAsset.Instantiate();
            root.Add(editorUIInstance);
            // 监听 如果用户修改了配置中的细节
            terrain.terrainTileConfig.SetOnTileConfigChangedAction(OnTileConfigChanged);

            InitTopMenu(); // 初始化顶部菜单
            InitPanel(); // 初始化面板
        }
        
    }

    // 当设置地块配置
    private void OnConfigOrDataSet()
    {
        // Editor保存的配置和Terrain当前配置不相等，意味着用户修改了配置
        // 清理上一次Config的当值修改Action
        if (curTileConfig != terrain.terrainTileConfig)
        {
            if (terrain.terrainTileConfig != null && curTileConfig != null) curTileConfig.CleanEditorAction();
            curTileConfig = terrain.terrainTileConfig;
        }
        
        DrawEditor();
    }

    // 当地块配置修改时要触发的行为
    private void OnTileConfigChanged()
    {
        // 重新绘制地形面板
        InitTerrainPanel();

        if (terrain.terrainData.enablePreview)
        {
            CreateAllCellGameObject();
        }
    }

    /// <summary>
    /// 在场景视图中启用“编辑器”来处理事件 Unity自带的
    /// </summary>
    private void OnSceneGUI()
    {
        if (terrain.terrainData == null || terrain.terrainTileConfig == null || terrain.terrainData.enablePreview == false)
        {
            return;
        }

        switch (curPanelType)
        {
            case PanelType.Terrain:
                TerrainPanelOnSceneGUI();
                break;
            case PanelType.Item:
                break;
            case PanelType.Setting:
                break;
        }

        
    }

    #region 工具函数
    private Camera sceneViewCamera => SceneView.currentDrawingSceneView.camera; // 编辑器模式下
    private Vector3 GetMousePosition()
    {
        // 获取屏幕坐标 左上角是 0,0 => 但摄像机需要的屏幕坐标，左下角是 0,0,0
        Vector2 eventMousePosition = Event.current.mousePosition;
        Vector3 mousePosition = eventMousePosition;
        mousePosition.y = sceneViewCamera.pixelHeight - eventMousePosition.y;
        return mousePosition;
    }

    private void DrawCellGizmos(Vector3 position)
    {
        terrain.SetOnDrawGizmos(() =>
        {
            Gizmos.DrawWireCube(position, terrain.terrainData.cellSize * Vector3.one);
        });
    }
    #endregion

    #region 顶部菜单
    private List<Button> topButtonList;
    private static Color normalMenuItemColor = new Color(0.35f, 0.35f, 0.35f);
    private static Color selectedMenuItemColor = new Color(0.24f, 0.57f, 0.7f);

    private Toggle previewToggle;
    private enum PanelType
    {
        Terrain = 0,
        Item = 1,
        Setting = 2
    }

    private void InitTopMenu()
    {
        previewToggle = root.Q<Toggle>("PreviewToggle");
        previewToggle.RegisterValueChangedCallback(OnPreviewToggleValueChanged);
        previewToggle.value = terrain.terrainData.enablePreview;
        if (terrain.terrainData.enablePreview)
        {
            CreateAllCellGameObject();
        }


        Toolbar topToolBar = root.Q<Toolbar>("TopToolBar");
        topButtonList = new List<Button>(topToolBar.childCount);
        for (int i  = 0; i < topToolBar.childCount; i++)
        {
            Button button = (Button)topToolBar[i];
            PanelType panelType = (PanelType)i;
            button.clicked += () =>
            {
                ShowPanel(panelType);
            };

            topButtonList.Add(button);
        }
    }

    private void OnPreviewToggleValueChanged(ChangeEvent<bool> evt)
    {
        // toggle的值发生改变
        terrain.terrainData.enablePreview = evt.newValue;

        // 预览模式：绘制地面
        if (terrain.terrainData.enablePreview)
        {
            CreateAllCellGameObject();
        }
        else // 取消预览：清理操作
        {
            terrain.CleanCellsForEditor();
        }
    }
    #endregion

    #region 面板控制
    private PanelType curPanelType;
    private VisualElement panelParent;
    private VisualElement[] panels;

    private void InitPanel()
    {
        panelParent = root.Q<VisualElement>("Panels"); // 获取面板
        panels = new VisualElement[panelParent.childCount];
        for (int i  = 0; i < panelParent.childCount; i++)
        {
            panels[i] = panelParent[i];
        }

        InitTerrainPanel();
        InitSettingPanel();

        ShowPanel((PanelType)terrain.terrainData.terrainEditorPanelIndex);
    }

    private void ShowPanel(PanelType panelType)
    {
        int oldIndex = (int)curPanelType;
        int newIndex = (int)panelType;
        curPanelType = panelType;
        // 菜单部分
        // 先改变旧的按钮的颜色
        topButtonList[oldIndex].style.backgroundColor = normalMenuItemColor;
        terrain.terrainData.terrainEditorPanelIndex = newIndex;
        //terrain.terrainData.Save(); 这样调才切换按钮的时候会卡一下 改成在 OnDisable() 的时候调
        topButtonList[newIndex].style.backgroundColor = selectedMenuItemColor;

        // 面板部分
        // 移除全部面板
        for (int i = panelParent.childCount - 1; i >= 0; i--)
        {
            panelParent.RemoveAt(i);
        }
        // 添加指定面板
        panelParent.Add(panels[newIndex]);

    }
    #endregion

    #region 地形面板
    private VisualElement tilePreviewParent;
    private static List<TilePrefabPreivew> tilePrefabPreviewList = new List<TilePrefabPreivew>();
    private TilePrefabPreivew curTilePrefabPreview;
    private DropdownField operationDropdownField;

    /// <summary>
    /// 初始化地形面板
    /// </summary>
    private void InitTerrainPanel()
    {
        tilePreviewParent = panels[(int)PanelType.Terrain].Q<ScrollView>("TileScrollView").contentContainer;
        operationDropdownField = panels[(int)PanelType.Terrain].Q<DropdownField>("OperationDropdownField");

        InitTilePreview();
    }

    private void InitTilePreview()
    {
        CleanTilePrefabPreview();

        // 根据地块配置列表显示全部地块
        for (int i = 0; i < terrain.terrainTileConfig.tileConfigList.Count; i++)
        {
            TilePrefabPreivew tilePrefabPreview = new TilePrefabPreivew();
            GameObject tilePrefab = terrain.terrainTileConfig.tileConfigList[i].topPrefab;
            tilePrefabPreview.Init(tilePreviewParent, tilePrefab, i, OnSelectTilePreview);
            tilePrefabPreviewList.Add(tilePrefabPreview);
        }
    }

    private void OnSelectTilePreview(TilePrefabPreivew tilePrefabView)
    {
        if (curTilePrefabPreview != null)
        {
            curTilePrefabPreview.OnUnselect();
        }

        curTilePrefabPreview = tilePrefabView;
        curTilePrefabPreview.OnSelect();
    }

    private void CleanTilePrefabPreview()
    {
        for (int i = 0; i < tilePrefabPreviewList.Count; i++)
        {
            tilePrefabPreviewList[i].DoDestroy();
        }

        tilePrefabPreviewList.Clear();
    }

    private void TerrainPanelOnSceneGUI()
    {
        // 当前操作无意义 || 没有选择地块
        if (operationDropdownField.index == 0 || curTilePrefabPreview == null)
        {
            return;
        }

        // 禁止用户选择游戏物体
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        MeshFilter[] meshFilters = terrain.transform.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < meshFilters.Length; i++)
        {
            Ray ray = sceneViewCamera.ScreenPointToRay(GetMousePosition());

            if (IntersectRayMeshTool.IntersectRayMesh(ray, meshFilters[i], out RaycastHit hitInfo))
            {
                //terrain.testMousePosition.Position = hitInfo.point;

                TerrainCell cell = terrain.GetCellByWorldPosition(hitInfo.point);

                if (cell != null)
                {
                    // 添加操作
                    if (operationDropdownField.index == 1)
                    {
                        // 检测是否超过地图边界 && 检测当前选中物体上方是否已存在cell
                        if (cell.CellData.Coordinate.y + 1 < terrain.terrainData.terrainSize.y
                            && terrain.GetTopCell(cell.CellData.Coordinate) == null)
                        {
                            DrawCellGizmos(cell.CellData.Position + new Vector3(0, terrain.terrainData.cellSize + terrain.terrainData.cellSize / 2, 0));
                            if (Event.current.type == EventType.MouseDown)
                            {
                                Vector3Int targetCellCoord = cell.CellData.Coordinate;
                                targetCellCoord.y += 1;
                                OperateCell((CellOperationType)operationDropdownField.index, targetCellCoord, curTilePrefabPreview.configIndex);
                            }
                        }
                    }
                    // 移除操作
                    else if (operationDropdownField.index == 2)
                    {
                        DrawCellGizmos(cell.CellData.Position + new Vector3(0, terrain.terrainData.cellSize / 2, 0));

                        if (Event.current.type == EventType.MouseDown)
                        {
                            // 不允许删除地基
                            if (cell.CellData.Coordinate.y != 0)
                            {
                                OperateCell((CellOperationType)operationDropdownField.index, cell.CellData.Coordinate, curTilePrefabPreview.configIndex);
                            }
                            else
                            {
                                Debug.LogWarning("不允许删除地基");
                            }
                        }
                    }
                    // 替换操作
                    else if (operationDropdownField.index == 3)
                    {
                        DrawCellGizmos(cell.CellData.Position + new Vector3(0, terrain.terrainData.cellSize / 2, 0));

                        if (Event.current.type == EventType.MouseDown)
                        {
                            OperateCell((CellOperationType)operationDropdownField.index, cell.CellData.Coordinate, curTilePrefabPreview.configIndex);
                        }
                    }
                }
                break;
            }
        }
    }

    /// <summary>
    /// 对格子的操作类型
    /// </summary>
    enum CellOperationType
    {
        None,
        Create,
        Remove,
        Replace
    }

    /// <summary>
    /// 对格子进行实际的操作
    /// </summary>
    /// <param name="cellOperationType"></param>
    /// <param name="targetCellCoord"></param>
    /// <param name="cellConfigIndex"></param>
    private void OperateCell(CellOperationType cellOperationType, Vector3Int targetCellCoord, int cellConfigIndex = -1)
    {
        switch (cellOperationType)
        {
            case CellOperationType.Create:
                break;
            case CellOperationType.Remove:
                break;
            case CellOperationType.Replace:
                break;
        }
    }

    #endregion

    #region 设置面板
    private FloatField cellSizeField;
    private Vector3IntField terrainSizeField;
    private float oldCellSizeValue;
    private Vector3Int oldTerrainSizeValue;

    private void InitSettingPanel()
    {
        cellSizeField = panels[(int)PanelType.Setting].Q<FloatField>("CellSize");
        cellSizeField.RegisterCallback<FocusInEvent>(CellSizeFocusIn);
        cellSizeField.RegisterCallback<FocusOutEvent>(CellSizeFocusOut);
        cellSizeField.value = terrain.terrainData.cellSize;

        terrainSizeField = panels[(int)PanelType.Setting].Q<Vector3IntField>("TerrainSize");
        terrainSizeField.RegisterCallback<FocusInEvent>(TerrainSizeFocusIn);
        terrainSizeField.RegisterCallback<FocusOutEvent>(TerrainSizeFocusOut);
        terrainSizeField.value = terrain.terrainData.terrainSize;
    }

    private void CellSizeFocusIn(FocusInEvent evt)
    {
        // 此回调是刚进来的时候 直接将值赋给 oldCellSizeValue
        oldCellSizeValue = cellSizeField.value;
    }

    private void CellSizeFocusOut(FocusOutEvent evt)
    {
        if (oldCellSizeValue != cellSizeField.value) // 发生了变化
        {
            terrain.terrainData.SetCellSize(cellSizeField.value);

            if (terrain.terrainData.enablePreview)
            {
                CreateAllCellGameObject();
            }
        }
    }

    private void TerrainSizeFocusIn(FocusInEvent evt)
    {
        oldTerrainSizeValue = terrainSizeField.value;
    }

    private void TerrainSizeFocusOut(FocusOutEvent evt)
    {
        if (oldTerrainSizeValue != terrainSizeField.value)
        {
            terrain.terrainData.SetTerrainSize(terrainSizeField.value);

            if (terrain.terrainData.enablePreview)
            {
                CreateAllCellGameObject();
            }
        }
    }


    private void CreateAllCellGameObject()
    {
         terrain.CreateAllCell();
         terrain.CreateAllCellGameObjectForEditor();
    }
    #endregion
}
