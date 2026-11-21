using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public static class LocalizationSettingsInstaller
{
    public const string SettingsPath = "Assets/Localization Settings.asset";
    public const string LocalesFolder = "Assets/Localization";
    public const string ProjectLocale = "fa-IR";
    public const string CommandLineArgument = "-language=";
    public static readonly string[] Locales = { "fa-IR", "en-US" };

    private const int PreloadAllLocales = 3;
    private const string ProjectLocaleProperty = "m_ProjectLocaleIdentifier.m_Code";
    private const string PreloadProperty = "m_PreloadBehavior";
    private const string SynchronousProperty = "m_InitializeSynchronously";

    public static List<string> Missing()
    {
        List<string> missing = new List<string>();
        if (LocalizationEditorSettings.ActiveLocalizationSettings == null)
            missing.Add("Localization settings");

        foreach (string code in Locales)
        {
            if (LocalizationEditorSettings.GetLocale(code) == null)
                missing.Add("Locale " + code);
        }

        return missing;
    }

    public static bool Install(bool applyValues)
    {
        bool changed = false;
        LocalizationSettings settings = LocalizationEditorSettings.ActiveLocalizationSettings;
        if (settings == null)
        {
            settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(SettingsPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<LocalizationSettings>();
                AssetDatabase.CreateAsset(settings, SettingsPath);
            }

            LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            applyValues = true;
            changed = true;
        }

        for (int i = 0; i < Locales.Length; i++)
            changed |= AddLocale(Locales[i], i);

        if (applyValues && !Matches(settings))
        {
            Apply(settings);
            changed = true;
        }

        return changed;
    }

    public static bool Matches(LocalizationSettings settings)
    {
        if (settings == null)
            return false;

        List<IStartupLocaleSelector> selectors = settings.GetStartupLocaleSelectors();
        bool selectorsMatch = selectors.Count == 3
                              && selectors[0] is CommandLineLocaleSelector commandLine && commandLine.CommandLineArgument == CommandLineArgument
                              && selectors[1] is SystemLocaleSelector
                              && selectors[2] is SpecificLocaleSelector specific && specific.LocaleId.Code == ProjectLocale;

        SerializedObject serialized = new SerializedObject(settings);
        return selectorsMatch
               && serialized.FindProperty(ProjectLocaleProperty).stringValue == ProjectLocale
               && serialized.FindProperty(PreloadProperty).intValue == PreloadAllLocales
               && serialized.FindProperty(SynchronousProperty).boolValue;
    }

    private static void Apply(LocalizationSettings settings)
    {
        List<IStartupLocaleSelector> selectors = settings.GetStartupLocaleSelectors();
        selectors.Clear();
        selectors.Add(new CommandLineLocaleSelector { CommandLineArgument = CommandLineArgument });
        selectors.Add(new SystemLocaleSelector());
        selectors.Add(new SpecificLocaleSelector { LocaleId = new LocaleIdentifier(ProjectLocale) });

        SerializedObject serialized = new SerializedObject(settings);
        serialized.Update();
        serialized.FindProperty(ProjectLocaleProperty).stringValue = ProjectLocale;
        serialized.FindProperty(PreloadProperty).intValue = PreloadAllLocales;
        serialized.FindProperty(SynchronousProperty).boolValue = true;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(settings);
    }

    private static bool AddLocale(string code, int sortOrder)
    {
        if (LocalizationEditorSettings.GetLocale(code) != null)
            return false;

        Locale locale = Locale.CreateLocale(new LocaleIdentifier(code));
        locale.name = new CultureInfo(code).EnglishName + " (" + code + ")";
        locale.SortOrder = (ushort) sortOrder;
        Directory.CreateDirectory(LocalesFolder);
        AssetDatabase.CreateAsset(locale, LocalesFolder + "/" + locale.name + ".asset");
        LocalizationEditorSettings.AddLocale(locale);
        return true;
    }
}
