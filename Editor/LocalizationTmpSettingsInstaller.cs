using System.Collections.Generic;
using TMPro;
using UnityEditor;

public static class LocalizationTmpSettingsInstaller
{
    public const string FontAssetPath = "Fonts & Materials/";

    private const string StockFontName = "LiberationSans SDF";
    private const string DefaultFontProperty = "m_defaultFontAsset";
    private const string FontPathProperty = "m_defaultFontAssetPath";
    private const string FallbackProperty = "m_fallbackFontAssets";
    private const string MatchMaterialProperty = "m_matchMaterialPreset";

    private static bool essentialsRequested;

    public static List<string> Missing()
    {
        List<string> missing = new List<string>();
        TMP_Settings settings = TMP_Settings.instance;
        if (settings == null)
        {
            missing.Add("TextMeshPro settings");
            return missing;
        }

        SerializedObject serialized = new SerializedObject(settings);
        if (IsStock(serialized.FindProperty(DefaultFontProperty).objectReferenceValue as TMP_FontAsset))
            missing.Add("TextMeshPro default font");

        TMP_FontAsset persian = LocalizationFontInstaller.Load<TMP_FontAsset>(LocalizationFonts.Persian, LocalizationFonts.Persian.FontAsset);
        if (persian == null || !Contains(serialized.FindProperty(FallbackProperty), persian))
            missing.Add("TextMeshPro fallback font");

        return missing;
    }

    public static bool Install(bool applyValues)
    {
        TMP_Settings settings = TMP_Settings.instance;
        if (settings == null)
        {
            RequestEssentials();
            return false;
        }

        SerializedObject serialized = new SerializedObject(settings);
        serialized.Update();

        TMP_FontAsset english = LocalizationFontInstaller.Load<TMP_FontAsset>(LocalizationFonts.English, LocalizationFonts.English.FontAsset);
        SerializedProperty defaultFont = serialized.FindProperty(DefaultFontProperty);
        if (english != null && defaultFont.objectReferenceValue != english && (applyValues || IsStock(defaultFont.objectReferenceValue as TMP_FontAsset)))
            defaultFont.objectReferenceValue = english;

        TMP_FontAsset persian = LocalizationFontInstaller.Load<TMP_FontAsset>(LocalizationFonts.Persian, LocalizationFonts.Persian.FontAsset);
        SerializedProperty fallback = serialized.FindProperty(FallbackProperty);
        if (persian != null && !Contains(fallback, persian))
        {
            fallback.InsertArrayElementAtIndex(0);
            fallback.GetArrayElementAtIndex(0).objectReferenceValue = persian;
        }

        if (applyValues)
        {
            SerializedProperty fontPath = serialized.FindProperty(FontPathProperty);
            if (fontPath.stringValue != FontAssetPath)
                fontPath.stringValue = FontAssetPath;

            SerializedProperty matchMaterial = serialized.FindProperty(MatchMaterialProperty);
            if (!matchMaterial.boolValue)
                matchMaterial.boolValue = true;
        }

        bool changed = serialized.ApplyModifiedPropertiesWithoutUndo();
        if (changed)
            EditorUtility.SetDirty(settings);

        return changed;
    }

    private static bool IsStock(TMP_FontAsset font)
    {
        return font == null || font.name == StockFontName;
    }

    private static bool Contains(SerializedProperty list, TMP_FontAsset font)
    {
        for (int i = 0; i < list.arraySize; i++)
        {
            if (list.GetArrayElementAtIndex(i).objectReferenceValue == font)
                return true;
        }

        return false;
    }

    private static void RequestEssentials()
    {
        if (essentialsRequested)
            return;

        essentialsRequested = true;
        AssetDatabase.importPackageCompleted += OnEssentialsImported;
        TMP_PackageResourceImporter.ImportResources(true, false, false);
    }

    private static void OnEssentialsImported(string packageName)
    {
        AssetDatabase.importPackageCompleted -= OnEssentialsImported;
        EditorApplication.delayCall += () => LocalizationSetup.Run(false);
    }
}
