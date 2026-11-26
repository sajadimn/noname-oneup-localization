using System.Collections.Generic;
using NUnit.Framework;
using RTLTMPro;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class FontMaterialStyleTests
{
    private LocalizationSettings previous;
    private LocalizationSettings settings;
    private Locale english;
    private Locale persian;
    private FontStyleTestDatabase database;
    private readonly List<Object> owned = new List<Object>();

    [SetUp]
    public void SetUp()
    {
        previous = LocalizationSettings.GetInstanceDontCreateDefault();
        settings = Own(ScriptableObject.CreateInstance<LocalizationSettings>());
        english = Own(Locale.CreateLocale("en-US"));
        persian = Own(Locale.CreateLocale("fa-IR"));
        database = new FontStyleTestDatabase
        {
            EnglishFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/English/EnglishFontAsset.asset"),
            PersianFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Persian/PersianFontAsset.asset"),
            EnglishShadow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Fonts/English/Shadow.mat"),
            PersianShadow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Fonts/Persian/Shadow.mat"),
            EnglishNoShadow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Fonts/English/NoShadow.mat"),
            PersianNoShadow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Fonts/Persian/NoShadow.mat")
        };
        Assert.IsNotNull(database.PersianShadow);
        Assert.IsNotNull(database.EnglishShadow);
        settings.SetAssetDatabase(database);
        LocalizationSettings.Instance = settings;
        LocalizationSettings.SelectedLocale = persian;
    }

    [TearDown]
    public void TearDown()
    {
        for (int i = owned.Count - 1; i >= 0; i--)
            if (owned[i] != null)
                Object.DestroyImmediate(owned[i]);
        owned.Clear();
        LocalizationSettings.Instance = previous;
    }

    [Test]
    public void LO_32_PersianShadowUsesFontsWithoutOutline()
    {
        Material current = Own(new Material(database.PersianShadow));
        current.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.24f);
        current.SetFloat(ShaderUtilities.ID_FaceDilate, 0.06f);
        Material actual = RTLSharedMaterial.ApplyFontStyle(current, database.PersianShadow, true);
        Assert.AreEqual(0f, actual.GetFloat(ShaderUtilities.ID_OutlineWidth));
        Assert.AreEqual(database.PersianShadow.GetFloat(ShaderUtilities.ID_FaceDilate), actual.GetFloat(ShaderUtilities.ID_FaceDilate));
        AssertShadow(actual, database.PersianShadow);
        Assert.AreEqual(0f, database.PersianShadow.GetFloat(ShaderUtilities.ID_OutlineWidth));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void LO_33_NoShadowUsesFontsWithoutEffects(bool isPersian)
    {
        Material source = isPersian ? database.PersianNoShadow : database.EnglishNoShadow;
        Material current = Own(new Material(source));
        current.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.4f);
        current.EnableKeyword("UNDERLAY_ON");
        current.EnableKeyword("OUTLINE_ON");
        Material actual = RTLSharedMaterial.ApplyFontStyle(current, source, isPersian);
        Assert.AreSame(source, actual);
        Assert.AreEqual(0f, actual.GetFloat(ShaderUtilities.ID_OutlineWidth));
        Assert.IsFalse(actual.IsKeywordEnabled("UNDERLAY_ON"));
    }

    [Test]
    public void LO_34_EnglishShadowPreservesFontsOutlineAndStroke()
    {
        Material current = Own(new Material(database.EnglishShadow));
        current.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.24f);
        current.SetFloat(ShaderUtilities.ID_FaceDilate, 0.06f);
        Material actual = RTLSharedMaterial.ApplyFontStyle(current, database.EnglishShadow, false);
        Assert.AreSame(database.EnglishShadow, actual);
        Assert.Greater(actual.GetFloat(ShaderUtilities.ID_OutlineWidth), 0f);
        AssertShadow(actual, database.EnglishShadow);
    }

    [Test]
    public void LO_35_SharedColorVariantsDoNotMutateFonts()
    {
        Color original = database.PersianShadow.GetColor(ShaderUtilities.ID_UnderlayColor);
        Material colored = RTLSharedMaterial.Get(database.PersianShadow, "test-blue", material => material.SetColor(ShaderUtilities.ID_UnderlayColor, Color.blue));
        Material actual = RTLSharedMaterial.ApplyFontStyle(colored, database.PersianShadow, true);
        Assert.AreSame(database.PersianShadow, actual);
        Assert.AreEqual(original, database.PersianShadow.GetColor(ShaderUtilities.ID_UnderlayColor));
        Assert.AreSame(actual, RTLSharedMaterial.ApplyFontStyle(colored, database.PersianShadow, true));
    }

    [Test]
    public void LO_36_UiAndFutureTextFollowLocaleAndMaterialChanges()
    {
        RTLTextMeshPro text = CreateUi();
        Assert.AreSame(database.PersianShadow, text.fontSharedMaterial);
        AssignOutline(text);
        Canvas.ForceUpdateCanvases();
        Assert.AreEqual(0f, text.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth));
        LocalizationSettings.SelectedLocale = english;
        text.GetComponent<RTLFontChanger>().SetShadowMaterial();
        Assert.AreEqual(database.EnglishShadow.GetFloat(ShaderUtilities.ID_OutlineWidth), text.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth));
        LocalizationSettings.SelectedLocale = persian;
        text.GetComponent<RTLFontChanger>().SetShadowMaterial();
        Canvas.ForceUpdateCanvases();
        Assert.AreEqual(0f, text.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth));
        Assert.AreEqual(0f, CreateUi().fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth));
    }

    [Test]
    public void LO_37_WorldTextUsesFontsAndRejectsCustomOutline()
    {
        GameObject root = Own(new GameObject("World label"));
        RTLTextMeshPro3D text = root.AddComponent<RTLTextMeshPro3D>();
        text.font = database.PersianFont;
        text.fontSharedMaterial = database.PersianShadow;
        AssignOutline(text);
        Canvas.ForceUpdateCanvases();
        Assert.AreEqual(0f, text.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth));
        AssertShadow(text.fontSharedMaterial, database.PersianShadow);
    }

    [Test]
    public void LO_38_ShadowToggleKeepsColorsAndCorrectFontsStyle()
    {
        RTLTextMeshPro text = CreateUi();
        RTLShadowColor colors = text.gameObject.AddComponent<RTLShadowColor>();
        colors.SetColors(Color.red, Color.blue);
        text.SendMessage("LateUpdate", SendMessageOptions.DontRequireReceiver);
        Canvas.ForceUpdateCanvases();
        Assert.AreEqual(Color.blue, text.fontSharedMaterial.GetColor(ShaderUtilities.ID_UnderlayColor));
        Assert.AreEqual(0f, text.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth));
        text.HasShadow = false;
        Canvas.ForceUpdateCanvases();
        Assert.AreSame(database.PersianNoShadow, text.fontSharedMaterial);
        text.HasShadow = true;
        text.SendMessage("LateUpdate", SendMessageOptions.DontRequireReceiver);
        Canvas.ForceUpdateCanvases();
        Assert.AreEqual(Color.blue, text.fontSharedMaterial.GetColor(ShaderUtilities.ID_UnderlayColor));
        AssertShadow(text.fontSharedMaterial, database.PersianShadow);
    }

    [Test]
    public void LO_39_MissingReferencePreservesMaterial()
    {
        Material current = Own(new Material(database.PersianShadow));
        Assert.AreSame(current, RTLSharedMaterial.ApplyFontStyle(current, null, true));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void LO_40_WorldTextPreservesExistingNoShadowChoice(bool isPersian)
    {
        LocalizationSettings.SelectedLocale = isPersian ? persian : english;
        GameObject root = Own(new GameObject("Existing world label"));
        RTLTextMeshPro3D text = root.AddComponent<RTLTextMeshPro3D>();
        Material source = isPersian ? database.PersianNoShadow : database.EnglishNoShadow;
        text.font = isPersian ? database.PersianFont : database.EnglishFont;
        text.fontSharedMaterial = source;
        Canvas.ForceUpdateCanvases();
        Assert.AreSame(source, text.fontSharedMaterial);
        Assert.IsFalse(text.HasShadow);
        text.HasShadow = true;
        Canvas.ForceUpdateCanvases();
        AssertShadow(text.fontSharedMaterial, isPersian ? database.PersianShadow : database.EnglishShadow);
        text.HasShadow = false;
        Assert.AreSame(source, text.fontSharedMaterial);
    }

    [Test]
    public void LO_41_InPlaceOverrideAndFontMaterialInstanceAreCorrected()
    {
        RTLTextMeshPro text = CreateUi();
        Material overrideMaterial = Own(new Material(database.PersianShadow));
        text.fontMaterial = overrideMaterial;
        Canvas.ForceUpdateCanvases();
        Assert.AreEqual(0f, text.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth));
        Material material = text.fontMaterial;
        owned.Add(material);
        material.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.4f);
        material.EnableKeyword("OUTLINE_ON");
        Canvas.ForceUpdateCanvases();
        Assert.AreEqual(0f, text.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth));
        text.gameObject.SetActive(false);
        text.gameObject.SetActive(true);
        Assert.AreEqual(0f, text.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth));
    }

    private RTLTextMeshPro CreateUi()
    {
        GameObject root = Own(new GameObject("UI label", typeof(RectTransform)));
        RTLTextMeshPro text = root.AddComponent<RTLTextMeshPro>();
        return text;
    }

    private void AssignOutline(TMP_Text text)
    {
        Material material = Own(new Material(text.fontSharedMaterial));
        material.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.24f);
        material.SetFloat(ShaderUtilities.ID_FaceDilate, 0.06f);
        text.fontSharedMaterial = material;
    }

    private static void AssertShadow(Material actual, Material source)
    {
        foreach (string key in new[] { "_UnderlayOffsetX", "_UnderlayOffsetY", "_UnderlayDilate", "_UnderlaySoftness" })
            Assert.AreEqual(source.GetFloat(key), actual.GetFloat(key), key);
        Assert.AreEqual(source.IsKeywordEnabled("UNDERLAY_ON"), actual.IsKeywordEnabled("UNDERLAY_ON"));
    }

    private T Own<T>(T value) where T : Object
    {
        owned.Add(value);
        return value;
    }
}
