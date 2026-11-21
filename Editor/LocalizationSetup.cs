using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class LocalizationSetup
{
    public const string MenuPath = "OneUp/Localization/Setup";

    static LocalizationSetup()
    {
        if (Application.isBatchMode)
            return;

        EditorApplication.delayCall += AutoRun;
    }

    [MenuItem(MenuPath)]
    private static void RunFromMenu()
    {
        bool changed = Run(true);
        Debug.Log(changed ? "OneUp Localization: setup done" : "OneUp Localization: already set up");
    }

    public static List<string> Missing()
    {
        List<string> missing = new List<string>();
        foreach (LocalizationFont font in LocalizationFonts.All)
        {
            foreach (string file in font.Files)
            {
                if (LocalizationFontInstaller.IsMissing(font, file))
                    missing.Add("Font " + font.Folder + "/" + file);
            }
        }

        missing.AddRange(LocalizationSettingsInstaller.Missing());
        missing.AddRange(LocalizationFontsTableInstaller.Missing());
        missing.AddRange(LocalizationTmpSettingsInstaller.Missing());
        return missing;
    }

    public static bool Run(bool applyValues)
    {
        bool changed = LocalizationFontInstaller.Install();
        changed |= LocalizationSettingsInstaller.Install(applyValues);
        changed |= LocalizationFontsTableInstaller.Install();
        changed |= LocalizationTmpSettingsInstaller.Install(applyValues);

        if (changed)
            AssetDatabase.SaveAssets();

        return changed;
    }

    private static void AutoRun()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.delayCall += AutoRun;
            return;
        }

        if (Missing().Count == 0)
            return;

        if (Run(false))
            Debug.Log("OneUp Localization: setup done");
    }
}
