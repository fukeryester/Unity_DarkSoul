using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class TransformHelper
{
    public static Transform DeepFind(this Transform parent, string targetName)
    {
        if(parent.name == targetName) return parent;
        Transform temp = null;
        foreach(Transform child in parent)
        {
            if(child.name == targetName)
            {
                return child;
            }
            else
            {
                temp = DeepFind(child, targetName);
                if (temp) return temp;
            }
        }
        return null;
    }
}
