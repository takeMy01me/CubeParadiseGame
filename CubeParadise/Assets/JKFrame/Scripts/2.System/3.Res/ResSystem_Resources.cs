#if ENABLE_ADDRESSABLES == false
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

namespace JKFrame
{
    public static class ResSystem
    {
        // ��Ҫ��������� 
        // Key��TypeNameOrAssetName
        // Value���Ƿ񻺴�
        private static Dictionary<string, bool> wantCacheDic;

        #region ��ͨclass����
        /// <summary>
        /// ��ȡʵ��-��ͨClass
        /// ���������Ҫ���棬��Ӷ�����л�ȡ
        /// ��������û�л򷵻�null
        /// </summary>
        public static T Get<T>() where T : class
        {
            return PoolSystem.GetObject<T>();
        }

        /// <summary>
        /// ��ȡʵ��-��ͨClass
        /// ���������Ҫ���棬��Ӷ�����л�ȡ
        /// ��������û�л򷵻�null
        /// </summary>
        /// <param name="keyName">������е�����</param>
        public static T Get<T>(string keyName) where T : class
        {
            return PoolSystem.GetObject<T>(keyName);
        }
        /// <summary>
        /// ��ȡʵ��-��ͨClass
        /// ���������Ҫ���棬��Ӷ�����л�ȡ
        /// ��������û�л�newһ������
        /// <summary>
        public static T GetOrNew<T>() where T : class, new()
        {
            T obj = PoolSystem.GetObject<T>();
            if (obj == null) obj = new T();
            return obj;
        }


        /// <summary>
        /// ��ȡʵ��-��ͨClass
        /// ���������Ҫ���棬��Ӷ�����л�ȡ
        /// ��������û�л�newһ������
        /// <summary>
        /// <param name="keyName">������е�����</param>
        public static T GetOrNew<T>(string keyName) where T : class, new()
        {
            T obj = PoolSystem.GetObject<T>(keyName);
            if (obj == null) obj = new T();
            return obj;
        }

        /// <summary>
        /// ж����ͨ����������ʹ�ö���صķ�ʽ
        /// </summary>
        public static void PushObjectInPool(System.Object obj)
        {
            PoolSystem.PushObject(obj);
        }

        /// <summary>
        /// ��ͨ���󣨷�GameObject�����ö������
        /// ����KeyName���
        /// </summary>
        public static void PushObjectInPool(object obj, string keyName)
        {
            PoolSystem.PushObject(obj, keyName);
        }

        /// <summary>
        /// ��ʼ������ز���������
        /// </summary>
        /// <param name="keyName">��Դ����</param>
        /// <param name="maxCapacity">�������ƣ�����ʱ�����ٶ����ǽ������أ�-1��������</param>
        /// <param name="defaultQuantity">Ĭ����������д��������з����Ӧ�����Ķ���0����Ԥ�ȷ���</param>
        public static void InitObjectPool<T>(string keyName, int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            PoolSystem.InitObjectPool<T>(keyName, maxCapacity, defaultQuantity);
        }
        /// <summary>
        /// ��ʼ������ز���������
        /// </summary>
        /// <param name="maxCapacity">�������ƣ�����ʱ�����ٶ����ǽ������أ�-1��������</param>
        /// <param name="defaultQuantity">Ĭ����������д��������з����Ӧ�����Ķ���0����Ԥ�ȷ���</param>
        public static void InitObjectPool<T>(int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            PoolSystem.InitObjectPool<T>(maxCapacity, defaultQuantity);
        }
        /// <summary>
        /// ��ʼ��һ����ͨC#���������
        /// </summary>
        /// <param name="keyName">keyName</param>
        /// <param name="maxCapacity">����������ʱ�ᶪ�������ǽ������أ�-1��������</param>
        public static void InitObjectPool(string keyName, int maxCapacity = -1)
        {
            PoolSystem.InitObjectPool(keyName, maxCapacity);
        }

        /// <summary>
        /// ��ʼ�������
        /// </summary>
        /// <param name="type">��Դ����</param>
        /// <param name="maxCapacity">�������ƣ�����ʱ�����ٶ����ǽ������أ�-1��������</param>
        public static void InitObjectPool(System.Type type, int maxCapacity = -1)
        {
            PoolSystem.InitObjectPool(type, maxCapacity);
        }

