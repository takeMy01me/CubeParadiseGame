using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

public class TilePrefabPreivew
{
    private static Color normalColor = new Color(0.33f, 0.33f, 0.33f);
    private static Color selectColor = new Color(0.24f, 0.57f, 0.70f);

    private VisualElement parent;
    private GameObject tileObj;
    private IMGUIContainer IMGUIContainer;
    private PreviewRenderUtility previewRenderUtility;

    private Mesh tileMesh;
    private Material tileMaterial;

    private Action<TilePrefabPreivew> clickAction;

    public int configIndex;

    public void Init(VisualElement parent, GameObject tileObj, int configIndex, Action<TilePrefabPreivew> clickAction)
    {
        this.parent = parent;
        this.tileObj = tileObj;
        this.tileMesh = tileObj.GetComponent<MeshFilter>().sharedMesh;
        this.tileMaterial = tileObj.GetComponent<MeshRenderer>().sharedMaterial;
        this.clickAction = clickAction;
        this.configIndex = configIndex;

        IMGUIContainer = new IMGUIContainer();
        SetGUIStyle();
        IMGUIContainer.RegisterCallback<MouseDownEvent>(OnMouseDown);

        previewRenderUtility = new PreviewRenderUtility();
        SetPreviewRender();

        IMGUIContainer.onGUIHandler = DrawTilePreview;

        OnUnselect();
        parent.Add(IMGUIContainer);
    }

    public void OnSelect()
    {
        IMGUIContainer.style.borderLeftColor = selectColor;
        IMGUIContainer.style.borderRightColor = selectColor;
        IMGUIContainer.style.borderTopColor = selectColor;
        IMGUIContainer.style.borderBottomColor = selectColor;
    }

    public void OnUnselect()
    {
        IMGUIContainer.style.borderLeftColor = normalColor;
        IMGUIContainer.style.borderRightColor = normalColor;
        IMGUIContainer.style.borderTopColor = normalColor;
        IMGUIContainer.style.borderBottomColor = normalColor;
    }


    public void DoDestroy()
    {
        if (previewRenderUtility != null)
        {
            previewRenderUtility.Cleanup();
            previewRenderUtility = null;
        }

        this.parent = null;
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        clickAction?.Invoke(this);
    }

    private void DrawTilePreview()
    {
        previewRenderUtility.BeginPreview(IMGUIContainer.contentRect, GUIStyle.none);
        previewRenderUtility.DrawMesh(tileMesh, Vector3.zero, Quaternion.Euler(0, 180f, 0), tileMaterial, 0);
        previewRenderUtility.camera.Render();
        previewRenderUtility.EndAndDrawPreview(IMGUIContainer.contentRect);
    }

    private void SetPreviewRender()
    {
        previewRenderUtility.camera.farClipPlane = 30;
        previewRenderUtility.camera.nearClipPlane = 0.1f;
        previewRenderUtility.camera.clearFlags = CameraClearFlags.Color;
        previewRenderUtility.camera.transform.position = new Vector3(0, 0, -3f);

        previewRenderUtility.lights[0].color = Color.white;
        previewRenderUtility.lights[0].transform.rotation = Quaternion.Euler(55, 0, 0);
        previewRenderUtility.lights[0].intensity = 1;
    }

    private void SetGUIStyle()
    {
        IMGUIContainer.style.width = 35;
        IMGUIContainer.style.height = 35;
        IMGUIContainer.style.marginRight = 2;

        IMGUIContainer.style.borderLeftWidth = 2f;
        IMGUIContainer.style.borderRightWidth = 2f;
        IMGUIContainer.style.borderTopWidth = 2f;
        IMGUIContainer.style.borderBottomWidth = 2f;
    }
}
