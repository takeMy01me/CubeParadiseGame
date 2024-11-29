#if ENABLE_ADDRESSABLES
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace JKFrame
{
    public static class ResSystem
    {
        /// <summary>
        /// ����Unity��Դ  ��AudioClip Sprite Ԥ����
        /// Ҫע�⣬��Դ����ʹ��ʱ����Ҫ����һ��Release
        /// </summary>
        /// <param name="assetName">��Դ����</param>
        public static T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            return Addressables.LoadAssetAsync<T>(assetName).WaitForCompletion();
        }

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
        /// ��ʼ��һ��GameObject���͵Ķ��������
        /// </summary>
        /// <param name="keyName">��Դ����</param>
        /// <param name="maxCapacity">�������ƣ�����ʱ�����ٶ����ǽ������أ�-1��������</param>
        /// <param name="defaultQuantity">Ĭ����������д��������з����Ӧ�����Ķ���0����Ԥ�ȷ���</param>
        /// <param name="assetName">��Դ����</param>
        public static void InitGameObjectPoolForKeyName(string keyName, int maxCapacity = -1, string assetName = null, int defaultQuantity = 0)
        {
            if (defaultQuantity<=0 || assetName ==null)
            {
                PoolSystem.InitGameObjectPool(keyName, maxCapacity, null, 0);
            }
            else
            {
                GameObject prefab = LoadAsset<GameObject>(assetName);
                PoolSystem.InitGameObjectPool(keyName, maxCapacity, prefab, defaultQuantity);
                UnloadAsset<GameObject>(prefab); // Ԥ�����ͷŵ�������û��Ҫ��
            }
        }

        /// <summary>
        /// ��ʼ������ز���������
        /// </summary>
        /// <param name="maxCapacity">�������ƣ�����ʱ�����ٶ����ǽ������أ�-1��������</param>
        /// <param name="defaultQuantity">Ĭ����������д��������з����Ӧ�����Ķ���0����Ԥ�ȷ���</param>
        /// <param name="assetName">��Դ����</param>
        public static void InitGameObjectPoolForAssetName(string assetName, int maxCapacity = -1, int defaultQuantity = 0)
        {
            InitGameObjectPoolForKeyName(assetName, maxCapacity, assetName, defaultQuantity);
        }


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
        /// ������Ϸ����
        /// ���Զ�����������Ƿ��������������򷵻ض�����е�
        /// </summary>
        /// <param name="assetName">��Դ����</param>
        /// <param name="keyName">������еķ������ƣ���ΪNull</param>
        /// <param name="parent">������</param>
        /// <param name="autoRelease">��������ʱ�����Զ�ȥ����һ��Addressables.Release</param>
        public static GameObject InstantiateGameObject(string assetName, Transform parent = null, string keyName = null, bool autoRelease = true)
        {
            GameObject go ;
            if (keyName == null) go = PoolSystem.GetGameObject(assetName, parent);
            else go = PoolSystem.GetGameObject(keyName,parent);
            if (go.IsNull() == false) return go;
            else
            {
                go = Addressables.InstantiateAsync(assetName, parent).WaitForCompletion();
                if (autoRelease)
                {
                    go.transform.OnReleaseAddressableAsset<int>(AutomaticReleaseAssetAction);
                }
                go.name = assetName;
            }
            return go;
        }

        /// <summary>
        /// ������Ϸ����
        /// ���Զ�����������Ƿ��������������򷵻ض�����е�
        /// </summary>
        /// <param name="assetName">��Դ����</param>
        /// <param name="keyName">������еķ������ƣ���ΪNull</param>
        /// <param name="parent">������</param>
        /// <param name="autoRelease">��������ʱ�����Զ�ȥ����һ��Addressables.Release</param>
        public static T InstantiateGameObject<T>(string assetName, Transform parent = null, string keyName = null,  bool autoRelease = true) where T : Component
        {
            GameObject go = InstantiateGameObject(assetName, parent, keyName, autoRelease);
            if (go.IsNull() == false) return go.GetComponent<T>();
            else return null;
        }

        /// <summary>
        /// �Զ��ͷ���Դ�¼��������¼�����
        /// </summary>
        private static void AutomaticReleaseAssetAction(GameObject obj, int arg)
        {
            Addressables.ReleaseInstance(obj);
        }

        /// <summary>
        /// �첽������Ϸ����
        /// </summary>
        /// <typeparam name="T">�������ϵ����</typeparam>
        /// <param name="assetName">��Դ����</param>
        /// <param name="callBack">ʵ������Ļص�����</param>
        /// <param name="parent">������</param>
        public static void InstantiateGameObjectAsync<T>(string assetName, Action<T> callBack = null, Transform parent = null, string keyName = null, bool autoRelease = true) where T : UnityEngine.Object
        {
            GameObject go;
            if (keyName == null) go = PoolSystem.GetGameObject(assetName, parent);
            else go = PoolSystem.GetGameObject(keyName,parent);
            // ���������
            if (!go.IsNull())
            {
                if (autoRelease) go.transform.OnReleaseAddressableAsset<int>(AutomaticReleaseAssetAction);
                callBack?.Invoke(go.GetComponent<T>());
                return;
            }
            // ��ͨ�������
            MonoSystem.Start_Coroutine(DoLoadGameObjectAsync<T>(assetName, callBack, parent));

        }
        static IEnumerator DoLoadGameObjectAsync<T>(string assetName, Action<T> callBack = null, Transform parent = null,  bool autoRelease = true) where T : UnityEngine.Object
        {
            AsyncOperationHandle<GameObject> request = Addressables.InstantiateAsync(assetName, parent);
            yield return request;
            if (autoRelease) request.Result.transform.OnReleaseAddressableAsset<int>(AutomaticReleaseAssetAction);
            callBack?.Invoke(request.Result.GetComponent<T>());
        }


#endregion

#region ��ϷAsset
        /// <summary>
        /// �첽����Unity��Դ AudioClip Sprite GameObject(Ԥ����)
        /// </summary>
        /// <typeparam name="T">��Դ����</typeparam>
        /// <param name="assetName">��Դ����</param>
        /// <param name="callBack">�ص�����</param>
        public static void LoadAssetAsync<T>(string assetName, Action<T> callBack) where T : UnityEngine.Object
        {
            MonoSystem.Start_Coroutine(DoLoadAssetAsync<T>(assetName, callBack));
        }

        static IEnumerator DoLoadAssetAsync<T>(string assetName, Action<T> callBack) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> request = Addressables.LoadAssetAsync<T>(assetName);
            yield return request;
            callBack?.Invoke(request.Result);
        }

        /// <summary>
        /// ����ָ��Key��������Դ
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="keyName">һ����lable</param>
        /// <param name="callBack">ע�����������ÿһ����Դ�Ļص�</param>
        /// <returns>������Դ</returns>
        public static IList<T> LoadAssets<T>(string keyName, Action<T> callBack = null)
        {
            return Addressables.LoadAssetsAsync<T>(keyName, callBack).WaitForCompletion();
        }

        /// <summary>
        /// ����ָ��Key��������Դ
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="keyName">һ����lable</param>
        /// <param name="callBack">ע�����������ÿһ����Դ�Ļص�</param>
        public static void LoadAssetsAsync<T>(string keyName, Action<IList<T>> callBack = null, Action<T> callBackOnEveryOne = null)
        {
            MonoSystem.Start_Coroutine(DoLoadAssetsAsync<T>(keyName, callBack, callBackOnEveryOne));
        }

        static IEnumerator DoLoadAssetsAsync<T>(string keyName, Action<IList<T>> callBack = null, Action<T> callBackOnEveryOne = null)
        {
            AsyncOperationHandle<IList<T>> request = Addressables.LoadAssetsAsync<T>(keyName, callBackOnEveryOne);
            yield return request;
            callBack?.Invoke(request.Result);
        }

        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="obj">�������</param>
        public static void UnloadAsset<T>(T obj)
        {
            Addressables.Release(obj);
        }

        /// <summary>
        /// ������Ϸ���岢�ͷ���Դ
        /// </summary>
        public static bool UnloadInstance(GameObject obj)
        {
            return Addressables.ReleaseInstance(obj);
        }
#endregion
    }
}
#endif

