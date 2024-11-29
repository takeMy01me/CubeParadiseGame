using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using System;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "TileConfig", menuName = "TileTerrain/TileConfig")]
public class TileTerrainTileConfig : ConfigBase
{
    [ListDrawerSettings(ShowIndexLabels = true, ShowPaging = false)]
    [OnValueChanged("OnTileConfigChanged")]
    public List<TerrainTileConfigItem> tileConfigList = new List<TerrainTileConfigItem>();


#if UNITY_EDITOR
    private Action onTileConfigChangedAction;
    public void SetOnTileConfigChangedAction(Action onTileConfigChangedAction)
    {
        this.onTileConfigChangedAction = onTileConfigChangedAction;
    }

    public void CleanEditorAction()
    {
        onTileConfigChangedAction = null;
    }

    private void OnTileConfigChanged()
    {
        onTileConfigChangedAction?.Invoke();
    }
#endif
}


/// <summary>
/// 一类地块的配置
/// </summary>
[Serializable]
public class TerrainTileConfigItem
{
#if UNITY_EDITOR
    public string name;
#endif
    public GameObject forwardPrefab;
    public GameObject backPrefab;
    public GameObject leftPrefab;
    public GameObject rightPrefab;
    public GameObject topPrefab;
    //public GameObject bottomPrefab; // 俯视角游戏，不需要底部
}
