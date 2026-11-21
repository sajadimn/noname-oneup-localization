using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public static class LocalizeSetting
{
    public static async Task<bool> SetLocalizationSetting(string languageCode)
    {
        if (LocalizationSettings.SelectedLocale == null)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.1f));
            return await SetLocalizationSetting(languageCode);
        }
        
        Locale locale =
            LocalizationSettings.AvailableLocales.Locales.Find(x => x.Identifier.Code == languageCode);
        LocalizationSettings.SelectedLocale = locale;
        return true;
    }

    public static string GetLocalizationCode()
    {
        return LocalizationSettings.SelectedLocale.Identifier.Code;
    }

    public static string GetLocalizationString(string table, string key)
    {
        StringTable stringTable = LocalizationSettings.Instance.GetStringDatabase().GetTable(table);
        if (stringTable.GetEntry(key) != null)
            return stringTable.GetEntry(key).Value;

        Debug.LogError($"{key} is not exist in table of {table}");
        return $"{key} is not exist in table of {table}";
    }

    public static string GetLocalizationString(string table, string key, Dictionary<string, string> replaceKey)
    {
        StringTable stringTable = LocalizationSettings.Instance.GetStringDatabase().GetTable(table);
        if (stringTable.GetEntry(key) != null)
        {
            string localizedString = stringTable.GetEntry(key).Value;
            foreach (KeyValuePair<string, string> replace in replaceKey)
            {
                localizedString = localizedString.Replace(replace.Key, replace.Value);
            }

            return localizedString;
        }

        Debug.LogError($"{key} is not exist in table of {table}");
        return $"{key} is not exist in table of {table}";
    }

    public static string GetLocalizationNumber(string number)
    {
        if (LocalizationSettings.SelectedLocale.Identifier.Code == "fa-IR")
        {
            return number.Replace("0", "۰")
                .Replace("1", "۱")
                .Replace("2", "۲")
                .Replace("3", "۳")
                .Replace("4", "۴")
                .Replace("5", "۵")
                .Replace("6", "۶")
                .Replace("7", "۷")
                .Replace("8", "۸")
                .Replace("9", "۹");
        }

        return number.Replace("۰", "0")
            .Replace("۱", "1")
            .Replace("۲", "2")
            .Replace("۳", "3")
            .Replace("۴", "4")
            .Replace("۵", "5")
            .Replace("۶", "6")
            .Replace("۷", "7")
            .Replace("۸", "8")
            .Replace("۹", "9");
    }
}