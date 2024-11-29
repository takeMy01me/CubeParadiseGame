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
            new TestData(0, "°×"),
            new TestData(1, "ºú"),
            new TestData(2, "Óñ"),
            new TestData(3, "¾¸"),
            new TestData(4, "Áú"),
            new TestData(5, "Í©"),
        };

        var t = testList.First( t => t.Name == "ºú");

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
