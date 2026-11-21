public class LocalizationFont
{
    public string Folder { get; }
    public string LocaleCode { get; }
    public string FontAsset { get; }
    public bool Preload { get; }
    public string[] Files { get; }

    public LocalizationFont(string folder, string localeCode, string fontAsset, bool preload, params string[] files)
    {
        Folder = folder;
        LocaleCode = localeCode;
        FontAsset = fontAsset;
        Preload = preload;
        Files = files;
    }
}

public class LocalizationFontKey
{
    public string Key { get; }
    public string File { get; }

    public LocalizationFontKey(string key, string file)
    {
        Key = key;
        File = file;
    }
}
