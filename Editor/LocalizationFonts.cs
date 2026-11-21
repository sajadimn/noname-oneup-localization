using System.Collections.Generic;

public static class LocalizationFonts
{
    public const string ProjectRoot = "Assets/Fonts";
    public const string TableName = "Fonts";
    public const string TableFolder = "Assets/Localization/Fonts";
    public const string PreloadLabel = "Preload";
    public const string ShadowMaterial = "Shadow.mat";
    public const string NoShadowMaterial = "NoShadow.mat";

    public static readonly IReadOnlyList<LocalizationFont> All = new[]
    {
        new LocalizationFont("English", "en-US", "EnglishFontAsset.asset", false,
            "EnglishFont.TTF", "EnglishFontAsset.asset", NoShadowMaterial, ShadowMaterial),
        new LocalizationFont("Persian", "fa-IR", "PersianFontAsset.asset", true,
            "PersianFont.TTF", "PersianFontAsset.asset", NoShadowMaterial, ShadowMaterial),
        new LocalizationFont("Turkish", "tr-TR", "TurkishFontAsset.asset", false,
            "TurkishFont.TTF", "TurkishFontFile.TTF", "TurkishFontAsset.asset", NoShadowMaterial, ShadowMaterial)
    };

    public static LocalizationFont English => All[0];
    public static LocalizationFont Persian => All[1];

    public static IReadOnlyList<LocalizationFontKey> Keys(LocalizationFont font)
    {
        return new[]
        {
            new LocalizationFontKey("Fonts", font.FontAsset),
            new LocalizationFontKey("MaterialShadow", ShadowMaterial),
            new LocalizationFontKey("MaterialNoShadow", NoShadowMaterial)
        };
    }
}
