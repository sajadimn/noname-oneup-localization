# Font material styles — 1.0.1

Designer decision, 2026-11-26: use the project's existing Fonts Shadow and NoShadow materials. Persian never has an outline or stroke; its Shadow option retains its shadow. English Shadow retains the outline/stroke and shadow authored in its Fonts material. NoShadow has neither effect. Per-label colors remain supported through shared materials. Never modify the project's font/material assets.

## Design

RTLFontChanger selects shared Fonts materials without making per-text instances. RTLSharedMaterial resolves the selected style, using the configured Fonts table/keys, and derives shared color variants only when a label has RTLShadowColor. Material geometry/effects come from Fonts, not arbitrary per-label width/dilation overrides. RTLTextMeshPro and RTLTextMeshPro3D apply the policy before rendering so existing and future objects, locale switches, toggles and custom material assignments follow the same rule. Missing localization data leaves the current material usable and retries later. No dependencies or new configuration assets.

## Focused regression plan

| ID | Assertion | Status |
|---|---|---|
| LO-32 | Persian Shadow rejects custom width/dilation, retains source shadow and colors; source unchanged | Green |
| LO-33 | NoShadow rejects outlines/underlays in both languages | Green |
| LO-34 | English Shadow uses the exact existing source width/dilation/shadow | Green |
| LO-35 | Identical styles share material; source assets remain unchanged | Green |
| LO-36 | UI text and future instances enforce Fonts after custom assignments and locale changes | Green |
| LO-37 | World text enforces the same policy | Green |
| LO-38 | Shadow/NoShadow toggles resolve the correct source and custom colors survive | Green |
| LO-39 | Missing source safely defers styling until Fonts becomes available | Green |
| LO-40 | Existing world NoShadow selection is preserved in both languages; explicit toggles work | Green |
| LO-41 | In-place/instance overrides and re-enable cannot bypass the policy | Green |
| LO-42 | Runtime UI creation, frame updates, custom colors, toggles and locale switches preserve font/material policy | Green |

Package-only validation runs in an isolated Unity project; no game boot or designer save data access. Run only these new cases. Record exact red/green results and version/archive validation below before PR delivery.

Red: Unity 6000.2.13f1 isolated batch EditMode run with filter FontMaterialStyleTests fails compilation with CS0117 for the not-yet-implemented RTLSharedMaterial.ApplyFontStyle API. No runtime tests are claimed as executed at this stage.

LO-42 runtime fixture uses an in-memory completed localization initialization and direct test asset database, because the isolated project intentionally has no Addressables content build. With the fixture corrected, creating a live UI label reproduces a real pre-existing initialization bug: the required RTLFontChanger enables before RTLTextMeshPro exists, dereferences null, and invokes the locale-reset error path. Defer initialization until the text and both selected font/material assets exist; retain the regression assertions.

## Delivery

Version 1.0.1; branch fix/1.0.1-font-material-styles. Build a UPM .tgz with package/ layout, existing GUIDs preserved. Open a PR; leave merge and release publication to the designer.


## Final validation

- Unity 6000.2.13f1, isolated package-only project; designer's CarSort working tree, Editor and save data untouched.
- FontMaterialStyleTests.LO_32…LO_41: 12/12 EditMode cases green. LO-33 and LO-40 each have English/Persian cases. After the final RTLFontChanger initialization change, only its three affected cases LO-36, LO-38 and LO-41 were rerun: 3/3 green.
- FontMaterialRuntimeTests.LO_42_PM_MaterialsStayCorrectAcrossFramesAndLocaleChanges: 1/1 green while entering/exiting PlayMode, including initially missing assets becoming available, live creation, per-frame custom styling, shadow choice, English locale switch, actual font selection and persistent custom colors. Classified as an EditMode UnityTest using EnterPlayMode/ExitPlayMode, not a separate PlayMode runner suite.
- Thirteen distinct new regression cases are covered; no game tests or old package suites ran. Latest compilations have zero C# errors. Source-material tests and variants confirm no source assets are mutated.
- Final shared-color caching check: LO-35, LO-38 and LO-42 rerun after caching each component's colored source variant, 3/3 green; this avoids rebuilding color keys on repeated material reads/render callbacks.
- Actual offscreen camera previews rendered and inspected in both languages: the Shadow row keeps the source shadow; English has its authored outline; Persian has none; both NoShadow rows have neither effect.
- Original font payload copied byte-for-byte from installed 1.0.0 distribution, including original GUIDs, because the GitHub baseline accidentally omitted Fonts~ under the global *~ ignore. It is now explicitly tracked; no font design changes.
- Primary red 6c44960 (missing policy API), initial green 6387931; world compatibility red 83a23b1 / green e695cc9; runtime creation red 2d37fb8 / green 4604d36. Later commits strengthen validation and finish documentation/distribution.
- Documentation updated: README.md, TEST_PLAN.md, AGENTS.md. Shared game-project-kit reference also records the designer's general rule. CarSort integration and its game-specific plain TMP counters remain a separate project PR after package review; this PR changes the package only.

## Manual review checklist

- [ ] Import 1.0.1 in a project using its existing Fonts table; confirm its source assets/GUIDs are unchanged.
- [ ] Check Persian UI/world Shadow labels: shadows remain, no outlines/strokes.
- [ ] Check Persian UI/world NoShadow labels: no shadow or outlines/strokes.
- [ ] Check English Shadow labels: source shadow and outline/stroke remain; English NoShadow has neither.
- [ ] Switch language, toggle HasShadow, instantiate a new label and apply custom label colors.
- [ ] Migrate plain TMP labels to RTL equivalents during each game's integration to receive the package enforcement.
- [ ] Merge this PR, then publish v1.0.1 with the prepared UPM tarball.
