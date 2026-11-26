using System;
using System.Collections.Generic;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public static class RTLSharedMaterial
{
    private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();
    private static readonly Dictionary<(LocalizedAssetDatabase, Locale, string, string), Material> FontMaterials =
        new Dictionary<(LocalizedAssetDatabase, Locale, string, string), Material>();

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
        if (!LocalizationSettings.HasSettings || LocalizationSettings.SelectedLocale == null || LocalizationSettings.AssetDatabase == null)
            return null;
        var id = (LocalizationSettings.AssetDatabase, LocalizationSettings.SelectedLocale, table, key);
        if (FontMaterials.TryGetValue(id, out Material material) && material != null)
            return material;
        material = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Material>(table, key, LocalizationSettings.SelectedLocale);
        if (material != null)
            FontMaterials[id] = material;
        return material;
    }

    internal static Material ForText(TMP_Text text, Material current)
    {
        if (text == null || !LocalizationSettings.HasSettings || LocalizationSettings.SelectedLocale == null)
            return current;
        bool shadow = text is RTLTextMeshPro ui ? ui.HasShadow : text is RTLTextMeshPro3D world && world.UsesShadow(current);
        RTLFontChanger changer = text.GetComponent<RTLFontChanger>();
        Material configured = changer != null ? changer.GetConfiguredMaterial(shadow) :
            GetConfiguredMaterial("Fonts", shadow ? "MaterialShadow" : "MaterialNoShadow");
        if (configured == null)
            return current;
        string language = LocalizationSettings.SelectedLocale.Identifier.Code;
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
