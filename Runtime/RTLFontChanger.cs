using RTLTMPro;
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
    private bool initialized;

    private void OnSelectedLocaleChanged(Locale locale)
    {
        initialized = false;
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        initialized = false;
        SetFont();
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
    }

    private void Update()
    {
        if (!RTLSharedMaterial.TryGetLocale(out Locale locale))
            return;
        if (initialized && lastLanguage == locale.Identifier.Code)
            return;
        SetFont();
    }

    private void SetFont()
    {
        if (!RTLSharedMaterial.TryGetLocale(out Locale locale))
            return;
        if (RTLTextMeshPro == null)
            RTLTextMeshPro = GetComponent<RTLTextMeshPro>();
        if (RTLTextMeshPro == null)
            return;
        TMP_FontAsset font = RTLSharedMaterial.GetConfiguredAsset<TMP_FontAsset>(assetTableName, assetFontKey);
        Material material = GetConfiguredMaterial(RTLTextMeshPro.HasShadow);
        if (font == null || material == null)
            return;
        RTLTextMeshPro.font = font;
        RTLTextMeshPro.fontSharedMaterial = material;
        RTLTextMeshPro.UpdateFontAsset();
        lastLanguage = locale.Identifier.Code;
        initialized = true;
    }

    public void SetShadowMaterial()
    {
        initialized = false;
        if (RTLTextMeshPro == null)
            RTLTextMeshPro = GetComponent<RTLTextMeshPro>();
        if (RTLTextMeshPro == null)
            return;
        Material material = GetConfiguredMaterial(RTLTextMeshPro.HasShadow);
        if (material != null)
            RTLTextMeshPro.fontSharedMaterial = material;
    }

    internal Material GetConfiguredMaterial(bool shadow)
    {
        return RTLSharedMaterial.GetConfiguredMaterial(assetTableName, shadow ? assetShadowMaterial : assetNoShadowMaterial);
    }
}
