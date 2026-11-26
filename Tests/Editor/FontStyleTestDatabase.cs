using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class FontStyleTestDatabase : LocalizedAssetDatabase
{
    public TMP_FontAsset EnglishFont;
    public TMP_FontAsset PersianFont;
    public Material EnglishShadow;
    public Material PersianShadow;
    public Material EnglishNoShadow;
    public Material PersianNoShadow;
    public bool Missing;

    public override TObject GetLocalizedAsset<TObject>(TableReference tableReference, TableEntryReference tableEntryReference, Locale locale = null)
    {
        if (Missing)
            return null;
        bool persian = (locale ?? LocalizationSettings.SelectedLocale).Identifier.Code.StartsWith("fa");
        if (typeof(TObject) == typeof(TMP_FontAsset))
            return (persian ? PersianFont : EnglishFont) as TObject;
        if (typeof(TObject) == typeof(Material))
        {
            bool shadow = tableEntryReference.Key == "MaterialShadow";
            return (persian ? (shadow ? PersianShadow : PersianNoShadow) : (shadow ? EnglishShadow : EnglishNoShadow)) as TObject;
        }
        return null;
    }
}
