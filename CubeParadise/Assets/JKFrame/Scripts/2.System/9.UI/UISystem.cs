using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JKFrame
{
    /// <summary>
    /// UI���ڵ�
    /// </summary>
    public class UISystem : MonoBehaviour
    {
        private static UISystem instance;
        public static void Init()
        {
            instance = JKFrameRoot.RootTransform.GetComponentInChildren<UISystem>();
        }
        #region �ڲ���
        [Serializable]
        private class UILayer
        {
            public Transform root;
            public bool enableMask = true;
            public Image maskImage;
            private int count = 0;
            public void OnWindowShow()
            {
                count += 1;
                Update();
            }
            public void OnWindowClose()
            {
                count -= 1;
                Update();
            }
            private void Update()
            {
                if (enableMask == false) return;
                maskImage.raycastTarget = count != 0;
                int posIndex = root.childCount - 2;
                maskImage.transform.SetSiblingIndex(posIndex < 0 ? 0 : posIndex);
            }
        }
        #endregion

        private static Dictionary<string, UIWindowData> UIWindowDataDic => JKFrameRoot.Setting.UIWindowDataDic;
        [SerializeField] private UILayer[] uiLayers;
        [SerializeField] private RectTransform dragLayer;
        /// <summary>
        /// ��ק�㣬λ������UI�����ϲ�
        /// </summary>
        public static RectTransform DragLayer { get => instance.dragLayer; }
        private static UILayer[] UILayers { get => instance.uiLayers; }

        [SerializeField] GameObject UITipsItemPrefab;
        [SerializeField] private RectTransform UITipsItemParent;

        #region ��������
        // UIϵͳ�Ĵ�����������Ҫ������Ԥ����·�����Ƿ񻺴桢��ǰ���ڶ���ʵ������Ҫ��Ϣ
        // Ϊ�˷���ʹ�ã����Դ������ݱ����ȴ����UIWindowDataDic�У�����ͨ��UIϵͳ��ʾ���رյ�

        /// <summary>
        /// ��ʼ��UIԪ������
        /// ִֻ��OnInit����ִ��OnShow
        /// ���Զ�SetActive(false)
        /// </summary>
        /// <param name="windowKey">�Զ�������ƣ���������Դ·�����������ƻ������Զ���</param>
        /// <param name="windowData">���ڵ���Ҫ����</param>
        /// <param name="instantiateAtOnce">�Ƿ�����ʵ������ǰ�����л����Ҫ</param>
        public static void AddUIWindowData(string windowKey, UIWindowData windowData, bool instantiateAtOnce = false)
        {
            if (UIWindowDataDic.TryAdd(windowKey, windowData))
            {
                if (instantiateAtOnce)
                {
                    if (windowData.isCache)
                    {
                        UI_WindowBase window = ResSystem.InstantiateGameObject<UI_WindowBase>(windowKey, UILayers[windowData.layerNum].root);
                        window.Init();
                        window.gameObject.SetActive(false);
                    }
                    else
                    {
                        JKLog.Warning("JKFrame:UIWindowData�е�isCache=false����instantiateAtOnce=true!��ǰʵ�������ڲ���Ҫ����Ĵ�����˵û������");
                    }
                }
            }
        }

        /// <summary>
        /// ��ʼ��UIԪ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="windowData"></param>
        /// <param name="instantiateAtOnce">
        /// ����ʵ���������ǣ�
        /// ִֻ��OnInit����ִ��OnShow
        /// ���Զ�SetActive(false)
        /// </param>
        public static void AddUIWindowData(Type type, UIWindowData windowData, bool instantiateAtOnce = false)
        {
            AddUIWindowData(type.FullName, windowData, instantiateAtOnce);
        }

        /// <summary>
        /// ��ʼ��UIԪ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="windowData"></param>
        /// <param name="instantiateAtOnce">
        /// ����ʵ���������ǣ�
        /// ִֻ��OnInit����ִ��OnShow
        /// ���Զ�SetActive(false)
        /// </param>
        public static void AddUIWindowData<T>(UIWindowData windowData, bool instantiateAtOnce = false)
        {
            AddUIWindowData(typeof(T), windowData, instantiateAtOnce);
        }

        /// <summary>
        /// ��ȡUI��������
        /// </summary>
        /// <param name="windowKey"></param>
        /// <returns>����ΪNull</returns>
        public static UIWindowData GetUIWindowData(string windowKey)
        {
            if (UIWindowDataDic.TryGetValue(windowKey, out UIWindowData windowData))
            {
                return windowData;
            }
            return null;
        }

        /// <summary>
        /// ���Ի�ȡUI��������
        /// </summary>
        /// <param name="windowKey"></param>
        public static bool TryGetUIWindowData(string windowKey, out UIWindowData windowData)
        {
            return UIWindowDataDic.TryGetValue(windowKey, out windowData);
        }

        /// <summary>
        /// �Ƴ�UI��������
        /// </summary>
        /// <param name="windowKey"></param>
        /// <param name="destoryWidnow">������ڴ��ڣ������ٵĴ��ڵ�ʵ��</param>
        /// <returns></returns>
        public static bool RemoveUIWindowData(string windowKey, bool destoryWidnow = false)
        {
            if (TryGetUIWindowData(windowKey, out UIWindowData windowData))
            {
                if (windowData.instance != null)
                {
                    Destroy(windowData.instance.gameObject);
                }
            }
            return UIWindowDataDic.Remove(windowKey);
        }

        /// <summary>
        /// �������UI��������
        /// </summary>
        public static void ClearUIWindowData()
        {
            var enumerator = UIWindowDataDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Destroy(enumerator.Current.Value.instance.gameObject);
            }
            UIWindowDataDic.Clear();
        }
        #endregion

        #region ����
        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="layer">�㼶 -1���ڲ�����</param>
        public static T Show<T>(int layer = -1) where T : UI_WindowBase
        {
            return Show(typeof(T), layer) as T;
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <typeparam name="T">Ҫ���صĴ�������</typeparam>
        /// <param name="windowKey">���ڵ�Key</param>
        /// <param name="layer">�㼶 -1���ڲ�����</param>
        public static T Show<T>(string windowKey, int layer = -1) where T : UI_WindowBase
        {
            return Show(windowKey, layer) as T;
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="type">��������</param>
        /// <param name="layer">�㼶 -1���ڲ�����</param>
        public static UI_WindowBase Show(Type type, int layer = -1)
        {
            return Show(type.FullName, layer);
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="windowKey">���ڵ�key</param>
        /// <param name="layer">�㼶 -1���ڲ�����</param>
        public static UI_WindowBase Show(string windowKey, int layer = -1)
        {
            if (UIWindowDataDic.TryGetValue(windowKey, out UIWindowData windowData))
            {
                return Show(windowData, layer);
            }
            // ��Դ����û����ζ�Ų�������ʾ
            JKLog.Log($"JKFrame:������{windowKey}��UIWindowData");
            return null;
        }

        private static UI_WindowBase Show(UIWindowData windowData, int layer = -1)
        {
            int layerNum = layer == -1 ? windowData.layerNum : layer;
            // ʵ����ʵ�����߻�ȡ��ʵ������֤����ʵ������
            if (windowData.instance != null)
            {
                windowData.instance.gameObject.SetActive(true);
                windowData.instance.transform.SetParent(UILayers[layerNum].root);
                windowData.instance.transform.SetAsLastSibling();
                windowData.instance.OnShow();
            }
            else
            {
                UI_WindowBase window = ResSystem.InstantiateGameObject<UI_WindowBase>(windowData.assetPath, UILayers[layerNum].root);
                windowData.instance = window;
                window.Init();
                window.OnShow();
            }
            windowData.layerNum = layerNum;
            UILayers[layerNum].OnWindowShow();
            return windowData.instance;
        }
        #endregion

        #region ��ȡ�����ٴ���
        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="windowKey">����Key</param>
        /// <returns>û�ҵ���ΪNull</returns>
        public static UI_WindowBase GetWindow(string windowKey)
        {
            if (UIWindowDataDic.TryGetValue(windowKey, out UIWindowData windowData))
            {
                return windowData.instance;
            }
            return null;
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="windowKey">����Key</param>
        /// <returns>û�ҵ���ΪNull</returns>
        public static T GetWindow<T>(string windowKey) where T : UI_WindowBase
        {
            return GetWindow(windowKey) as T;
        }


        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <returns>û�ҵ���ΪNull</returns>
        public static T GetWindow<T>() where T : UI_WindowBase
        {
            return GetWindow(typeof(T).FullName) as T;
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <returns>û�ҵ���ΪNull</returns>
        public static UI_WindowBase GetWindow(Type windowType)
        {
            return GetWindow(windowType.FullName);
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="windowKey">����Key</param>
        /// <returns>û�ҵ���ΪNull</returns>
        public static T GetWindow<T>(Type windowType) where T : UI_WindowBase
        {
            return GetWindow(windowType.FullName) as T;
        }

        /// <summary>
        /// ���Ի�ȡ����
        /// </summary>
        /// <param name="windowKey"></param>
        public static bool TryGetWindow(string windowKey, out UI_WindowBase window)
        {
            UIWindowDataDic.TryGetValue(windowKey, out UIWindowData windowData);
            window = windowData.instance;
            return window != null;
        }

        /// <summary>
        /// ���Ի�ȡ����
        /// </summary>
        /// <param name="windowKey"></param>
        public static bool TryGetWindow<T>(string windowKey, out T window) where T : UI_WindowBase
        {
            UIWindowDataDic.TryGetValue(windowKey, out UIWindowData windowData);
            window = windowData.instance as T;
            return window != null;
        }

        /// <summary>
        /// ���ٴ���
        /// </summary>
        public static void DestroyWindow(string windowKey)
        {
            UI_WindowBase window = GetWindow(windowKey);
            if (window != null)
            {
                DestroyImmediate(window.gameObject);
            }
        }
        #endregion

        #region �رմ���
        /// <summary>
        /// �رմ���
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        public static void Close<T>()
        {
            Close(typeof(T));
        }

        /// <summary>
        /// �رմ���
        /// </summary>
        /// <typeparam name="Type">��������</typeparam>
        public static void Close(Type type)
        {
            Close(type.FullName);
        }

        /// <summary>
        /// �رմ���
        /// </summary>
        /// <param name="windowKey"></param>
        public static void Close(string windowKey)
        {
            if (TryGetUIWindowData(windowKey, out UIWindowData windowData))
            {
                if (windowData.instance != null)
                {
                    windowData.instance.OnClose();
                    // ����������
                    if (windowData.isCache)
                    {
                        windowData.instance.transform.SetAsFirstSibling();
                        windowData.instance.gameObject.SetActive(false);
                    }
                    // ������������
                    else
                    {
#if ENABLE_ADDRESSABLES
                        ResSystem.UnloadInstance(windowData.instance.gameObject);
#endif
                        DestroyImmediate(windowData.instance.gameObject);
                        windowData.instance = null;
                    }
                    UILayers[windowData.layerNum].OnWindowClose();
                }
                else JKLog.Warning("JKFrame:����Ҫ�رյĴ��ڲ�����");
            }
            else JKLog.Warning("JKFrame:δ��ѯ��UIWindowData");
        }

        /// <summary>
        /// �ر�ȫ������
        /// </summary>
        public static void CloseAllWindow()
        {
            // ������������״̬���߼�
            var enumerator = UIWindowDataDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.instance != null
                    && enumerator.Current.Value.instance.gameObject.activeInHierarchy == true)
                {
                    enumerator.Current.Value.instance.Close();
                }
            }
        }
        #endregion

        #region UITips
        public static void AddTips(string tips)
        {
            UITipsItem item = PoolSystem.GetGameObject<UITipsItem>(instance.UITipsItemPrefab.name, instance.UITipsItemParent);
            if (item == null) item = GameObject.Instantiate(instance.UITipsItemPrefab, instance.UITipsItemParent).GetComponent<UITipsItem>();
            item.Init(tips);
        }
        #endregion

    }
}