        #endregion

        #region ��Ϸ����

        /// <summary>
        /// ж����Ϸ����������ʹ�ö���صķ�ʽ
        /// </summary>
        public static void PushGameObjectInPool(GameObject gameObject)
        {
            PoolSystem.PushGameObject(gameObject);
        }

        /// <summary>
        /// ��Ϸ������ö������
        /// </summary>
        /// <param name="keyName">������е�key</param>
        /// <param name="obj">���������</param>
        public static void PushGameObjectInPool(string keyName, GameObject gameObject)
        {
            PoolSystem.PushGameObject(keyName, gameObject);
        }


        /// <summary>
        /// ��ʼ��һ��GameObject���͵Ķ��������
        /// </summary>
        /// <param name="keyName">������еı�ʶ</param>
        /// <param name="maxCapacity">�������ƣ�����ʱ�����ٶ����ǽ������أ�-1��������</param>
        /// <param name="defaultQuantity">Ĭ����������д��������з����Ӧ�����Ķ���0����Ԥ�ȷ���</param>
        /// <param name="prefab">��дĬ������ʱԤ�ȷ���Ķ���</param>
        public static void InitGameObjectPool(string keyName, int maxCapacity = -1, string assetPath = null, int defaultQuantity = 0)
        {
            GameObject prefab = LoadAsset<GameObject>(assetPath);
            PoolSystem.InitGameObjectPool(keyName, maxCapacity, prefab, defaultQuantity);
            UnloadAsset(prefab);
        }

        /// <summary>
        /// ��ʼ������ز���������
        /// </summary>
        /// <param name="maxCapacity">�������ƣ�����ʱ�����ٶ����ǽ������أ�-1��������</param>
        /// <param name="defaultQuantity">Ĭ����������д��������з����Ӧ�����Ķ���0����Ԥ�ȷ���</param>
        /// <param name="assetPath">��Դ·��</param>
        public static void InitGameObjectPool(string assetPath, int maxCapacity = -1, int defaultQuantity = 0)
        {
            GameObject prefab = LoadAsset<GameObject>(assetPath);
            PoolSystem.InitGameObjectPool(prefab, maxCapacity, defaultQuantity);
            UnloadAsset(prefab);

        }

        /// <summary>
        /// ������Ϸ����
        /// </summary>
        public static void UnloadInstance(GameObject obj)
        {
            GameObject.Destroy(obj);
        }
        /// <summary>
        /// ������Ϸ����
        /// ���Զ������Ƿ��ڶ�����д���
        /// </summary>
        /// <param name="assetPath">��Դ·��</param>
        /// <param name="parent">������</param>
        public static GameObject InstantiateGameObject(string assetPath, Transform parent = null,string keyName=null)
        {
            string assetName = GetAssetNameByPath(assetPath);
            GameObject go;
            if (keyName == null) go = PoolSystem.GetGameObject(assetName, parent);
            else go = PoolSystem.GetGameObject(keyName, parent);
            if (!go.IsNull()) return go;

            GameObject prefab = LoadAsset<GameObject>(assetPath);
            if (!prefab.IsNull())
            {
                go = GameObject.Instantiate(prefab, parent);
                go.name = assetName;
                UnloadAsset(prefab);
            }
            return go;
        }

