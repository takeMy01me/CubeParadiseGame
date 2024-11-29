using UnityEngine;

namespace JKFrame
{
#if UNITY_EDITOR
    using UnityEditor;
    [InitializeOnLoad]
#endif
    /// <summary>
    /// ��ܸ��ڵ�
    /// </summary>
    public class JKFrameRoot : MonoBehaviour
    {
        private JKFrameRoot() { }
        private static JKFrameRoot Instance;
        public static Transform RootTransform { get; private set; }
        public static JKFrameSetting Setting { get => Instance.FrameSetting; }
        // ��ܲ���������ļ�
        [SerializeField] JKFrameSetting FrameSetting;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            RootTransform = transform;
            DontDestroyOnLoad(gameObject);
            Init();
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        private void Init()
        {
            InitSystems();
        }

        #region System
        private void InitSystems()
        {
            MonoSystem.Init();
            AudioSystem.Init();
            UISystem.Init();
#if ENABLE_LOG
            JKLog.Init(FrameSetting.LogConfig);
#endif
        }

        #endregion

        #region Editor
#if UNITY_EDITOR
        // �༭��ר���¼�ϵͳ
        public static EventModule EditorEventModule;
        static JKFrameRoot()
        {
            EditorEventModule = new EventModule();
            EditorApplication.update += () =>
            {
                InitForEditor();
            };
        }
        [InitializeOnLoadMethod]
        public static void InitForEditor()
        {
            // ��ǰ�Ƿ�Ҫ���в��Ż�׼��������
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<JKFrameRoot>();
                if (Instance == null) return;
                Instance.FrameSetting.InitOnEditor();
                // ���������д��ڶ�����һ��Show
                UI_WindowBase[] window = Instance.transform.GetComponentsInChildren<UI_WindowBase>();
                foreach (UI_WindowBase win in window)
                {
                    win.OnShow();
                }
            }
        }
#endif
        #endregion
    }

}


