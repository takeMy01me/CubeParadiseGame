using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class Test : MonoBehaviour
{
    private List<TestData> testList;
    void Start()
    {
        testList = new List<TestData>()
        {
            new TestData(0, "��"),
            new TestData(1, "��"),
            new TestData(2, "��"),
            new TestData(3, "��"),
            new TestData(4, "��"),
            new TestData(5, "ͩ"),
        };

        var t = testList.First( t => t.Name == "��");

        Debug.Log($"t's info: Index: {t.Index}, Name: {t.Name}");
        
    }

}

public class TestData
{
    public int Index;
    public string Name;

    public TestData(int index, string name)
    {
        this.Index = index;
        this.Name = name;
    }
}
