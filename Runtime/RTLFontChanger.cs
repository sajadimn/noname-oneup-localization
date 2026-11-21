using RTLTMPro;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[RequireComponent(typeof(RTLOrder))]
[RequireComponent(typeof(RTLTextMeshPro))]
public class RTLFontChanger : MonoBehaviour
{
    [SerializeField] private RTLTextMeshPro RTLTextMeshPro;
    [SerializeField] private string assetTableName = "Fonts";
    [SerializeField] private string assetFontKey = "Fonts";
    [SerializeField] private string assetShadowMaterial = "MaterialShadow";
    [SerializeField] private string assetNoShadowMaterial = "MaterialNoShadow";

    private string lastLanguage = "";
    private bool initialized = false;
    private static int changeLocalizeCount;
    private static string savedLocalizeLanguage;

    private void OnSelectedLocaleChanged(Locale obj)
    {
        initialized = false;
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;

        if (LocalizationSettings.SelectedLocale == null)
            return;

        if (LocalizationSettings.Instance == null)
            return;

        if (LocalizationSettings.Instance.GetAssetDatabase() == null)
            return;

        if (lastLanguage == LocalizationSettings.SelectedLocale.Identifier.Code && HasFont())
            return;

        SetFont();
    }

    private bool HasFont()
    {
        if (RTLTextMeshPro == null)
            RTLTextMeshPro = GetComponent<RTLTextMeshPro>();

        return RTLTextMeshPro.font != null && RTLTextMeshPro.fontSharedMaterial != null;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
    }

    private void Update()
    {
        if (LocalizationSettings.SelectedLocale == null)
            return;

        if (LocalizationSettings.Instance == null)
            return;

        if (LocalizationSettings.Instance.GetAssetDatabase() == null)
            return;

        if (initialized)
            return;

        SetFont();
    }


    private void SetFont()
    {
        try
        {
            initialized = true;
            if (RTLTextMeshPro == null)
                RTLTextMeshPro = GetComponent<RTLTextMeshPro>();

            RTLTextMeshPro.font = LocalizationSettings.Instance.GetAssetDatabase()
                .GetLocalizedAsset<TMP_FontAsset>(assetTableName, assetFontKey);
            
            string key = RTLTextMeshPro.HasShadow ? assetShadowMaterial : assetNoShadowMaterial;
            RTLTextMeshPro.fontMaterial = LocalizationSettings.Instance.GetAssetDatabase()
                .GetLocalizedAsset<Material>(assetTableName, key);

            //Todo First Disable Preload And Synchronize Localization Then Remove This Rename Code And Check Text Fonts And Shadows(3 July Commit 18:18)
            RTLTextMeshPro.font.name = RTLTextMeshPro.font.name;
            RTLTextMeshPro.fontSharedMaterial.name = RTLTextMeshPro.fontSharedMaterial.name;
            RTLTextMeshPro.UpdateFontAsset();
            lastLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        }
        catch (Exception e)
        {
            ResetLocalize();
        }
    }

    public void SetShadowMaterial()
    {
        if (RTLTextMeshPro == null)
            RTLTextMeshPro = GetComponent<RTLTextMeshPro>();

        Locale locale = LocalizationSettings.SelectedLocale ?? LocalizationSettings.ProjectLocale;
        if (locale == null)
            return;

        string key = RTLTextMeshPro.HasShadow ? assetShadowMaterial : assetNoShadowMaterial;
        Material material = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Material>(assetTableName, key, locale);
        if (material == null)
            return;

        if (Application.isPlaying)
            RTLTextMeshPro.fontMaterial = material;
        else
            RTLTextMeshPro.fontSharedMaterial = material;
    }

    private void ResetLocalize()
    {
        lastLanguage = "";
        if (RTLFontChanger.changeLocalizeCount == 0)
        {
            ChangeLocalization(SetFont);
        }
        else
        {
            StartCoroutine(WaitUntilLocalizeReset());
        }
    }

    private IEnumerator WaitUntilLocalizeReset()
    {
        Debug.LogError("RTLFontChanger - ChangeLocalization: WaitUntilLocalizeReset - Start");
        yield return new WaitWhile(() => RTLFontChanger.changeLocalizeCount < 2);
        Debug.LogError("RTLFontChanger - ChangeLocalization: WaitUntilLocalizeReset - Finish");
        SetFont();
    }

    private void ChangeLocalization(Action callback)
    {
        if (RTLFontChanger.changeLocalizeCount == 0)
        {
            Debug.LogError("RTLFontChanger - ChangeLocalization: RTLFontChanger.changeLocalizeCount == 0");
            RTLFontChanger.changeLocalizeCount++;
            RTLFontChanger.savedLocalizeLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
            LocalizationSettings.SelectedLocale =
                LocalizationSettings.AvailableLocales.Locales.Find(x =>
                    x.Identifier.Code != LocalizationSettings.SelectedLocale.Identifier.Code);
            ChangeLocalization(callback);
        }
        else if (RTLFontChanger.changeLocalizeCount == 1)
        {
            Debug.LogError("RTLFontChanger - ChangeLocalization: RTLFontChanger.changeLocalizeCount == 1");
            RTLFontChanger.changeLocalizeCount++;
            LocalizationSettings.SelectedLocale =
                LocalizationSettings.AvailableLocales.Locales.Find(x =>
                    x.Identifier.Code == RTLFontChanger.savedLocalizeLanguage);
            callback?.Invoke();
        }
    }
}