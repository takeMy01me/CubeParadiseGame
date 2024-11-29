using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JKFrame
{
    public class ObjectPoolModule
    {
        #region ObjectPoolModule���е����ݼ���ʼ������
        /// <summary>
        /// ��ͨ�� ��������
        /// </summary>
        public Dictionary<string, ObjectPoolData> poolDic { get; private set; } = new Dictionary<string, ObjectPoolData>();

        /// <summary>
        /// ��ʼ������ز���������
        /// </summary>
        /// <param name="maxCapacity">�������ƣ�����ʱ�����ٶ����ǽ������أ�-1��������</param>
        /// <param name="defaultQuantity">Ĭ����������д��������з����Ӧ�����Ķ���0����Ԥ�ȷ���</param>
        public void InitObjectPool<T>(string keyName, int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            //���õĶ�����Ѿ�����
            if (poolDic.TryGetValue(keyName, out ObjectPoolData poolData))
            {
                //������������
                poolData.maxCapacity = maxCapacity;
                //�ײ�Queue�Զ��������ﲻ��

                //��ָ��Ĭ������ʱ��������
                if (defaultQuantity != 0)
                {
                    int nowCapacity = poolData.PoolQueue.Count;
                    // ���ɲ�ֵ���������������������
                    for (int i = 0; i < defaultQuantity - nowCapacity; i++)
                    {
                        T obj = new T();
                        PushObject(obj, keyName);
                    }
                }

            }
            //���õĶ���ز�����
            else
            {
                //���������
                poolData = CreateObjectPoolData(keyName, maxCapacity);

                //��ָ��Ĭ��������Ĭ�϶���ʱ��������
                if (defaultQuantity != 0)
                {
                    // �������������������������
                    for (int i = 0; i < defaultQuantity; i++)
                    {
                        T obj = new T();
                        PushObject(obj, keyName);
                    }
                }
            }
        }

        /// <summary>
        /// ��ʼ������ز���������
        /// </summary>
        /// <param name="maxCapacity">�������ƣ�����ʱ�����ٶ����ǽ������أ�-1��������</param>
        /// <param name="defaultQuantity">Ĭ����������д��������з����Ӧ�����Ķ���0����Ԥ�ȷ���</param>
        public void InitObjectPool<T>(int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            InitObjectPool<T>(typeof(T).FullName, maxCapacity, defaultQuantity);
        }

        /// <summary>
        /// ��ʼ�������
        /// </summary>
        /// <param name="keyName">��Դ����</param>
        /// <param name="maxCapacity">�������ƣ�����ʱ�����ٶ����ǽ������أ�-1��������</param>
        public void InitObjectPool(string keyName, int maxCapacity = -1)
        {
            //���õĶ�����Ѿ�����
            if (poolDic.TryGetValue(keyName, out ObjectPoolData poolData))
            {
                //������������
                poolData.maxCapacity = maxCapacity;
                //�ײ�Queue�Զ��������ﲻ��
            }
            //���õĶ���ز�����
            else
            {
                //���������
                CreateObjectPoolData(keyName, maxCapacity);
            }
        }
        /// <summary>
        /// ��ʼ�������
        /// </summary>
        /// <param name="type">��Դ����</param>
        /// <param name="maxCapacity">�������ƣ�����ʱ�����ٶ����ǽ������أ�-1��������</param>
        public void InitObjectPool(System.Type type, int maxCapacity = -1)
        {
            InitObjectPool(type.FullName, maxCapacity);
        }

        /// <summary>
        /// ����һ���µĶ��������
        /// </summary>
        private ObjectPoolData CreateObjectPoolData(string layerName, int capacity = -1)
        {
            // ����Object������õ�poolData����
            ObjectPoolData poolData = this.GetObject<ObjectPoolData>();

            //Object�������û����new
            if (poolData == null)
            {
                poolData = new ObjectPoolData(capacity);
            }

            //���õ���poolData�������г�ʼ��������֮ǰ�����ݣ�
            poolData.maxCapacity = capacity;
            poolDic.Add(layerName, poolData);
            return poolData;
        }
        #endregion
        #region ObjectPool��ع���
        public object GetObject(string keyName)
        {
            object obj = null;
            if (poolDic.TryGetValue(keyName, out ObjectPoolData objectPoolData) && objectPoolData.PoolQueue.Count > 0)
            {
                obj = poolDic[keyName].GetObj();
            }
            return obj;
        }

        public object GetObject(System.Type type)
        {
            return GetObject(type.FullName);
        }

        public T GetObject<T>() where T : class
        {
            return (T)GetObject(typeof(T));
        }

        public T GetObject<T>(string keyName) where T : class
        {
            return (T)GetObject(keyName);
        }

        public bool PushObject(object obj)
        {
            return PushObject(obj, obj.GetType().FullName);
        }
        public bool PushObject(object obj, string keyName)
        {
            if (poolDic.TryGetValue(keyName, out ObjectPoolData poolData) == false)
            {
                poolData = CreateObjectPoolData(keyName);
            }
            return poolData.PushObj(obj);
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
        public void ClearObject<T>()
        {
            ClearObject(typeof(T).FullName);
        }
        public void ClearObject(System.Type type)
        {
            ClearObject(type.FullName);
        }

        public void ClearObject(string keyName)
        {
            if (poolDic.TryGetValue(keyName, out ObjectPoolData objectPoolData))
            {
                objectPoolData.Desotry(true);
                poolDic.Remove(keyName);
            }
        }
    }
    #endregion

}