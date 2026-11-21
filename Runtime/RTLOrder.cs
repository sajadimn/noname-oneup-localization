using System;
using System.Collections;
using System.Threading.Tasks;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class RTLOrder : MonoBehaviour
{
    private RTLTextMeshPro text;

    private void Awake()
    {
        text = GetComponent<RTLTextMeshPro>();
        if (text == null)
            return;

        SetGeometryOrder(LocalizeSetting.GetLocalizationCode());
    }

    private IEnumerator Start()
    {
        text = GetComponent<RTLTextMeshPro>();
        if (text == null)
            yield break;
        yield return new WaitForSeconds(1);
        if (LocalizationSettings.SelectedLocale == null)
            yield break;

        if (LocalizationSettings.Instance == null)
            yield break;

        if (LocalizationSettings.Instance.GetAssetDatabase() == null)
            yield break;

        LocalizationSettings.SelectedLocaleChanged += ChangeHandler;
        ChangeHandler(LocalizationSettings.SelectedLocale);
    }

    protected virtual void ChangeHandler(Locale value)
    {
        if (value == null)
            return;
        SetGeometryOrder(value.Identifier.Code);
    }

    private void SetGeometryOrder(string code)
    {
        if (string.IsNullOrEmpty(code))
            return;

        if (code.Equals("fa-IR"))
            text.geometrySortingOrder = VertexSortingOrder.Reverse;
        else
            text.geometrySortingOrder = VertexSortingOrder.Normal;
    }
}