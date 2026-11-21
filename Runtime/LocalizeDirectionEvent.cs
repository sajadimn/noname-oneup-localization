using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class LocalizeDirectionEvent : MonoBehaviour
{
    private bool lastLanguageIsRTL = false;
    [SerializeField] private bool switchCostumeChildren = false;

    [SerializeField]
    private List<Transform> children = new List<Transform>();

    private void OnEnable()
    {
        if (lastLanguageIsRTL == CurrentLanguageIsRTL())
            return;

        SwitchChildSiblingIndex();
        lastLanguageIsRTL = CurrentLanguageIsRTL();
    }

    private bool CurrentLanguageIsRTL()
    {
        if (LocalizationSettings.SelectedLocale == null)
            return false;

        CultureInfo cultureInfo = new CultureInfo(LocalizationSettings.SelectedLocale.Identifier.Code);
        return cultureInfo.TextInfo.IsRightToLeft;
    }

    private void SwitchChildSiblingIndex()
    {
        if (transform.childCount == 0)
            return;
        if (switchCostumeChildren)
        {
            for (int i = 0, j = children.Count - 1; i < children.Count / 2; i++, j--)
            {
                try
                {
                    int temp = children[i].GetSiblingIndex();
                    children[i].SetSiblingIndex(children[j].GetSiblingIndex());
                    children[j].SetSiblingIndex(temp);
                }
                catch (Exception exception)
                {
                    Debug.LogError("LocalizeDirectionEvent - SwitchChildSiblingIndex - Exception: " +
                                   exception.Message);
                }
            }
        }
        else
        {
            int siblingIndex = transform.childCount;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).SetSiblingIndex(siblingIndex);
                siblingIndex--;
            }
        }
    }
}