using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Sirenix.OdinInspector.Editor;
using UnityEditor.UIElements;
using System;

[CustomEditor(typeof(TileTerrain))] // ����: �ӹ�Inspector�Ļ���
public class TileTerrainEditor : OdinEditor
{
    [MenuItem("GameObject/TileTerrain")]
    private static void CreateTileTerrainObject()
    {
        GameObject tileTerrain = new GameObject("TileTerrain");
        tileTerrain.AddComponent<TileTerrain>();
        Selection.activeGameObject = tileTerrain;
    }

    // �ű���帳ֵ
    public VisualTreeAsset editorUIAsset;
    /// <summary>
    /// root ����������
    /// 1. odin���ƵĲ���
    /// 2. ��editorUIAssetʵ���������Ĳ���
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

        // ����һ�µ�ǰ������
        curTileConfig = terrain.terrainTileConfig;
        // ���� ����û�������������
        terrain.SetOnTileConfigSetAction(OnConfigOrDataSet);

        // ������ò����ڵĻ� ֱ�ӷ���odin��Ⱦ���ּ���
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
         
        // ��֤���ƵĿ�����
        if (terrain.terrainTileConfig != null && terrain.terrainData != null)
        {
            editorUIInstance = editorUIAsset.Instantiate();
            root.Add(editorUIInstance);
            // ���� ����û��޸��������е�ϸ��
            terrain.terrainTileConfig.SetOnTileConfigChangedAction(OnTileConfigChanged);

            InitTopMenu(); // ��ʼ�������˵�
            InitPanel(); // ��ʼ�����
        }
        
    }

    // �����õؿ�����
    private void OnConfigOrDataSet()
    {
        // Editor��������ú�Terrain��ǰ���ò���ȣ���ζ���û��޸�������
        // ������һ��Config�ĵ�ֵ�޸�Action
        if (curTileConfig != terrain.terrainTileConfig)
        {
            if (terrain.terrainTileConfig != null && curTileConfig != null) curTileConfig.CleanEditorAction();
            curTileConfig = terrain.terrainTileConfig;
        }
        
        DrawEditor();
    }

    // ���ؿ������޸�ʱҪ��������Ϊ
    private void OnTileConfigChanged()
    {
        // ���»��Ƶ������
        InitTerrainPanel();

        if (terrain.terrainData.enablePreview)
        {
            CreateAllCellGameObject();
        }
    }

    /// <summary>
    /// �ڳ�����ͼ�����á��༭�����������¼� Unity�Դ���
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

    #region ���ߺ���
    private Camera sceneViewCamera => SceneView.currentDrawingSceneView.camera; // �༭��ģʽ��
    private Vector3 GetMousePosition()
    {
        // ��ȡ��Ļ���� ���Ͻ��� 0,0 => ���������Ҫ����Ļ���꣬���½��� 0,0,0
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

    #region �����˵�
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
        // toggle��ֵ�����ı�
        terrain.terrainData.enablePreview = evt.newValue;

        // Ԥ��ģʽ�����Ƶ���
        if (terrain.terrainData.enablePreview)
        {
            CreateAllCellGameObject();
        }
        else // ȡ��Ԥ�����������
        {
            terrain.CleanCellsForEditor();
        }
    }
    #endregion

    #region ������
    private PanelType curPanelType;
    private VisualElement panelParent;
    private VisualElement[] panels;

    private void InitPanel()
    {
        panelParent = root.Q<VisualElement>("Panels"); // ��ȡ���
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
        // �˵�����
        // �ȸı�ɵİ�ť����ɫ
        topButtonList[oldIndex].style.backgroundColor = normalMenuItemColor;
        terrain.terrainData.terrainEditorPanelIndex = newIndex;
        //terrain.terrainData.Save(); ���������л���ť��ʱ��Ῠһ�� �ĳ��� OnDisable() ��ʱ���
        topButtonList[newIndex].style.backgroundColor = selectedMenuItemColor;

        // ��岿��
        // �Ƴ�ȫ�����
        for (int i = panelParent.childCount - 1; i >= 0; i--)
        {
            panelParent.RemoveAt(i);
        }
        // ���ָ�����
        panelParent.Add(panels[newIndex]);

    }
    #endregion

    #region �������
    private VisualElement tilePreviewParent;
    private static List<TilePrefabPreivew> tilePrefabPreviewList = new List<TilePrefabPreivew>();
    private TilePrefabPreivew curTilePrefabPreview;
    private DropdownField operationDropdownField;

    /// <summary>
    /// ��ʼ���������
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

        // ���ݵؿ������б���ʾȫ���ؿ�
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
        // ��ǰ���������� || û��ѡ��ؿ�
        if (operationDropdownField.index == 0 || curTilePrefabPreview == null)
        {
            return;
        }

        // ��ֹ�û�ѡ����Ϸ����
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
                    // ��Ӳ���
                    if (operationDropdownField.index == 1)
                    {
                        // ����Ƿ񳬹���ͼ�߽� && ��⵱ǰѡ�������Ϸ��Ƿ��Ѵ���cell
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
                    // �Ƴ�����
                    else if (operationDropdownField.index == 2)
                    {
                        DrawCellGizmos(cell.CellData.Position + new Vector3(0, terrain.terrainData.cellSize / 2, 0));

                        if (Event.current.type == EventType.MouseDown)
                        {
                            // ������ɾ���ػ�
                            if (cell.CellData.Coordinate.y != 0)
                            {
                                OperateCell((CellOperationType)operationDropdownField.index, cell.CellData.Coordinate, curTilePrefabPreview.configIndex);
                            }
                            else
                            {
                                Debug.LogWarning("������ɾ���ػ�");
                            }
                        }
                    }
                    // �滻����
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
    /// �Ը��ӵĲ�������
    /// </summary>
    enum CellOperationType
    {
        None,
        Create,
        Remove,
        Replace
    }

    /// <summary>
    /// �Ը��ӽ���ʵ�ʵĲ���
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

    #region �������
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
        // �˻ص��Ǹս�����ʱ�� ֱ�ӽ�ֵ���� oldCellSizeValue
        oldCellSizeValue = cellSizeField.value;
    }

    private void CellSizeFocusOut(FocusOutEvent evt)
    {
        if (oldCellSizeValue != cellSizeField.value) // �����˱仯
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
