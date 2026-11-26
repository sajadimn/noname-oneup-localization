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

        return RTLTextMeshPro != null && RTLTextMeshPro.font != null && RTLTextMeshPro.fontSharedMaterial != null;
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
            if (RTLTextMeshPro == null)
                RTLTextMeshPro = GetComponent<RTLTextMeshPro>();
            if (RTLTextMeshPro == null)
            {
                initialized = false;
                return;
            }
            
            string key = RTLTextMeshPro.HasShadow ? assetShadowMaterial : assetNoShadowMaterial;
            TMP_FontAsset font = LocalizationSettings.Instance.GetAssetDatabase()
                .GetLocalizedAsset<TMP_FontAsset>(assetTableName, assetFontKey);
            Material material = RTLSharedMaterial.GetConfiguredMaterial(assetTableName, key);
            if (font == null || material == null)
            {
                initialized = false;
                return;
            }
            RTLTextMeshPro.font = font;
            RTLTextMeshPro.fontSharedMaterial = material;

            RTLTextMeshPro.UpdateFontAsset();
            lastLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
            initialized = true;
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
        if (RTLTextMeshPro == null)
            return;

        Locale locale = LocalizationSettings.SelectedLocale ?? LocalizationSettings.ProjectLocale;
        if (locale == null)
            return;

        string key = RTLTextMeshPro.HasShadow ? assetShadowMaterial : assetNoShadowMaterial;
        Material material = RTLSharedMaterial.GetConfiguredMaterial(assetTableName, key);
        if (material == null)
            return;

        RTLTextMeshPro.fontSharedMaterial = material;
    }

    internal Material GetConfiguredMaterial(bool shadow)
    {
        return RTLSharedMaterial.GetConfiguredMaterial(assetTableName, shadow ? assetShadowMaterial : assetNoShadowMaterial);
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