        /// <summary>
        /// ������Ϸ���岢��ȡ���
        /// </summary>
        /// <param name="path">��Դ·��</param>
        /// <param name="parent">������</param>
        public static T InstantiateGameObject<T>(string path, Transform parent = null, string keyName = null) where T : UnityEngine.Component
        {
            GameObject go = InstantiateGameObject(path, parent, keyName);
            if (!go.IsNull())
            {
                return go.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// �첽ʵ������Ϸ����
        /// </summary>
        public static void InstantiateGameObjectAsync(string path, Action<GameObject> callBack = null, Transform parent = null, string keyName = null)
        {
            // �и�·����ȡʵ�ʵ���Դ����
            string assetName = GetAssetNameByPath(path);
            GameObject go;
            if (keyName == null) go = PoolSystem.GetGameObject(assetName, parent);
            else go = PoolSystem.GetGameObject(keyName, parent);
            // �������
            if (!go.IsNull())
            {
                callBack?.Invoke(go);
                return;
            }
            // ��ͨ�������
            MonoSystem.Start_Coroutine(DoInstantiateGameObjectAsync(path, callBack, parent));
        }


        /// <summary>
        /// �첽ʵ������Ϸ���岢��ȡ���
        /// </summary>
        /// <typeparam name="T">��Ϸ�����ϵ����</typeparam>
        public static void InstantiateGameObjectAsync<T>(string path, Action<T> callBack = null, Transform parent = null, string keyName = null) where T : UnityEngine.Component
        {
            string assetName = GetAssetNameByPath(path);
            // �����ֵ�������
            GameObject go;
            if (keyName == null) go = PoolSystem.GetGameObject(assetName, parent);
            else go = PoolSystem.GetGameObject(keyName, parent);
            // ������
            if (!go.IsNull())
            {
                callBack?.Invoke(go.GetComponent<T>());
                return;
            }
            // ��ͨ�������
            MonoSystem.Start_Coroutine(DoInstantiateGameObjectAsync<T>(path, callBack, parent));
        }

        static IEnumerator DoInstantiateGameObjectAsync(string path, Action<GameObject> callBack = null, Transform parent = null)
        {
            ResourceRequest resourceRequest = Resources.LoadAsync<GameObject>(path);
            yield return resourceRequest;
            GameObject prefab = resourceRequest.asset as GameObject;
            GameObject go = GameObject.Instantiate<GameObject>(prefab);
            go.name = prefab.name;
            UnloadAsset(prefab);
            callBack?.Invoke(go);
        }
        static IEnumerator DoInstantiateGameObjectAsync<T>(string path, Action<T> callBack = null, Transform parent = null) where T : UnityEngine.Object
        {
            ResourceRequest resourceRequest = Resources.LoadAsync<GameObject>(path);
            yield return resourceRequest;
            GameObject prefab = resourceRequest.asset as GameObject;
            GameObject go = GameObject.Instantiate<GameObject>(prefab);
            go.name = prefab.name;
            UnloadAsset(prefab);
            callBack?.Invoke(go.GetComponent<T>());
        }
        #endregion
        #region ��ϷAsset
        /// <summary>
        /// ����Unity��Դ  ��AudioClip Sprite Ԥ����
        /// </summary>
        /// <typeparam name="T">��Դ����</typeparam>
        public static T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            return Resources.Load<T>(path);
        }

        /// <summary>
        /// ͨ��path��ȡ��Դ����
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetAssetNameByPath(string path)
        {
            return path.Substring(path.LastIndexOf("/") + 1);
        }
        /// <summary>
        /// �첽����Unity��Դ AudioClip Sprite GameObject(Ԥ����)
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="path">��Դ·��</param>
        /// <param name="callBack">������ɺ�Ļص�</param>
        public static void LoadAssetAsync<T>(string path, Action<T> callBack) where T : UnityEngine.Object
        {
            MonoSystem.Start_Coroutine(DoLoadAssetAsync<T>(path, callBack));
        }

        static IEnumerator DoLoadAssetAsync<T>(string path, Action<T> callBack) where T : UnityEngine.Object
        {
            ResourceRequest resourceRequest = Resources.LoadAsync<T>(path);
            yield return resourceRequest;
            callBack?.Invoke(resourceRequest.asset as T);
        }

        /// <summary>
        /// ����ָ��·����������Դ
        /// </summary>
        public static UnityEngine.Object[] LoadAssets(string path)
        {
            return Resources.LoadAll(path);
        }

        /// <summary>
        /// ����ָ��·����������Դ
        /// </summary>
        public static T[] LoadAssets<T>(string path) where T : UnityEngine.Object
        {
            return Resources.LoadAll<T>(path);
        }

        /// <summary>
        /// ж����Դ
        /// </summary>
        public static void UnloadAsset(UnityEngine.Object assetToUnload)
        {
            Resources.UnloadAsset(assetToUnload);
        }



        /// <summary>
        /// ж��ȫ��δʹ�õ���Դ
        /// </summary>
        public static void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }
        #endregion


    }
}
#endif