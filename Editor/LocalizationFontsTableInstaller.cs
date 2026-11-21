using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

public static class LocalizationFontsTableInstaller
{
    public static List<string> Missing()
    {
        List<string> missing = new List<string>();
        AssetTableCollection collection = LocalizationEditorSettings.GetAssetTableCollection(LocalizationFonts.TableName);
        if (collection == null)
        {
            missing.Add("Fonts table");
            return missing;
        }

        foreach (LocalizationFont font in ProjectFonts())
        {
            AssetTable table = collection.GetTable(font.LocaleCode) as AssetTable;
            if (table == null)
            {
                missing.Add("Fonts table " + font.LocaleCode);
                continue;
            }

            foreach (LocalizationFontKey key in LocalizationFonts.Keys(font))
            {
                AssetTableEntry entry = table.GetEntry(key.Key);
                if (entry == null || string.IsNullOrEmpty(entry.Guid))
                    missing.Add("Fonts table " + font.LocaleCode + " " + key.Key);
            }

            if (font.Preload && !HasPreloadLabel(font))
                missing.Add("Preload label " + font.FontAsset);
        }

        return missing;
    }

    public static bool Install()
    {
        List<LocalizationFont> fonts = ProjectFonts();
        if (fonts.Count == 0)
            return false;

        bool changed = false;
        AssetTableCollection collection = LocalizationEditorSettings.GetAssetTableCollection(LocalizationFonts.TableName);
        if (collection == null)
        {
            List<Locale> locales = fonts.Select(f => LocalizationEditorSettings.GetLocale(f.LocaleCode)).ToList();
            collection = LocalizationEditorSettings.CreateAssetTableCollection(LocalizationFonts.TableName, LocalizationFonts.TableFolder, locales);
            changed = true;
        }

        foreach (LocalizationFont font in fonts)
        {
            AssetTable table = collection.GetTable(font.LocaleCode) as AssetTable;
            if (table == null)
            {
                table = collection.AddNewTable(font.LocaleCode) as AssetTable;
                changed = true;
            }

            foreach (LocalizationFontKey key in LocalizationFonts.Keys(font))
            {
                AssetTableEntry entry = table.GetEntry(key.Key);
                if (entry != null && !string.IsNullOrEmpty(entry.Guid))
                    continue;

                Object asset = LocalizationFontInstaller.Load<Object>(font, key.File);
                if (asset == null)
                    continue;

                collection.AddAssetToTable(table, key.Key, asset);
                changed = true;
            }

            if (font.Preload)
                changed |= AddPreloadLabel(font);
        }

        return changed;
    }

    private static List<LocalizationFont> ProjectFonts()
    {
        return LocalizationFonts.All.Where(f => LocalizationEditorSettings.GetLocale(f.LocaleCode) != null).ToList();
    }

    private static AddressableAssetEntry FontEntry(LocalizationFont font)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
            return null;

        string guid = UnityEditor.AssetDatabase.AssetPathToGUID(LocalizationFontInstaller.ProjectPath(font, font.FontAsset));
        return settings.FindAssetEntry(guid);
    }

    private static bool HasPreloadLabel(LocalizationFont font)
    {
        AddressableAssetEntry entry = FontEntry(font);
        return entry != null && entry.labels.Contains(LocalizationFonts.PreloadLabel);
    }

    private static bool AddPreloadLabel(LocalizationFont font)
    {
        AddressableAssetEntry entry = FontEntry(font);
        if (entry == null || entry.labels.Contains(LocalizationFonts.PreloadLabel))
            return false;

        AddressableAssetSettingsDefaultObject.Settings.AddLabel(LocalizationFonts.PreloadLabel);
        entry.SetLabel(LocalizationFonts.PreloadLabel, true, true);
        return true;
    }
}
