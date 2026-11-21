using System;
using System.Collections.Generic;
using System.Globalization;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizeTextAlignmentEvent : MonoBehaviour
{
    private bool lastLanguageIsRTL = false;
    [SerializeField] private bool switchCostumeChildren = false;

    [SerializeField]
    private List<RTLTextMeshPro> children = new List<RTLTextMeshPro>();

    private void OnEnable()
    {
        if (lastLanguageIsRTL == CurrentLanguageIsRTL())
            return;

        SwitchChildAlignment();
        lastLanguageIsRTL = CurrentLanguageIsRTL();
    }

    private bool CurrentLanguageIsRTL()
    {
        if (LocalizationSettings.SelectedLocale == null)
            return false;

        CultureInfo cultureInfo = new CultureInfo(LocalizationSettings.SelectedLocale.Identifier.Code);
        return cultureInfo.TextInfo.IsRightToLeft;
    }

    private void SwitchChildAlignment()
    {
        if (transform.childCount == 0)
            return;
        if (switchCostumeChildren)
        {
            for (int i = 0, j = children.Count - 1; i < children.Count / 2; i++, j--)
            {
                try
                {
                    TextAlignmentOptions temp = children[i].alignment == TextAlignmentOptions.Left
                        ? TextAlignmentOptions.Right
                        : TextAlignmentOptions.Left;
                    children[i].alignment = temp;
                }
                catch (Exception exception)
                {
                    Debug.LogError("LocalizeTextAlignmentEvent - SwitchChildAlignment - Exception: " +
                                   exception.Message);
                }
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<RTLTextMeshPro>() != null)
                {
                    TextAlignmentOptions temp =
                        transform.GetChild(i).GetComponent<RTLTextMeshPro>().alignment == TextAlignmentOptions.Left
                            ? TextAlignmentOptions.Right
                            : TextAlignmentOptions.Left;
                    transform.GetChild(i).GetComponent<RTLTextMeshPro>().alignment = temp;
                }
            }
        }
    }
}