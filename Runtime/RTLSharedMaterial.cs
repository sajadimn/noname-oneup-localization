using System;
using System.Collections.Generic;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class RTLSharedMaterial
{
    private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();
    private static readonly Dictionary<(LocalizedAssetDatabase, Locale, string, string, Type), AsyncOperationHandle> FontAssets =
        new Dictionary<(LocalizedAssetDatabase, Locale, string, string, Type), AsyncOperationHandle>();
    private static readonly HashSet<(LocalizedAssetDatabase, Locale, string, string, Type)> StartingAssets =
        new HashSet<(LocalizedAssetDatabase, Locale, string, string, Type)>();

    public static Material ApplyFontStyle(Material current, Material configured, bool persian)
    {
        if (configured == null)
            return current;
        if (!persian || !configured.HasProperty(ShaderUtilities.ID_OutlineWidth) ||
            Mathf.Approximately(configured.GetFloat(ShaderUtilities.ID_OutlineWidth), 0f))
            return configured;
        return Get(configured, "Persian no outline", material => material.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f));
    }

    internal static Material GetConfiguredMaterial(string table, string key)
    {
        return GetConfiguredAsset<Material>(table, key);
    }

    internal static bool TryGetLocale(out Locale locale)
    {
        locale = null;
        if (!LocalizationSettings.HasSettings)
            return false;
        var operation = LocalizationSettings.SelectedLocaleAsync;
        if (!operation.IsValid() || !operation.IsDone || operation.Status != AsyncOperationStatus.Succeeded)
            return false;
        locale = operation.Result;
        return locale != null;
    }

    internal static T GetConfiguredAsset<T>(string table, string key) where T : UnityEngine.Object
    {
        if (!TryGetLocale(out Locale locale) || LocalizationSettings.AssetDatabase == null)
            return null;
        var initialization = LocalizationSettings.InitializationOperation;
        if (!initialization.IsValid() || !initialization.IsDone || initialization.Status != AsyncOperationStatus.Succeeded)
            return null;
        var database = LocalizationSettings.AssetDatabase;
        var id = (database, locale, table, key, typeof(T));
        if (!FontAssets.TryGetValue(id, out AsyncOperationHandle operation) || !operation.IsValid())
        {
            if (!StartingAssets.Add(id))
                return null;
            try
            {
                operation = database.GetLocalizedAssetAsync<T>(table, key, locale);
                FontAssets[id] = operation;
            }
            finally
            {
                StartingAssets.Remove(id);
            }
        }
        if (!operation.IsValid() || !operation.IsDone || operation.Status != AsyncOperationStatus.Succeeded)
            return null;
        T asset = operation.Result as T;
        if (asset == null)
            FontAssets.Remove(id);
        return asset;
    }

    internal static Material ForText(TMP_Text text, Material current)
    {
        if (text == null || !TryGetLocale(out Locale locale))
            return current;
        bool shadow = text is RTLTextMeshPro ui ? ui.HasShadow : text is RTLTextMeshPro3D world && world.UsesShadow(current);
        RTLFontChanger changer = text.GetComponent<RTLFontChanger>();
        Material configured = changer != null ? changer.GetConfiguredMaterial(shadow) :
            GetConfiguredMaterial("Fonts", shadow ? "MaterialShadow" : "MaterialNoShadow");
        if (configured == null)
            return current;
        string language = locale.Identifier.Code;
        bool persian = language.Equals("fa", StringComparison.OrdinalIgnoreCase) || language.StartsWith("fa-", StringComparison.OrdinalIgnoreCase);
        Material styled = ApplyFontStyle(current, configured, persian);
        RTLShadowColor colors = shadow ? text.GetComponent<RTLShadowColor>() : null;
        return colors != null ? colors.ApplyColors(styled) : styled;
    }

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
