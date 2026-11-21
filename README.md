Localization Unity Package
===

## Description
This package contains right-to-left TextMeshPro text for Persian (`RTLTextMeshPro`), the localized font and shadow material switch (`RTLFontChanger`, `RTLShadowColor`), localization helpers (`LocalizeSetting`, `LocalizeDirectionEvent`, `LocalizeTextAlignmentEvent`) and a setup that prepares the project:

- copies the English, Persian and Turkish fonts, each with its font asset and `Shadow` / `NoShadow` materials, into `Assets/Fonts/<Language>`
- creates the Persian (fa-IR) and English (en-US) locales and the Localization settings: command line, system and specific (fa-IR) locale selectors, project locale fa-IR, Preload All Locales, Initialize Synchronously
- creates the `Fonts` asset table with the keys `Fonts`, `MaterialShadow` and `MaterialNoShadow` for each locale, and labels the Persian font asset `Preload` in Addressables
- imports the TextMeshPro Essentials when they are missing, and sets the English font asset as the TextMeshPro default font and the Persian one as its fallback

## How to use
Copy the link below and add it to your project via Unity Package Manager: [Installing from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)
```
https://github.com/sajadimn/noname-oneup-localization.git
```

## Dependencies
Required: `com.unity.localization` (with Addressables), `com.unity.ugui` (TextMeshPro). No Odin: the two localize components have their own inspectors.

## Setup
The setup runs by itself after the package is imported, and again whenever something it adds is missing. It never replaces what the project already has:

- a font file is copied only when the project has no file at that path and no asset with its GUID, so you can replace the fonts with your own
- Localization settings, locales and `Fonts` table entries are only added when missing
- the TextMeshPro default font is set only while it is still the stock font

Run `OneUp/Localization/Setup` to repeat it by hand. The menu also sets the Localization and TextMeshPro values listed above on settings that already exist.

A Turkish font is included; its `Fonts` table entries are added when the project has a Turkish (tr-TR) locale.

## Examples

✅️ Text from the `UI` string table
```c#
string title = LocalizeSetting.GetLocalizationString("UI", "TITLE");
```

✅️ Change the language
```c#
await LocalizeSetting.SetLocalizationSetting("fa-IR");
```

✅️ Numbers in the current language
```c#
string coins = LocalizeSetting.GetLocalizationNumber("120");
```

## Credits
Right-to-left text is based on [RTLTMPro](https://github.com/mnarimani/RTLTMPro) (MIT).

Fonts: Fredoka (English) and Lalezar (Persian, Borna Izadpanah) under the [SIL Open Font License](https://openfontlicense.org); Kimberley (Turkish, Ray Larabie) as Larabie Fonts freeware.
