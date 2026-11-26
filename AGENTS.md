# Localization package — instructions

Use the project's existing Fonts table and Shadow/NoShadow materials; never replace its authored font assets or material settings. Persian has no outline/stroke in either style. Persian Shadow keeps its authored shadow. English Shadow keeps the authored shadow and outline/stroke; NoShadow has neither. All current and future UI/world text must follow this policy after locale switches and per-label styling. Custom label colors use shared variants, never unique per-text instances.

Make changes in this package repository, preserve Unity .meta GUIDs, add focused regression tests before implementation, increment the package version, and deliver a GitHub PR plus a UPM tarball with package/ layout. Do not merge or publish a release without authorization. No new code comments.
