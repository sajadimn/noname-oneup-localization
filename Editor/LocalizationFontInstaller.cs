using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class LocalizationFontInstaller
{
    private const string PackageFontsFolder = "Fonts~";
    private const string GuidPrefix = "guid:";

    public static string PackageFontsPath()
    {
        string packagePath = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(LocalizationFontInstaller).Assembly).resolvedPath;
        return System.IO.Path.Combine(packagePath, PackageFontsFolder);
    }

    public static string ReadGuid(string metaPath)
    {
        string line = File.ReadAllLines(metaPath).FirstOrDefault(l => l.StartsWith(GuidPrefix));
        return line == null ? string.Empty : line.Substring(GuidPrefix.Length).Trim();
    }

    public static bool ShouldCopy(string targetPath, string guid)
    {
        if (File.Exists(targetPath))
            return false;

        string guidPath = AssetDatabase.GUIDToAssetPath(guid);
        return string.IsNullOrEmpty(guidPath) || !File.Exists(guidPath);
    }

    public static string TargetPath(LocalizationFont font, string file)
    {
        return LocalizationFonts.ProjectRoot + "/" + font.Folder + "/" + file;
    }

    public static string PackageFile(LocalizationFont font, string file)
    {
        return System.IO.Path.Combine(PackageFontsPath(), font.Folder, file);
    }

    public static string ProjectPath(LocalizationFont font, string file)
    {
        string guidPath = AssetDatabase.GUIDToAssetPath(ReadGuid(PackageFile(font, file) + ".meta"));
        return !string.IsNullOrEmpty(guidPath) && File.Exists(guidPath) ? guidPath : TargetPath(font, file);
    }

    public static T Load<T>(LocalizationFont font, string file) where T : Object
    {
        return AssetDatabase.LoadAssetAtPath<T>(ProjectPath(font, file));
    }

    public static bool IsMissing(LocalizationFont font, string file)
    {
        return ShouldCopy(TargetPath(font, file), ReadGuid(PackageFile(font, file) + ".meta"));
    }

    public static bool Install()
    {
        bool changed = false;
        foreach (LocalizationFont font in LocalizationFonts.All)
        {
            foreach (string file in font.Files)
            {
                if (!IsMissing(font, file))
                    continue;

                string target = TargetPath(font, file);
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(target));
                File.Copy(PackageFile(font, file), target);
                File.Copy(PackageFile(font, file) + ".meta", target + ".meta");
                changed = true;
            }
        }

        if (changed)
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        return changed;
    }
}
