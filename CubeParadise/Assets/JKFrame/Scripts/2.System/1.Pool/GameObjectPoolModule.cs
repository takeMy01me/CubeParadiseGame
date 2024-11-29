using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JKFrame
{
    public class GameObjectPoolModule
    {
        #region GameObjectPoolModule���е����ݼ���ʼ������
        // ���ڵ�
        private Transform poolRootTransform;
        /// <summary>
        /// GameObject��������
        /// </summary>
        public Dictionary<string, GameObjectPoolData> poolDic { get; private set; } = new Dictionary<string, GameObjectPoolData>();
        public void Init(Transform poolRootTransform)
        {
            this.poolRootTransform = poolRootTransform;
        }

        /// <summary>
        /// ��ʼ������ز���������
        /// </summary>
        /// <param name="keyName">��Դ����</param>
        /// <param name="maxCapacity">�������ƣ�����ʱ�����ٶ����ǽ������أ�-1��������</param>
        /// <param name="defaultQuantity">Ĭ����������д��������з����Ӧ�����Ķ���0����Ԥ�ȷ���</param>
        /// <param name="prefab">��дĬ������ʱԤ�ȷ���Ķ���</param>
        public void InitObjectPool(string keyName, int maxCapacity = -1, GameObject prefab = null, int defaultQuantity = 0)
        {
            if (defaultQuantity > maxCapacity && maxCapacity != -1)
            {
                JKLog.Error("Ĭ���������������������");
                return;
            }
            //���õĶ�����Ѿ�����
            if (poolDic.TryGetValue(keyName, out GameObjectPoolData poolData))
            {
                //������������
                poolData.maxCapacity = maxCapacity;
                //�ײ�Queue�Զ��������ﲻ��

                //��ָ��Ĭ��������Ĭ�϶���ʱ��������
                if (defaultQuantity > 0)
                {
                    if (prefab.IsNull() == false)
                    {
                        int nowCapacity = poolData.PoolQueue.Count;
                        // ���ɲ�ֵ���������������������
                        for (int i = 0; i < defaultQuantity - nowCapacity; i++)
                        {
                            GameObject go = GameObject.Instantiate(prefab);
                            go.name = prefab.name;
                            PushObject(keyName, go);
                        }
                    }
                    else
                    {
                        JKLog.Error("Ĭ�϶���δָ��");
                    }
                }

            }
            //���õĶ���ز�����
            else
            {
                //���������
                poolData = CreateGameObjectPoolData(keyName, maxCapacity);

                //��ָ��Ĭ��������Ĭ�϶���ʱ��������
                if (defaultQuantity != 0)
                {
                    if (prefab.IsNull() == false)
                    {
                        // �������������������������
                        for (int i = 0; i < defaultQuantity; i++)
                        {
                            GameObject go = GameObject.Instantiate(prefab);
                            go.name = prefab.name;
                            PushObject(keyName, go);
                        }
                    }
                    else
                    {
                        JKLog.Error("Ĭ��������Ĭ�϶���δָ��");
                    }
                }
            }
        }

        /// <summary>
        /// ��ʼ������ز���������
        /// </summary>
        /// <param name="maxCapacity">�������ƣ�����ʱ�����ٶ����ǽ������أ�-1��������</param>
        /// <param name="defaultQuantity">Ĭ����������д��������з����Ӧ�����Ķ���0����Ԥ�ȷ���</param>
        /// <param name="prefab">��дĬ������ʱԤ�ȷ���Ķ���</param>
        public void InitObjectPool(GameObject prefab,int maxCapacity = -1, int defaultQuantity = 0)
        {
            InitObjectPool(prefab.name, maxCapacity, prefab, defaultQuantity);
        }

        /// <summary>
        /// ����һ���µĶ��������
        /// </summary>
        private GameObjectPoolData CreateGameObjectPoolData(string layerName, int maxCapacity = -1)
        {
            //����Object������õ�poolData����
            GameObjectPoolData poolData = PoolSystem.GetObject<GameObjectPoolData>();

            //Object�������û����new
            if (poolData == null) poolData = new GameObjectPoolData(maxCapacity);

            //���õ���poolData�������г�ʼ��������֮ǰ�����ݣ�
            poolData.Init(layerName, poolRootTransform, maxCapacity);
            poolDic.Add(layerName, poolData);
            return poolData;
        }
        #endregion

        #region GameObjectPool��ع���

        public GameObject GetObject(string keyName, Transform parent = null)
        {
            GameObject obj = null;
            // �����û����һ��
            if (poolDic.TryGetValue(keyName, out GameObjectPoolData poolData) && poolData.PoolQueue.Count > 0)
            {
                obj = poolData.GetObj(parent);
            }
            return obj;
        }

        public void PushObject(GameObject go)
        {
            PushObject(go.name, go);
        }
        public bool PushObject(string keyName, GameObject obj)
        {
            // ������û����һ��
            if (poolDic.TryGetValue(keyName, out GameObjectPoolData poolData))
            {
                return poolData.PushObj(obj);
            }
            else
            {
                poolData = CreateGameObjectPoolData(keyName);
                return poolData.PushObj(obj);
            }
        }

        public void Clear(string keyName)
        {
            if (poolDic.TryGetValue(keyName, out GameObjectPoolData gameObjectPoolData))
            {
                gameObjectPoolData.Desotry(true);
                poolDic.Remove(keyName);
            }
        }
        public void ClearAll()
        {
            var enumerator = poolDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.Desotry(false);
            }
            poolDic.Clear();
        }
        #endregion
    }
}