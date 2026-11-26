using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
[ExecuteAlways]
public class RTLShadowColor : MonoBehaviour
{
    [SerializeField] private Color outlineColor = new Color(0f, 0f, 0f, 0.7058824f);
    [SerializeField] private Color underlayColor = new Color(0f, 0f, 0f, 0.7058824f);

    private TMP_Text text;
    private Material colored;
    private Material colorSource;

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
        if (text == null)
            text = GetComponent<TMP_Text>();
        if (text.fontSharedMaterial == null)
            return;
        Material current = text.fontSharedMaterial;
        Material styled = text is RTLTMPro.RTLTextMeshPro || text is RTLTMPro.RTLTextMeshPro3D ?
            RTLSharedMaterial.ForText(text, current) : ApplyColors(current);
        if (styled == current)
            return;
        text.fontSharedMaterial = styled;
        text.UpdateMeshPadding();
    }

    internal Material ApplyColors(Material current)
    {
        if (current == null)
            return null;
        if (current == colorSource && colored != null)
            return colored;
        colorSource = current;
        if (current.GetColor(ShaderUtilities.ID_OutlineColor) == outlineColor &&
            current.GetColor(ShaderUtilities.ID_UnderlayColor) == underlayColor)
        {
            colored = current;
            return colored;
        }

        Color outline = outlineColor;
        Color underlay = underlayColor;
        colored = RTLSharedMaterial.Get(current, RTLSharedMaterial.Key(outline) + " " + RTLSharedMaterial.Key(underlay), material =>
        {
            material.SetColor(ShaderUtilities.ID_OutlineColor, outline);
            material.SetColor(ShaderUtilities.ID_UnderlayColor, underlay);
        });
        return colored;
    }
}
