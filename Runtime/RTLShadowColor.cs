using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class RTLShadowColor : MonoBehaviour
{
    [SerializeField] private Color outlineColor = new Color(0f, 0f, 0f, 0.7058824f);
    [SerializeField] private Color underlayColor = new Color(0f, 0f, 0f, 0.7058824f);

    private TMP_Text text;
    private Material colored;

    public void SetColors(Color outline, Color underlay)
    {
        outlineColor = outline;
        underlayColor = underlay;
        colored = null;
    }

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void LateUpdate()
    {
        if (text.fontSharedMaterial == null || text.fontSharedMaterial == colored)
            return;

        Material current = text.fontSharedMaterial;
        if (current.GetColor(ShaderUtilities.ID_OutlineColor) == outlineColor &&
            current.GetColor(ShaderUtilities.ID_UnderlayColor) == underlayColor)
        {
            colored = current;
            return;
        }

        Color outline = outlineColor;
        Color underlay = underlayColor;
        colored = RTLSharedMaterial.Get(current, RTLSharedMaterial.Key(outline) + " " + RTLSharedMaterial.Key(underlay), material =>
        {
            material.SetColor(ShaderUtilities.ID_OutlineColor, outline);
            material.SetColor(ShaderUtilities.ID_UnderlayColor, underlay);
        });
        text.fontSharedMaterial = colored;
    }
}
