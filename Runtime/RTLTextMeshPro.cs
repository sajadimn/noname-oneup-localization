using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace RTLTMPro
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RTLFontChanger))]
    public class RTLTextMeshPro : TextMeshProUGUI
    {
        public override Material fontSharedMaterial
        {
            get
            {
                return base.fontSharedMaterial;
            }
            set { base.fontSharedMaterial = RTLSharedMaterial.ForText(this, value); }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Canvas.preWillRenderCanvases += ApplyFontStyle;
            ApplyFontStyle();
        }

        protected override void OnDisable()
        {
            Canvas.preWillRenderCanvases -= ApplyFontStyle;
            base.OnDisable();
        }

        public override void Rebuild(UnityEngine.UI.CanvasUpdate update)
        {
            ApplyFontStyle();
            base.Rebuild(update);
        }

        private void ApplyFontStyle()
        {
            Material current = base.fontSharedMaterial;
            Material styled = RTLSharedMaterial.ForText(this, current);
            if (styled == current)
                return;
            base.fontSharedMaterial = styled;
            UpdateMeshPadding();
        }

        // ReSharper disable once InconsistentNaming
#if TMP_VERSION_2_1_0_OR_NEWER
        public override string text
#else
        public new string text
#endif
        {
            get { return base.text; }
            set
            {
                if (originalText == value)
                    return;

                originalText = value;

                UpdateText();
            }
        }

        public string OriginalText
        {
            get { return originalText; }
        }

        public bool PreserveNumbers
        {
            get { return preserveNumbers; }
            set
            {
                if (preserveNumbers == value)
                    return;

                preserveNumbers = value;
                havePropertiesChanged = true;
            }
        }

        public bool Farsi
        {
            get { return farsi; }
            set
            {
                if (farsi == value)
                    return;

                farsi = value;
                havePropertiesChanged = true;
            }
        }

        public bool FixTags
        {
            get { return fixTags; }
            set
            {
                if (fixTags == value)
                    return;

                fixTags = value;
                havePropertiesChanged = true;
            }
        }

        public bool ForceFix
        {
            get { return forceFix; }
            set
            {
                if (forceFix == value)
                    return;

                forceFix = value;
                havePropertiesChanged = true;
            }
        }

        public bool HasShadow
        {
            get { return hasShadow; }
            set
            {
                if (hasShadow == value)
                    return;

                hasShadow = value;
                havePropertiesChanged = true;
                ApplyShadow();
            }
        }

        private void ApplyShadow()
        {
            if (hasShadow && GetComponent<RTLShadowColor>() == null)
                gameObject.AddComponent<RTLShadowColor>();

            RTLFontChanger fontChanger = GetComponent<RTLFontChanger>();
            if (fontChanger != null)
                fontChanger.SetShadowMaterial();
        }

#if UNITY_EDITOR
        [System.NonSerialized] private bool shadowValidated;
        [System.NonSerialized] private bool validatedShadow;

        protected override void Awake()
        {
            base.Awake();
            shadowValidated = true;
            validatedShadow = hasShadow;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            bool changed = shadowValidated && validatedShadow != hasShadow;
            shadowValidated = true;
            validatedShadow = hasShadow;
            if (!changed)
                return;

            UnityEditor.EditorApplication.delayCall += ApplyShadowFromInspector;
        }

        private void ApplyShadowFromInspector()
        {
            if (this == null)
                return;

            if (hasShadow && GetComponent<RTLShadowColor>() == null)
                UnityEditor.Undo.AddComponent<RTLShadowColor>(gameObject);

            RTLFontChanger fontChanger = GetComponent<RTLFontChanger>();
            if (fontChanger != null)
                fontChanger.SetShadowMaterial();
        }
#endif

        public bool IsRTL
        {
            get { return isRTL; }
            set
            {
                if (isRTL == value)
                    return;

                isRTL = value;
                havePropertiesChanged = true;
            }
        }

        [SerializeField] protected bool preserveNumbers;

        [SerializeField] protected bool farsi = true;

        [SerializeField] [TextArea(3, 10)] protected string originalText;

        [SerializeField] protected bool fixTags = true;

        [SerializeField] protected bool forceFix;

        [SerializeField] protected bool hasShadow = true;
        [SerializeField] protected bool isRTL = true;

        protected readonly FastStringBuilder finalText = new FastStringBuilder(RTLSupport.DefaultBufferSize);
        private string lastLanguage = "";

        protected void Update()
        {
            if (IsLanguageChanged() || havePropertiesChanged)
            {
                UpdateText();
            }
        }

        private bool IsLanguageChanged()
        {
            if (!RTLSharedMaterial.TryGetLocale(out var locale))
                return false;

            if (lastLanguage == locale.Identifier.Code)
                return false;

            lastLanguage = locale.Identifier.Code;
            return true;
        }

        public void UpdateText()
        {
            if (originalText == null)
                originalText = "";
            if (IsRTL)
                SetRTL();
            if (ForceFix == false && TextUtils.IsRTLInput(originalText) == false)
            {
                isRightToLeftText = false;
                base.text = originalText;
            }
            else
            {
                isRightToLeftText = true;
                base.text = GetFixedText(originalText);
            }

            havePropertiesChanged = false;
        }

        private void SetRTL()
        {
            if (!RTLSharedMaterial.TryGetLocale(out var locale))
                return;

            CultureInfo cultureInfo = new CultureInfo(locale.Identifier.Code);
            ForceFix = cultureInfo.TextInfo.IsRightToLeft;

            PreserveNumbers = false;

            if (ForceFix)
                return;

            Regex regex = new Regex("[\u0600-\u06ff]|[\u0750-\u077f]|[\ufb50-\ufc3f]|[\ufe70-\ufefc]");
            ForceFix = regex.IsMatch(originalText);
            if (ForceFix)
                return;
        }

        private string GetFixedText(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            finalText.Clear();
            RTLSupport.FixRTL(input, finalText, Farsi, FixTags, PreserveNumbers);
            finalText.Reverse();

            return finalText.ToString();
        }
    }
}
