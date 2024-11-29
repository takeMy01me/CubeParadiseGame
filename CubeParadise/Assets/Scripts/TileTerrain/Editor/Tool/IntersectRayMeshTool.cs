using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


using System;
using System.Linq;
using System.Reflection;

internal class IntersectRayMeshTool
{
    private static Type handleUtilityType;
    private static MethodInfo rayMeshMethodInfo;

    static IntersectRayMeshTool()
    {
        handleUtilityType = typeof(Editor).Assembly.GetTypes().First(t => t.Name == "HandleUtility");
        rayMeshMethodInfo = handleUtilityType.GetMethod("IntersectRayMesh", BindingFlags.Static | BindingFlags.NonPublic);


    }


    public static bool IntersectRayMesh(Ray ray, MeshFilter meshFilter, out RaycastHit hit)
    {
        bool result;
        // meshfilter必须要传入 sharedMesh 共享网格，
        object[] pars = new object[] { ray, meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix, null };
        result = (bool)rayMeshMethodInfo.Invoke(null, pars);
        hit = (RaycastHit)pars[3];
        return result;

    }
}
