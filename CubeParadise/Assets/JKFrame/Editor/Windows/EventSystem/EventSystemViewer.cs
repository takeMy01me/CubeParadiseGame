using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class EventSystemViewer : EditorWindow
{
    //[MenuItem("JKFrame/EventSystemViewer")]
    //public static void ShowExample()
    //{
    //    //���ɴ���
    //    EventSystemViewer wnd = GetWindow<EventSystemViewer>();
    //    wnd.titleContent = new GUIContent("EventSystemViewer");
    //}

    public void CreateGUI()
    {
        //��ȡ��ǰ���ڵĸ�UIԪ������
        VisualElement root = rootVisualElement;

        // ����UXML���Ѷ�������visualTreeʵ����
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/JKFrame/Editor/Windows/EventSystem/EventSystemViewer.uxml");
        VisualElement elementUXML = visualTree.Instantiate();

        // ����USS��ʽ���󶨵�ʵ����������,��ӵ�UI��Ԫ��������
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/JKFrame/Editor/Windows/EventSystem/EventSystemViewer.uss");
        elementUXML.styleSheets.Add(styleSheet);
        root.Add(elementUXML);

    }
}