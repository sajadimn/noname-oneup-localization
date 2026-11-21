using System;
using System.Collections.Generic;
using UnityEngine;

public static class RTLSharedMaterial
{
    private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();

    public static Material Get(Material source, string key, Action<Material> apply)
    {
        string id = source.GetInstanceID() + "|" + key;
        if (Materials.TryGetValue(id, out Material shared) && shared != null)
            return shared;
        shared = new Material(source) { name = source.name + " " + key };
        apply(shared);
        Materials[id] = shared;
        return shared;
    }

    public static string Key(Color color)
    {
        return ColorUtility.ToHtmlStringRGBA(color);
    }
}
