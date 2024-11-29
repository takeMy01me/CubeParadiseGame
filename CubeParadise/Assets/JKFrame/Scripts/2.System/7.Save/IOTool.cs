using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace JKFrame
{
    public static class IOTool
    {
        private static BinaryFormatter binaryFormatter = new BinaryFormatter();

        /// <summary>
        /// ����Json
        /// </summary>
        /// <param name="jsonString">Json���ַ���</param>
        /// <param name="path">·��</param>
        public static void SaveJson(string jsonString,string path)
        { 
            File.WriteAllText(path, jsonString);
        }

        /// <summary>
        /// ��ȡJsonΪָ�������Ͷ���
        /// </summary>
        public static T LoadJson<T>(string path) where T : class
        {
            if (!File.Exists(path))
            {
                return null;
            }
            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }

        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="saveObject">����Ķ���</param>
        /// <param name="path">�����·��</param>
        public static void SaveFile(object saveObject, string path)
        {
            FileStream f = new FileStream(path, FileMode.OpenOrCreate);
            // �����Ƶķ�ʽ�Ѷ���д���ļ�
            binaryFormatter.Serialize(f, saveObject);
            f.Dispose();
        }

        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <typeparam name="T">���غ�ҪתΪ������</typeparam>
        /// <param name="path">����·��</param>
        public static T LoadFile<T>(string path) where T : class
        {
            if (!File.Exists(path))
            {
                return null;
            }
            FileStream file = new FileStream(path, FileMode.Open);
            // �����ݽ���ɶ���
            T obj = (T)binaryFormatter.Deserialize(file);
            file.Dispose();
            return obj;
        }


    }
}

