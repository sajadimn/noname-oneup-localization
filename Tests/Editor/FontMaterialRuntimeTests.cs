using System.Collections;
using NUnit.Framework;
using RTLTMPro;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.TestTools;

public class FontMaterialRuntimeTests
{
    [UnityTest]
    public IEnumerator LO_42_PM_MaterialsStayCorrectAcrossFramesAndLocaleChanges()
    {
        yield return new EnterPlayMode();
        LocalizationSettings previous = LocalizationSettings.GetInstanceDontCreateDefault();
        var settings = ScriptableObject.CreateInstance<FontStyleRuntimeSettings>();
        var database = new FontStyleTestDatabase
        {
            EnglishFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/English/EnglishFontAsset.asset"),
            PersianFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Persian/PersianFontAsset.asset"),
            EnglishShadow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Fonts/English/Shadow.mat"),
            PersianShadow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Fonts/Persian/Shadow.mat"),
            EnglishNoShadow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Fonts/English/NoShadow.mat"),
            PersianNoShadow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Fonts/Persian/NoShadow.mat")
        };
        settings.SetAssetDatabase(database);
        LocalizationSettings.Instance = settings;
        Locale persian = Locale.CreateLocale("fa-IR");
        LocalizationSettings.SelectedLocale = persian;
        GameObject root = null;
        Material custom = null;
        Locale english = null;
        try
        {
            root = new GameObject("Runtime label", typeof(RectTransform));
            RTLTextMeshPro text = root.AddComponent<RTLTextMeshPro>();
            yield return null;
            Material source = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Material>("Fonts", "MaterialShadow");
            custom = new Material(source);
            custom.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.24f);
            custom.SetFloat(ShaderUtilities.ID_FaceDilate, 0.06f);
            TMP_Text reference = text;
            reference.fontSharedMaterial = custom;
            yield return null;
            Assert.AreEqual(0f, reference.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth));
            Assert.AreEqual(source.GetFloat(ShaderUtilities.ID_FaceDilate), reference.fontSharedMaterial.GetFloat(ShaderUtilities.ID_FaceDilate));
            RTLShadowColor colors = root.GetComponent<RTLShadowColor>() ?? root.AddComponent<RTLShadowColor>();
            colors.SetColors(Color.red, Color.blue);
            yield return null;
            Assert.AreEqual(Color.blue, text.fontSharedMaterial.GetColor(ShaderUtilities.ID_UnderlayColor));
            text.HasShadow = false;
            yield return null;
            Assert.IsFalse(text.fontSharedMaterial.IsKeywordEnabled("UNDERLAY_ON"));
            Assert.AreEqual(0f, text.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth));
            english = Locale.CreateLocale("en-US");
            LocalizationSettings.SelectedLocale = english;
            text.HasShadow = true;
            yield return null;
            yield return null;
            Material englishSource = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Material>("Fonts", "MaterialShadow");
            Assert.AreEqual(englishSource.GetFloat(ShaderUtilities.ID_OutlineWidth), text.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth));
            Assert.Greater(text.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth), 0f);
            Assert.AreSame(LocalizationSettings.AssetDatabase.GetLocalizedAsset<TMP_FontAsset>("Fonts", "Fonts"), text.font);
            Assert.AreEqual(Color.blue, text.fontSharedMaterial.GetColor(ShaderUtilities.ID_UnderlayColor));
        }
        finally
        {
            if (root != null)
                Object.DestroyImmediate(root);
            if (custom != null)
                Object.DestroyImmediate(custom);
            LocalizationSettings.Instance = previous;
            Object.DestroyImmediate(settings);
            Object.DestroyImmediate(persian);
            if (english != null)
                Object.DestroyImmediate(english);
        }
        yield return new ExitPlayMode();
    }
}
