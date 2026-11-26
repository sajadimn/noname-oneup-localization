# Font material styles — 1.0.1

Designer decision, 2026-11-26: use the project's existing Fonts Shadow and NoShadow materials. Persian never has an outline or stroke; its Shadow option retains its shadow. English Shadow retains the outline/stroke and shadow authored in its Fonts material. NoShadow has neither effect. Per-label colors remain supported through shared materials. Never modify the project's font/material assets.

## Design

RTLFontChanger selects shared Fonts materials without making per-text instances. RTLSharedMaterial resolves the selected style, using the configured Fonts table/keys, and derives shared color variants only when a label has RTLShadowColor. Material geometry/effects come from Fonts, not arbitrary per-label width/dilation overrides. RTLTextMeshPro and RTLTextMeshPro3D apply the policy before rendering so existing and future objects, locale switches, toggles and custom material assignments follow the same rule. Missing localization data leaves the current material usable and retries later. No dependencies or new configuration assets.

## Focused regression plan

| ID | Assertion | Status |
|---|---|---|
| LO-32 | Persian Shadow rejects custom width/dilation, retains source shadow and colors; source unchanged | Planned |
| LO-33 | NoShadow rejects outlines/underlays in both languages | Planned |
| LO-34 | English Shadow uses the exact existing source width/dilation/shadow | Planned |
| LO-35 | Identical styles share material; source assets remain unchanged | Planned |
| LO-36 | UI text and future instances enforce Fonts after custom assignments and locale changes | Planned |
| LO-37 | World text enforces the same policy | Planned |
| LO-38 | Shadow/NoShadow toggles resolve the correct source and custom colors survive | Planned |
| LO-39 | Missing source safely defers styling until Fonts becomes available | Planned |
| LO-40 | Existing world NoShadow selection is preserved in both languages; explicit toggles work | Planned |
| LO-41 | In-place/instance overrides and re-enable cannot bypass the policy | Planned |

Package-only validation runs in an isolated Unity project; no game boot or designer save data access. Run only these new cases. Record exact red/green results and version/archive validation below before PR delivery.

Red: Unity 6000.2.13f1 isolated batch EditMode run with filter FontMaterialStyleTests fails compilation with CS0117 for the not-yet-implemented RTLSharedMaterial.ApplyFontStyle API. No runtime tests are claimed as executed at this stage.

## Delivery

Version 1.0.1; branch fix/1.0.1-font-material-styles. Build a UPM .tgz with package/ layout, existing GUIDs preserved. Open a PR; leave merge and release publication to the designer.
