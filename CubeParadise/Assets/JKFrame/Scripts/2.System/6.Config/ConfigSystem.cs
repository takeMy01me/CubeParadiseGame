using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace JKFrame
{
    /// <summary>
    /// ����ϵͳ
    /// </summary>
    public static class ConfigSystem
    {
#if ENABLE_ADDRESSABLES
        /// <summary>
        /// ��ȡĳ�������µ� �����ļ�
        /// Addressables�е�name��configTypeName_id
        /// </summary>
        public static T GetConfig<T>(string configName) where T : ConfigBase
        {
            return ResSystem.LoadAsset<T>(configName);
        }

        /// <summary>
        /// ��ȡĳһ�����������µ�ȫ������
        /// Addressable��ĳһ��GroupName�µ�ȫ�������ļ�
        /// </summary>
        public static List<T> GetConfigList<T>(string configTypeName) where T : ConfigBase
        {
            return (List<T>)ResSystem.LoadAssets<T>(configTypeName);
        }
#else

        /// <summary>
        /// ��ȡĳһ��·���µ�ȫ������
        /// </summary>
        public static T[] GetConfigs<T>(string path) where T : ConfigBase
        {
            return ResSystem.LoadAssets<T>(path);
        }
        /// <summary>
        /// ��ȡĳһ��·���µ�ȫ������
        /// ����תΪList���ж����������ģ�������ʹ�� GetConfigs<T>()��
        /// </summary>
        public static List<T> GetConfigList<T>(string path) where T : ConfigBase
        {
            return ResSystem.LoadAssets<T>(path).ToList();
        }
#endif
    }
}

