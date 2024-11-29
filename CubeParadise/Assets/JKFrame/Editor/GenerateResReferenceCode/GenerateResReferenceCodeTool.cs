
#if ENABLE_ADDRESSABLES
using JKFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class GenerateResReferenceCodeTool
{
    static string scriptPath = Application.dataPath + "/JKFrame//Scripts/2.System/3.Res/R.cs";
    private static string fileStr =
@"using UnityEngine;
using UnityEngine.UI;
using System;
using JKFrame;
namespace R
{
~~��Ա~~
}";
    private static string classTemplate =
@" 
    public static class ##����##
    {
~~��Ա~~
    }";
    private static string PropertyTemplate =
@" 
        public static ##����## ##��Դ����## { get => ResSystem.LoadAsset<##����##>(""##��Դ·��##""); }";
    private static string SubAssetPropertyTemplate =
@"  
        public static ##����## ##��Դ����## { get => ResSystem.LoadAsset<##����##>(""##��Դ·��##""); }";
    private static string GameObjectPropertyTemplate =
@"  
        public static GameObject ##��Դ����##_GameObject(Transform parent = null,string keyName=null,bool autoRelease = true)
        {
            return ResSystem.InstantiateGameObject(""##��Դ·��##"", parent, keyName,autoRelease);
        }";

    public static void CleanResReferenceCode()
    {
        if (File.Exists(scriptPath)) File.Delete(scriptPath);
        Debug.Log("�����Դ����ű��ɹ�");
    }
    public static void GenerateResReferenceCode()
    {
        // ��ʼ����
        Debug.Log("��ʼ������Դ����");
        if (File.Exists(scriptPath)) File.Delete(scriptPath);

        FileStream file = new FileStream(scriptPath, FileMode.CreateNew);
        StreamWriter fileW = new StreamWriter(file, System.Text.Encoding.UTF8);

        // ��ȡȫ��Addressable��Group
        string groupsStr = "";
        AddressableAssetSettings assets = AddressableAssetSettingsDefaultObject.Settings;
        foreach (AddressableAssetGroup group in assets.groups)
        {
            if (group.name == "Built In Data") continue;
            string name = group.name.Replace(" ", "");   // ȥ���ո�
            // ������������
            string groupStr = classTemplate.Replace("##����##", name);

            // �ҵ�����ȫ����Դ�Լ�����
            List<AddressableAssetEntry> allAssetEntry = new List<AddressableAssetEntry>();
            group.GatherAllAssets(allAssetEntry, true, true, true);
            string propertyStrs = "";   // ���Ե��ַ���
            for (int i = 0; i < allAssetEntry.Count; i++)
            {
                AddressableAssetEntry assetItem = allAssetEntry[i];
                if (assetItem.IsSubAsset)   // sub��Դ��Ҫ����[]�޷�����class
                {
                    string subAssetPropertyStr = SubAssetPropertyTemplate.Replace("##����##", assetItem.MainAssetType.Name);
                    string assetName = assetItem.address.Replace("[", "_").Replace("]", ""); // ȥ������Դ�е�����
                    subAssetPropertyStr = subAssetPropertyStr.Replace("##��Դ����##", assetName.Replace(" ", ""));
                    subAssetPropertyStr = subAssetPropertyStr.Replace("##��Դ·��##", assetItem.address);
                    propertyStrs += subAssetPropertyStr;
                }
                else
                {
                    string propertyStr = PropertyTemplate.Replace("##����##", assetItem.MainAssetType.Name);
                    propertyStr = propertyStr.Replace("##��Դ����##", assetItem.address.Replace(" ", ""));
                    propertyStr = propertyStr.Replace("##��Դ·��##", assetItem.address);
                    propertyStrs += propertyStr;
                    if (assetItem.MainAssetType == typeof(GameObject))  // ��Ϸ��������һ������ֱ��ʵ������
                    {
                        string gameObjectPropertyStr = GameObjectPropertyTemplate.Replace("##��Դ����##", assetItem.address.Replace(" ", ""));
                        gameObjectPropertyStr = gameObjectPropertyStr.Replace("##��Դ·��##", assetItem.address);
                        propertyStrs += gameObjectPropertyStr;
                    }
                }
            }
            groupStr = groupStr.Replace("~~��Ա~~", propertyStrs);
            groupsStr += groupStr;
        }
        fileStr = fileStr.Replace("~~��Ա~~", groupsStr);
        fileW.Write(fileStr);
        fileW.Flush();
        fileW.Close();
        file.Close();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        // ��������
        Debug.Log("������Դ����ɹ�");
    }
}
#endif